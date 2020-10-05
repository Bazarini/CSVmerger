using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVMergerCore
{
    class Batch : IList<string>
    {
        public string this[int index] { get => Files[index]; set => Files[index] = value; }

        public int ID { get; private set; }
        public List<string> Files { get; private set; }

        #region IList methods
        public int Count => Files.Count;

        public bool IsReadOnly => ((ICollection<string>)Files).IsReadOnly;

        public void Add(string item)
        {
            Files.Add(item);
        }

        public void Clear()
        {
            Files.Clear();
        }

        public bool Contains(string item)
        {
            return Files.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Files.CopyTo(array, arrayIndex);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Files.GetEnumerator();
        }

        public int IndexOf(string item)
        {
            return Files.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            Files.Insert(index, item);
        }

        public bool Remove(string item)
        {
            return Files.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Files.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Files.GetEnumerator();
        }
        #endregion
    }
}
