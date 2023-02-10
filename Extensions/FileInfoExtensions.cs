using Penguin.Extensions.String;
using Penguin.IO.Exceptions;
using Penguin.IO.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Penguin.IO.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class FileInfoExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string NO_LINES_MESSAGE = "Can not read empty data to table without column headers to use as a schema";

        /// <summary>
        /// Reads a file to a datatable
        /// </summary>
        /// <param name="file">The file info to read</param>
        /// <param name="options">Optional Csv parsing options</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        [Obsolete("Use ToDataTable", false)]
        public static DataTable ReadToDataTable(this FileInfo file, CsvOptions options = null, Func<string, string> ProcessRow = null)
        {
            return ToDataTable(file, options, ProcessRow);
        }

        /// <summary>
        /// Reads a file to a datatable
        /// </summary>
        /// <param name="file">The file info to read</param>
        /// <param name="options">Optional Csv parsing options</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        public static DataTable ToDataTable(this FileInfo file, CsvOptions options = null, Func<string, string> ProcessRow = null)
        {
            options ??= new CsvOptions();

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            long fileLength = file.Length;

            IEnumerable<char> toParse = fileLength < 2_000_000_000 ? File.ReadAllText(file.FullName) : ReadFile(file.FullName);
            DataTable table = ToDataTable(toParse.SplitCsvLines(options.LineDelimeter).Select(l => l.Trim('\r')), options, ProcessRow);

            table.TableName = Path.GetFileNameWithoutExtension(file.FullName);

            return table;
        }

        /// <summary>
        /// Reads a file as an IEnumerable of strings, seperated by CSV line. This means that CSV cells may contain newlines
        /// </summary>
        /// <param name="fi">the File Info</param>
        /// <param name="lineDelimeter">The character to be user for line breaks</param>
        /// <returns>An IEnumerable of CSV lines</returns>
        public static IEnumerable<string> ReadCsvLines(this FileInfo fi, char lineDelimeter = '\n')
        {
            return fi is null
                ? throw new ArgumentNullException(nameof(fi))
                : ReadFile(fi).SplitCsvLines(lineDelimeter).Select(l => l.Trim('\r'));
        }

        private static IEnumerable<string> SplitCsvLines(this IEnumerable<char> source, char lineDelimeter)
        {
            return source.SplitQuotedString(new QuotedStringOptions() { RemoveQuotes = false, ItemDelimeter = lineDelimeter });
        }

        /// <summary>
        /// Reads an IEnumerable of CSV lines, each containing an IEnumerable of CSV cell values
        /// </summary>
        /// <param name="fi">The file info to read</param>
        /// <param name="options"></param>
        /// <returns>An IEnumerable of CSV lines, each containing an IEnumerable of CSV cell values</returns>
        public static IEnumerable<IEnumerable<string>> ReadCsvItems(this FileInfo fi, CsvOptions options = null)
        {
            options ??= new CsvOptions();

            return fi is null
                ? throw new ArgumentNullException(nameof(fi))
                : ReadFile(fi).SplitCsvLines(options.LineDelimeter).Select(l => l.Trim('\r').SplitQuotedString(options));
        }

        /// <summary>
        /// Reads a file as an IEnumerable of characters
        /// </summary>
        /// <param name="fi">The file to read</param>
        /// <returns>The IEnumerable of characters representing the content</returns>
        public static IEnumerable<char> ReadFile(this FileInfo fi)
        {
            return fi is null ? throw new ArgumentNullException(nameof(fi)) : ReadFile(fi.FullName);
        }

        private static IEnumerable<char> ReadFile(string filePath)
        {
            using StreamReader sr = new(filePath);
            int i;
            while ((i = sr.Read()) != -1)
            {
                yield return (char)i;
            }
        }

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="options">Optional csv parsing options</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        [Obsolete("Use ToDataTable", false)]
        public static DataTable ReadToDataTable(this IEnumerable<string> FileLines, CsvOptions options = null, Func<string, string> ProcessRow = null)
        {
            return FileLines.ToDataTable(options, ProcessRow);
        }

        /// <summary>
        /// Moves a file info to a new location, allowing specification of behaviour
        /// </summary>
        /// <param name="file">The file to move</param>
        /// <param name="newPath">The target path for the file</param>
        /// <param name="behaviour">How conflicting files should be handled</param>
        /// <returns>An object containing information relevant to the operation</returns>
        public static FileMoveResult MoveTo(this FileInfo file, string newPath, ExistingFileBehaviour behaviour)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (newPath is null)
            {
                throw new ArgumentNullException(nameof(newPath));
            }

            if (!File.Exists(newPath))
            {
                file.MoveTo(newPath);

                return new FileMoveResult()
                {
                    FileInfo = new FileInfo(newPath),
                    Result = FileMoveResultKind.Moved
                };
            }
            else
            {
                switch (behaviour)
                {
                    case ExistingFileBehaviour.Error:
                        throw new FileAlreadyExistsException();
                    case ExistingFileBehaviour.Overwrite:
                        File.Delete(newPath);
                        file.MoveTo(newPath);
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(newPath),
                            Result = FileMoveResultKind.Overwritten
                        };

                    case ExistingFileBehaviour.Rename:
                        newPath = IOHelper.FindFileName(newPath);
                        file.MoveTo(newPath);
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(newPath),
                            Result = FileMoveResultKind.Renamed
                        };

                    case ExistingFileBehaviour.Skip:
                        return new FileMoveResult()
                        {
                            FileInfo = new FileInfo(file.FullName),
                            Result = FileMoveResultKind.Skipped
                        };

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="options">Optional csv parsing options</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>

        public static DataTable ToDataTable(this IEnumerable<string> FileLines, CsvOptions options = null, Func<string, string> ProcessRow = null)
        {
            options ??= new CsvOptions();

            if (FileLines is null)
            {
                throw new ArgumentNullException(nameof(FileLines));
            }

            DataTable toReturn = new();

            IEnumerator<string> LinesEnumerator = FileLines.GetEnumerator();

            bool hasNextLine = LinesEnumerator.MoveNext();

            if (options.HasHeaders)
            {
                foreach (string Header in LinesEnumerator.Current.SplitQuotedString(options))
                {
                    toReturn.Columns.Add(new DataColumn(Header));
                }

                hasNextLine = LinesEnumerator.MoveNext();
            }
            else
            {
                if (!hasNextLine)
                {
                    throw new ArgumentException(NO_LINES_MESSAGE, nameof(FileLines));
                }

                foreach (string _ in LinesEnumerator.Current.SplitQuotedString(options))
                {
                    toReturn.Columns.Add(new DataColumn());
                }
            }

            while (hasNextLine)
            {
                List<object> items = new();

                foreach (string Column in LinesEnumerator.Current.SplitQuotedString(options))
                {
                    items.Add(ProcessRow != null ? ProcessRow(Column) : Column);
                }

                DataRow thisRow = toReturn.NewRow();

                thisRow.ItemArray = items.ToArray();

                toReturn.Rows.Add(thisRow);

                hasNextLine = LinesEnumerator.MoveNext();
            }

            return toReturn;
        }
    }
}