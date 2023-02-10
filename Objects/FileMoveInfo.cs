using System;
using System.IO;

namespace Penguin.IO.Objects
{
    public class FileMoveInfo
    {
        public FileMoveInfo(FileInfo source, string rootSource, string rootTarget)
        {
            if (string.IsNullOrWhiteSpace(rootSource))
            {
                throw new ArgumentException($"'{nameof(rootSource)}' cannot be null or whitespace.", nameof(rootSource));
            }

            if (string.IsNullOrWhiteSpace(rootTarget))
            {
                throw new ArgumentException($"'{nameof(rootTarget)}' cannot be null or whitespace.", nameof(rootTarget));
            }

            Source = source ?? throw new ArgumentNullException(nameof(source));
            TargetPath = Path.Combine(rootTarget, source.FullName[(rootSource.Length + 1)..]);
        }

        public FileInfo Source { get; set; }
        public string TargetPath { get; set; }
        public bool TargetExists => new FileInfo(TargetPath).Exists;
        public FileMoveResult Result { get; set; }
    }
}