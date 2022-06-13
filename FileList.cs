using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Penguin.IO
{
    public class FileList : IEnumerable<string>
    {
        private HashSet<string> Results = new HashSet<string>();

        private string Root { get; set; }

        public FileList(string root = null)
        {
            this.Root = root ?? Directory.GetCurrentDirectory();
        }

        public void AddRule(FileInclusion inclusion)
        {
            if (inclusion is null)
            {
                throw new ArgumentNullException(nameof(inclusion));
            }

            string testPath = Path.Combine(this.Root, inclusion.Path);

            List<string> files = new List<string>();

            try
            {
                if (Directory.Exists(testPath))
                {
                    files = Directory.EnumerateFiles(testPath, "*", inclusion.SearchOption).ToList();
                }
                else if (File.Exists(testPath))
                {
                    files = new List<string> { testPath };
                }
                else if (Directory.Exists(this.Root))
                {
                    Console.WriteLine($"\t{this.Root}\t{inclusion.Path}\t{inclusion.SearchOption}");
                    files = Directory.EnumerateFiles(this.Root, inclusion.Path, inclusion.SearchOption).ToList();
                }
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("Directory existence check failure");
            }

            foreach (string file in files)
            {
                _ = inclusion.Exclude ? this.Results.Remove(file) : this.Results.Add(file);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.Results.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
