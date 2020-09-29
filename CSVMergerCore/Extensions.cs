using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvMerger
{
    public static class EnumerableExtensions
    {
        public static List<List<T>> SplitList<T>(this List<T> input, int nSize)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < input.Count; i += nSize)
            {
                list.Add(input.GetRange(i, Math.Min(nSize, input.Count - i)));
            }
            return list;
        }
        public static IEnumerable<T> Odd<T>(this IEnumerable<T> collection)
        {
            IEnumerable<T> output = new List<T>();
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                output = output.Append(enumerator.Current);
                enumerator.MoveNext();
            }
            return output;
        }

        public static IEnumerable<T> Even<T>(this IEnumerable<T> collection)
        {
            IEnumerable<T> output = new List<T>();
            var enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            while (enumerator.MoveNext())
            {
                output = output.Append(enumerator.Current);
                enumerator.MoveNext();
            }
            return output;
        }
    }
    public static class StringExtensions
    {
        public static string AddIndex(this string input, int startIndex = 1, int minDigits = 1)
        {
            Regex regex = new Regex(@"[(]([0-9])[)]");
            if (!regex.IsMatch(input))
                return $"{Path.GetDirectoryName(input)}\\{Path.GetFileNameWithoutExtension(input)} ({startIndex}){Path.GetExtension(input)}";
            int currentIndex = int.Parse(regex.Match(input).Groups[1].Value);
            int outputIndex = currentIndex < startIndex ? startIndex : currentIndex + 1;
            return regex.Replace(input, $"({outputIndex})");
        }
    }
    public static class ReaderExt
    {
        public static string[] GetLines(this System.IO.StreamReader reader)
        {
            string content = reader.ReadToEnd();
            string[] lines = content.Split('\r');
            return lines.Where(s => s.Replace("\n", "") != "").Select(s => s.Replace("\n", "")).ToArray();
        }
    }

}
