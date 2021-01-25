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
