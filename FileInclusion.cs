using System.Collections.Generic;
using System.IO;

namespace Penguin.IO
{
    public class FileInclusion
    {
        public string Path { get; set; }
        public bool Recursive { get; set; } = true;
        public bool Exclude { get; set; } = false;

        public FileInclusion(string path, bool include = true, bool recursive = true)
        {
            this.Path = path;
            this.Recursive = recursive;
            this.Exclude = !include;
        }

        public FileInclusion()
        {

        }

        public SearchOption SearchOption => Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        public static implicit operator List<FileInclusion>(FileInclusion f) => new List<FileInclusion>() { f };
        public static implicit operator FileInclusion(string path) => new FileInclusion(path);
    }
}
