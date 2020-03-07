﻿using Penguin.Extensions.Collections;
using Penguin.Extensions.Strings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Penguin.IO.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class FileExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private const string NO_LINES_MESSAGE = "Can not read empty data to table without column headers to use as a schema";

        /// <summary>
        /// Reads a file to a datatable
        /// </summary>
        /// <param name="file">The file info to read</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        [Obsolete("Use ToDataTable", false)]
        public static DataTable ReadToDataTable(this FileInfo file, bool HasHeaders = true, Func<string, string> ProcessRow = null) => ToDataTable(file, HasHeaders, ProcessRow);
        /// <summary>
        /// Reads a file to a datatable
        /// </summary>
        /// <param name="file">The file info to read</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        public static DataTable ToDataTable(this FileInfo file, bool HasHeaders = true, Func<string, string> ProcessRow = null)
        {
            if(file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return ToDataTable(File.ReadAllText(file.FullName).SplitQuotedString('\n', false).Select(l => l.Trim('\r')), HasHeaders, ProcessRow);
        }

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        [Obsolete("Use ToDataTable", false)]
        public static DataTable ReadToDataTable(this IEnumerable<string> FileLines, bool HasHeaders = true, Func<string, string> ProcessRow = null) => FileLines.ToDataTable(HasHeaders, ProcessRow);

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        
        public static DataTable ToDataTable(this IEnumerable<string> FileLines, bool HasHeaders = true, Func<string, string> ProcessRow = null)
        {
            if (FileLines is null)
            {
                throw new ArgumentNullException(nameof(FileLines));
            }

            DataTable toReturn = new DataTable();

            IEnumerator<string> LinesEnumerator = FileLines.GetEnumerator();
          
            bool hasNextLine = LinesEnumerator.MoveNext();

            if (HasHeaders)
            {
                foreach (string Header in LinesEnumerator.Current.SplitQuotedString())
                {
                    toReturn.Columns.Add(new DataColumn(Header));
                }

                hasNextLine = LinesEnumerator.MoveNext();
            }
            else
            {
                if(!hasNextLine)
                {
                    throw new ArgumentException(NO_LINES_MESSAGE, nameof(FileLines));
                }

                foreach (string _ in LinesEnumerator.Current.SplitQuotedString())
                {
                    toReturn.Columns.Add(new DataColumn());
                }
            }

            while (hasNextLine)
            {
                List<object> items = new List<object>();

                foreach (string Column in LinesEnumerator.Current.SplitQuotedString())
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