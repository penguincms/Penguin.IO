using System.IO;

namespace Penguin.IO.Objects
{
    public enum DirectoryMoveResultKind
    {
        Renamed,
        Moved,
        Skipped,
        Merged
    }

    public class DirectoryMoveResult
    {
        public DirectoryInfo DirectoryInfo { get; set; }
        public DirectoryMoveResultKind Result { get; set; }
    }
}