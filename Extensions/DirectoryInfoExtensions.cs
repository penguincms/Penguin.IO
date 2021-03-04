using Penguin.IO.Exceptions;
using Penguin.IO.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Penguin.IO.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class DirectoryInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Enumerate files but calls an exception handler method for exceptions instead of bombing out
        /// </summary>
        /// <param name="di">Target DirectoryInfo</param>
        /// <param name="onError">Action to call on error</param>
        /// <param name="searchPattern">Search mask. * by default</param>
        /// <param name="searchOption">Recursive or not. Recursive by default</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo di, Action<DirectoryInfo, Exception> onError, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            return EnumerateEntries<FileInfo>(di, onError, searchPattern, searchOption);
        }

        /// <summary>
        /// Enumerate Directories but calls an exception handler method for exceptions instead of bombing out
        /// </summary>
        /// <param name="di">Target DirectoryInfo</param>
        /// <param name="onError">Action to call on error</param>
        /// <param name="searchPattern">Search mask. * by default</param>
        /// <param name="searchOption">Recursive or not. Recursive by default</param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo di, Action<DirectoryInfo, Exception> onError, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            return EnumerateEntries<DirectoryInfo>(di, onError, searchPattern, searchOption);
        }

        /// <summary>
        /// Enumerate FileSystemInfos but calls an exception handler method for exceptions instead of bombing out
        /// </summary>
        /// <param name="di">Target DirectoryInfo</param>
        /// <param name="onError">Action to call on error</param>
        /// <param name="searchPattern">Search mask. * by default</param>
        /// <param name="searchOption">Recursive or not. Recursive by default</param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo di, Action<DirectoryInfo, Exception> onError, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            return EnumerateEntries<FileSystemInfo>(di, onError, searchPattern, searchOption);
        }

        public static DirectoryMoveResult MoveTo(this DirectoryInfo directory, string newPath, ExistingDirectoryBehaviour existingDirectoryBehaviour, ExistingFileBehaviour existingFileBehaviour = ExistingFileBehaviour.Error)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrWhiteSpace(newPath))
            {
                throw new ArgumentException($"'{nameof(newPath)}' cannot be null or whitespace.", nameof(newPath));
            }

            DirectoryInfo newDir = new DirectoryInfo(newPath);

            if (!Directory.Exists(newPath))
            {
                if (string.Equals(newDir.Root, directory.Root))
                {
                    directory.MoveTo(newPath);

                    return new DirectoryMoveResult()
                    {
                        DirectoryInfo = new DirectoryInfo(newPath),
                        Result = DirectoryMoveResultKind.Moved
                    };
                } else
                {
                    directory.RecursiveMerge(newPath, existingFileBehaviour);
                    return new DirectoryMoveResult()
                    {
                        DirectoryInfo = new DirectoryInfo(newPath),
                        Result = DirectoryMoveResultKind.Merged
                    };
                }
            }
            else
            {
                switch (existingDirectoryBehaviour)
                {
                    case ExistingDirectoryBehaviour.Error:
                        throw new DirectoryAlreadyExistsException();
                    case ExistingDirectoryBehaviour.Merge:
                        directory.RecursiveMerge(newPath, existingFileBehaviour);
                        return new DirectoryMoveResult()
                        {
                            DirectoryInfo = new DirectoryInfo(newPath),
                            Result = DirectoryMoveResultKind.Merged
                        };

                    case ExistingDirectoryBehaviour.Rename:
                        newPath = IOHelper.FindDirectoryName(newPath);
                        directory.MoveTo(newPath);
                        return new DirectoryMoveResult()
                        {
                            DirectoryInfo = new DirectoryInfo(newPath),
                            Result = DirectoryMoveResultKind.Renamed
                        };

                    case ExistingDirectoryBehaviour.Skip:
                        return new DirectoryMoveResult()
                        {
                            DirectoryInfo = new DirectoryInfo(directory.FullName),
                            Result = DirectoryMoveResultKind.Skipped
                        };

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static void Merge(this DirectoryInfo source, string newPath, ExistingFileBehaviour existingFileBehaviour = ExistingFileBehaviour.Error)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!Directory.Exists(newPath))
            {
                throw new DirectoryNotFoundException(newPath);
            }

            source.RecursiveMerge(newPath, existingFileBehaviour);
        }

        private static List<FileMoveResult> RecursiveMerge(this DirectoryInfo source, string newpath, ExistingFileBehaviour existingFileBehaviour = ExistingFileBehaviour.Error)
        {
            List<FileMoveInfo> MoveInfo = new List<FileMoveInfo>();

            foreach (FileInfo fi in source.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                MoveInfo.Add(new FileMoveInfo(fi, source.FullName, newpath));
            }

            if (existingFileBehaviour == ExistingFileBehaviour.Error)
            {
                FileMoveInfo firstExists = MoveInfo.FirstOrDefault(f => f.TargetExists);

                if (firstExists != null)
                {
                    throw new FileAlreadyExistsException(firstExists.TargetPath);
                }
            }

            foreach (FileMoveInfo thisMoveInfo in MoveInfo)
            {
                DirectoryInfo parentDirectory = new FileInfo(thisMoveInfo.TargetPath).Directory;

                if (!parentDirectory.Exists)
                {
                    parentDirectory.Create();
                }

                thisMoveInfo.Source.MoveTo(thisMoveInfo.TargetPath, existingFileBehaviour);
            }

            List<DirectoryInfo> toRemove = source.EnumerateDirectories("*", SearchOption.AllDirectories).OrderByDescending(d => d.FullName.Length).ToList();

            foreach (DirectoryInfo di in toRemove)
            {
                if (!di.EnumerateFileSystemInfos().Any())
                {
                    di.Delete();
                }
            }

            if (!source.EnumerateFileSystemInfos().Any())
            {
                source.Delete();
            }

            return MoveInfo.Select(m => m.Result).ToList();
        }

        private static IEnumerable<T> EnumerateEntries<T>(this DirectoryInfo di, Action<DirectoryInfo, Exception> onError, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories) where T : FileSystemInfo
        {
            Queue<DirectoryInfo> directories = new Queue<DirectoryInfo>();

            directories.Enqueue(di);

            while (directories.Any())
            {
                DirectoryInfo thisDir = directories.Dequeue();

                List<T> entries = new List<T>();

                try
                {
                    foreach (FileSystemInfo fi in thisDir.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
                    {
                        if (fi is T fii)
                        {
                            entries.Add(fii);
                        }

                        if (fi is DirectoryInfo dii)
                        {
                            if (searchOption == SearchOption.AllDirectories)
                            {
                                directories.Enqueue(dii);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    onError?.Invoke(thisDir, ex);
                }

                foreach (T fi in entries)
                {
                    yield return fi;
                }
            }
        }
    }
}