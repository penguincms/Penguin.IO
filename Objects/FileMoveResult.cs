using System.IO;

namespace Penguin.IO.Objects
{
    public enum FileMoveResultKind
    {
        Renamed,
        Moved,
        Skipped,
        Overwritten
    }

    public class FileMoveResult
    {
        public FileInfo FileInfo { get; set; }

        public FileMoveResultKind Result { get; set; }
    }
}