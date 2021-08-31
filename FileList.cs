using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Penguin.IO
{
    public class FileList : IEnumerable<string>
    {
        HashSet<string> Results = new HashSet<string>();
        
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

            List<string> files;

            if (Directory.Exists(testPath))
            {
                files = Directory.EnumerateFiles(testPath, "*", inclusion.SearchOption).ToList();
            }
            else if (File.Exists(testPath))
            {
                files = new List<string> { testPath };
            }
            else
            {
                files = Directory.EnumerateFiles(this.Root, inclusion.Path, inclusion.SearchOption).ToList();
            }

            foreach (string file in files)
            {
                _ = inclusion.Exclude ? Results.Remove(file) : Results.Add(file);
            }
        }

        public IEnumerator<string> GetEnumerator() => this.Results.ToList().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
