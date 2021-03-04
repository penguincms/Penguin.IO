using System;

namespace Penguin.IO.Exceptions
{
    /// <summary>
    /// An attempt to move or create a directory in a location has failed because it already exists
    /// </summary>
    public class DirectoryAlreadyExistsException : PathAlreadyExistsException
    {
        public DirectoryAlreadyExistsException(string message) : base(message)
        {
        }

        public DirectoryAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DirectoryAlreadyExistsException()
        {
        }
    }
}