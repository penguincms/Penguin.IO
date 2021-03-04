using System;

namespace Penguin.IO.Exceptions
{
    /// <summary>
    /// An attempt to move or create a file or directory in a location has failed because it already exists
    /// </summary>
    public class PathAlreadyExistsException : Exception
    {
        public PathAlreadyExistsException(string message) : base(message)
        {
        }

        public PathAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PathAlreadyExistsException()
        {
        }
    }
}