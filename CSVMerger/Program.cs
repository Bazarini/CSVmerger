using System;

namespace CSVMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please select a path to the folder with csv's: ");
            CSVOperator.MergeFiles("C:\\Test", "C:\\Test\\outs\\Out.csv", FilesToMerge : 2);
        }
    }
}
