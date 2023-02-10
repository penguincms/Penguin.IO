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
        private readonly HashSet<string> Results = new();

        private string Root { get; set; }

        public FileList(string root = null)
        {
            Root = root ?? Directory.GetCurrentDirectory();
        }

        public void AddRule(FileInclusion inclusion)
        {
            if (inclusion is null)
            {
                throw new ArgumentNullException(nameof(inclusion));
            }

            string testPath = Path.Combine(Root, inclusion.Path);

            List<string> files = new();

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
                else if (Directory.Exists(Root))
                {
                    Console.WriteLine($"\t{Root}\t{inclusion.Path}\t{inclusion.SearchOption}");
                    files = Directory.EnumerateFiles(Root, inclusion.Path, inclusion.SearchOption).ToList();
                }
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("Directory existence check failure");
            }

            foreach (string file in files)
            {
                _ = inclusion.Exclude ? Results.Remove(file) : Results.Add(file);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Results.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}