﻿using System.IO;

namespace Penguin.IO.Objects
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class IOHelper
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Given a file path, finds a path that does not exist already using standard windows rename logic (ex Picture (1).jpg)
        /// </summary>
        /// <param name="path">The intended path</param>
        /// <returns>The free path, either original or renamed</returns>
        public static string FindFileName(string path)
        {
            FileInfo fi = new FileInfo(path);
            DirectoryInfo di = fi.Directory;

            string originalFileName = Path.GetFileNameWithoutExtension(path);

            string ext = Path.GetExtension(path);

            string fileName = $"{originalFileName}{ext}";

            string parentDir = di.FullName;

            string newPath = Path.Combine(parentDir, fileName);

            int i = 0;

            while (File.Exists(newPath))
            {
                fileName = $"{originalFileName} ({++i}){ext}";

                newPath = Path.Combine(parentDir, $"{fileName}");
            }

            return newPath;
        }

        /// <summary>
        /// Given a directory path, finds a path that does not exist already using standard windows rename logic (ex Images (1))
        /// </summary>
        /// <param name="path">The intended path</param>
        /// <returns>The free path, either original or renamed</returns>
        public static string FindDirectoryName(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            string originalFolderName = di.Name;

            string folderName = originalFolderName;

            string parentDir = di.Parent.FullName;

            string newPath = Path.Combine(parentDir, folderName);

            int i = 0;

            while (Directory.Exists(newPath))
            {
                folderName = $"{originalFolderName} ({++i})";

                newPath = Path.Combine(parentDir, folderName);
            }

            return newPath;
        }
    }
}