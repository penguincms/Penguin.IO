using Penguin.Extensions.String;
using Penguin.IO.Exceptions;
using Penguin.IO.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Penguin.IO.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class FileInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

        /// <summary>
        /// Moves a file info to a new location, allowing specification of behaviour
        /// </summary>
        /// <param name="file">The file to move</param>
        /// <param name="newPath">The target path for the file</param>
        /// <param name="behaviour">How conflicting files should be handled</param>
        /// <returns>An object containing information relevant to the operation</returns>
        public static FileMoveResult MoveTo(this FileInfo file, string newPath, ExistingFileBehaviour behaviour)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (newPath is null)
            {
                throw new ArgumentNullException(nameof(newPath));
            }

            if (!File.Exists(newPath))
            {
                file.MoveTo(newPath);

                return new FileMoveResult()
                {
                    FileInfo = new FileInfo(newPath),
                    Result = FileMoveResultKind.Moved
                };
            }
            else
            {
                switch (behaviour)
                {
                    case ExistingFileBehaviour.Error:
                        throw new FileAlreadyExistsException();
                    case ExistingFileBehaviour.Overwrite:
                        File.Delete(newPath);
                        file.MoveTo(newPath);
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(newPath),
                            Result = FileMoveResultKind.Overwritten
                        };

                    case ExistingFileBehaviour.Rename:
                        newPath = IOHelper.FindFileName(newPath);
                        file.MoveTo(newPath);
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(newPath),
                            Result = FileMoveResultKind.Renamed
                        };

                    case ExistingFileBehaviour.Skip:
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(file.FullName),
                            Result = FileMoveResultKind.Skipped
                        };

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="options">Optional csv parsing options</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>

   
    }
}