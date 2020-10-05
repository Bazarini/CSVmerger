using CsvMerger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVMergerCore
{
    public class CSVDocument : IList<CSVRow>
    {
        public List<CSVRow> Rows { get; private set; }
        public IEnumerable<string> Headers { get; private set; }
        public string HeadersToString()
        {
            return string.Join(";", Headers.Where(w => w != "DJVUIMAGES"));
        }
        #region IList props
        public int Count => Rows.Count;
        public bool IsReadOnly => ((ICollection<CSVRow>)Rows).IsReadOnly;
        public CSVRow this[int index] { get => Rows[index]; set => Rows[index] = value; }
        #endregion

        #region ctors
        public CSVDocument()
        {
            Rows = new List<CSVRow>();
            Headers = new List<string>();
        }
        public CSVDocument(IEnumerable<CSVRow> pages)
        {
            Rows = pages.ToList();
            Headers = pages.First().Headers;
        }
        #endregion

        public static CSVDocument FromFile(string filePath)
        {
            const string separator = "\";\"";
            CSVDocument output = new CSVDocument();
            string[] lines;
            using (StreamReader reader = new StreamReader(filePath))
                lines = reader.GetLines().ToArray();
            if (lines.Length % 2 != 0)
                throw new Exception($"Error: file {filePath} is incorrect. Missing headers or contents");
            for (int i = 0; i < lines.Length; i += 2)
            {
                IEnumerable<string> lineHeaders = lines[i].Split(new[] { separator }, StringSplitOptions.None).Select(s => s.Replace("\"", ""));
                IEnumerable<string> lineContents = lines[i + 1].Split(new[] { separator }, StringSplitOptions.None).Select(s => s.Replace("\"", ""));
                if (lineHeaders.Count() != lineContents.Count())
                    throw new Exception($"File {filePath} rows are split incorrectly");
                Dictionary<string, string> contents = lineHeaders.Zip(lineContents, (h, m) => new { h, m }).ToDictionary(item => item.h, item => item.m);
                CSVRow row = new CSVRow(contents);
                output.Add(row);
            }
            return output;
        }
        #region IList methods
        private bool CheckHeaders(CSVRow row)
        {         
            if (Headers.SequenceEqual(row.Headers))
                return true;
            return false;
        }
        public int IndexOf(CSVRow item)
        {
            return Rows.IndexOf(item);
        }

        public void Insert(int index, CSVRow item)
        {
            if (Rows.Count == 0)
                Headers = new List<string>(item.Headers);
            if (CheckHeaders(item))
                Rows.Insert(index, item);            
            else
                throw new Exception($"Headers are not equal {string.Join(";", Headers)} ({Rows.Count}) and  {string.Join(";", item.Headers)}");
        }

        public void RemoveAt(int index)
        {
            Rows.RemoveAt(index);
        }

        public void Add(CSVRow item)
        {
            if (Rows.Count == 0)
                Headers = new List<string>(item.Headers);
            if (CheckHeaders(item))
                Rows.Add(item);            
            else
                throw new Exception($"Headers are not equal {string.Join(";", Headers)} ({Rows.Count}) and  {string.Join(";", item.Headers)}");
        }

        public void Clear()
        {
            Rows.Clear();
        }

        public bool Contains(CSVRow item)
        {
            return Rows.Contains(item);
        }

        public void CopyTo(CSVRow[] array, int arrayIndex)
        {
            Rows.CopyTo(array, arrayIndex);
        }

        public bool Remove(CSVRow item)
        {
            return Rows.Remove(item);
        }

        IEnumerator<CSVRow> IEnumerable<CSVRow>.GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Rows.GetEnumerator();
        }
        #endregion

    }
}
