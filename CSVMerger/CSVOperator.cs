using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVMerger
{
    class CSVOperator
    {
        public static void MergeFiles(string[] paths, string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                Console.WriteLine("Opening file {0} to write.", outputPath);
                using (StreamReader reader = new StreamReader(paths[0]))
                    writer.WriteLine(reader.ReadLine());
                Console.WriteLine("Writing header.");
                foreach (string filePath in paths)
                {
                    Console.WriteLine("Opening file: {0}", filePath);
                    Console.WriteLine("Reading even rows.");
                    using (StreamReader reader = new StreamReader(filePath))
                        foreach (string line in reader.GetLines().Even())
                            writer.WriteLine(line);
                    Console.WriteLine("Writing rows from {0} to {1} .", filePath, outputPath);
                    Console.WriteLine("File {0} is added to {1} .", filePath, outputPath);
                }
            }
        }
        public static void MergeFiles(string folder, string outputPath, string inputFormat = ".csv", int FilesToMerge = 100)
        {
            List<string> files = Directory.GetFiles(folder).Where(w => w.Contains(inputFormat)).ToList();
            for (int i = 0; i < files.Count; i += FilesToMerge)
            {
                int countOfFilesToMerge = FilesToMerge;
                if (i + FilesToMerge > files.Count)
                    countOfFilesToMerge = files.Count - i;
                while (File.Exists(outputPath))
                {
                    outputPath = outputPath.AddIndex(1);
                }
                MergeFiles(files.GetRange(i, countOfFilesToMerge).ToArray(), outputPath);
            }

        }

    }
}
