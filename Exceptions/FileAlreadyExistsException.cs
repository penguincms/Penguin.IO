using System;

namespace Penguin.IO.Exceptions
{
    /// <summary>
    /// An attempt to move or create a File in a location has failed because it already exists
    /// </summary>
    public class FileAlreadyExistsException : PathAlreadyExistsException
    {
        public FileAlreadyExistsException(string message) : base(message)
        {
        }

        public FileAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FileAlreadyExistsException()
        {
        }
    }
}