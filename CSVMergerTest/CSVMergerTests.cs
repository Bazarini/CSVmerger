using NUnit.Framework;
using CsvMerger;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSVMergerTest
{
    [TestFixture]
    public class Tests
    {

        [TestCase]
        //public void MergeCSV()
        //{

        //    StreamReader expectedReader = new StreamReader(@"E:\Visual Studio\CSVMerger\Test\Output\Out.csv");
        //    StreamReader actualReader = new StreamReader(CSVOperator.MergeFiles(@"E:\Visual Studio\CSVMerger\Test", @"E:\Test.csv").First());
        //    var expected = expectedReader.ReadToEnd();
        //    var actual = actualReader.ReadToEnd();
        //    Assert.AreEqual(expected, actual);
        //}

        [TestCase]
        public void EvenTest()
        {
            List<int> expected = new List<int> { 2, 4, 6 };
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6 };
            List<int> actual = new List<int>(list.Even());
            Assert.AreEqual(expected, actual);
        }

        [TestCase]
        public void OddTest()
        {
            List<int> expected = new List<int> { 1, 3, 5 };
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6 };
            List<int> actual = new List<int>(list.Odd());
            Assert.AreEqual(expected, actual);
        }
        [TestCase]
        public void SplitTest1()
        {
            List<List<int>> expected = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6  } };
            List<int> list = new List<int> { 1, 2, 3, 4, 5, 6 };
            var actual = list.SplitList(3);
            Assert.AreEqual(expected, actual);
        }
        [TestCase]
        public void SplitTest2()
        {
            List<List<int>> expected = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5 } };
            List<int> list = new List<int> { 1, 2, 3, 4, 5 };
            var actual = list.SplitList(3);
            Assert.AreEqual(expected, actual);
        }

    }
}