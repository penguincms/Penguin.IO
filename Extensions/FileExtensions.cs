using Penguin.Extensions.Collections;
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
        /// <summary>
        /// Reads a file to a datatable
        /// </summary>
        /// <param name="file">The file info to read</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        public static DataTable ReadToDataTable(this FileInfo file, bool HasHeaders = true, Func<string, string> ProcessRow = null)
        {
            return ReadToDataTable(File.ReadAllLines(file.FullName).ToList(), HasHeaders, ProcessRow);
        }

        /// <summary>
        /// Reads a list of strings representing CSV lines to a datatable
        /// </summary>
        /// <param name="FileLines">The CSV lines</param>
        /// <param name="HasHeaders">A bool indicating if the first row is a header row</param>
        /// <param name="ProcessRow">A function to be called once per row to alter the row contents</param>
        /// <returns>A data table representing the contents of the source fileinfo</returns>
        public static DataTable ReadToDataTable(List<string> FileLines, bool HasHeaders = true, Func<string, string> ProcessRow = null)
        {
            Queue<string> Lines = FileLines.ToQueue();

            DataTable toReturn = new DataTable();

            if (HasHeaders)
            {
                foreach (string Header in Lines.Dequeue().SplitCSVRow())
                {
                    toReturn.Columns.Add(new DataColumn(Header));
                }
            }
            else
            {
                foreach (string column in Lines.Peek().SplitCSVRow())
                {
                    toReturn.Columns.Add(new DataColumn());
                }
            }

            while (Lines.Any())
            {
                List<object> items = new List<object>();

                foreach (string Column in Lines.Dequeue().SplitCSVRow())
                {
                    items.Add(ProcessRow != null ? ProcessRow(Column) : Column);
                }

                DataRow thisRow = toReturn.NewRow();

                thisRow.ItemArray = items.ToArray();

                toReturn.Rows.Add(thisRow);
            }

            return toReturn;
        }
    }
}