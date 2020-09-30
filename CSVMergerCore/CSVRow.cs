using System;
using System.Collections;
using System.Collections.Generic;

namespace CSVMergerCore
{
    public class CSVRow : IDictionary, ICloneable
    {
        public Dictionary<string, string> Content { get; private set; }
        public IEnumerable<string> Headers
        {
            get
            {
                return Content.Keys;
            }
        }
        #region IDictionary properties
        public string PathToPDF
        {
            get => Content["FILES"];
            set => Content["FILES"] = value;
        }
        public ICollection Keys => ((IDictionary)Content).Keys;
        public ICollection Values => ((IDictionary)Content).Values;
        public bool IsReadOnly => ((IDictionary)Content).IsReadOnly;
        public bool IsFixedSize => ((IDictionary)Content).IsFixedSize;
        public int Count => ((ICollection)Content).Count;
        public object SyncRoot => ((ICollection)Content).SyncRoot;
        public bool IsSynchronized => ((ICollection)Content).IsSynchronized;
        public object this[object key] { get => ((IDictionary)Content)[key]; set => ((IDictionary)Content)[key] = value; }
        #endregion        
        public CSVRow()
        {
            Content = new Dictionary<string, string>();
        }
        public CSVRow(Dictionary<string, string> content)
        {
            Content = content;
        }
        public override string ToString()
        {
            return string.Join(";", Content.Values);
        }
        #region IDictionary methods
        public bool Contains(object key)
        {
            return ((IDictionary)Content).Contains(key);
        }
        public void Add(object key, object value)
        {
            ((IDictionary)Content).Add(key, value);
        }
        public void Clear()
        {
            ((IDictionary)Content).Clear();
        }
        public IDictionaryEnumerator GetEnumerator()
        {
            return ((IDictionary)Content).GetEnumerator();
        }
        public void Remove(object key)
        {
            ((IDictionary)Content).Remove(key);
        }
        public void CopyTo(Array array, int index)
        {
            ((ICollection)Content).CopyTo(array, index);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Content).GetEnumerator();
        }
        #endregion
        public object Clone()
        {
            CSVRow output = new CSVRow();
            output.Content = new Dictionary<string, string>(Content);
            return output;
        }

    }
}
