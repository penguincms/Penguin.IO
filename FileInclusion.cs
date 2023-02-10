using System.Collections.Generic;
using System.IO;

namespace Penguin.IO
{
    public class FileInclusion
    {
        public string Path { get; set; }

        public bool Recursive { get; set; } = true;

        public bool Exclude { get; set; }

        public FileInclusion(string path, bool include = true, bool recursive = true)
        {
            Path = path;
            Recursive = recursive;
            Exclude = !include;
        }

        public FileInclusion()
        {
        }

        public SearchOption SearchOption => Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        public static implicit operator List<FileInclusion>(FileInclusion f)
        {
            return new List<FileInclusion>() { f };
        }

        public static implicit operator FileInclusion(string path)
        {
            return new FileInclusion(path);
        }

        public List<FileInclusion> ToList()
        {
            throw new System.NotImplementedException();
        }

        public FileInclusion ToFileInclusion()
        {
            throw new System.NotImplementedException();
        }
    }
}