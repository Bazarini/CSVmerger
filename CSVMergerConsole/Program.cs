using System;
using System.IO;
using System.Windows.Forms;

namespace CsvMerger
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string csvFolderPath;
            string csvOutPath;
            Console.WriteLine("Please select a path to the folder with csv's: ");
            using (FolderBrowserDialog csvFolderDialog = new FolderBrowserDialog { ShowNewFolderButton = false })
            {
                DialogResult dialogResult = csvFolderDialog.ShowDialog();
                if (dialogResult != DialogResult.OK)
                    Environment.Exit(0);
                csvFolderPath = csvFolderDialog.SelectedPath;
            }
            Console.WriteLine($"Selected path: {csvFolderPath}\r\n");
            Console.WriteLine("Please select a path to put merged files to");
            using (SaveFileDialog csvOutFolderDialog = new SaveFileDialog() { OverwritePrompt = false, Filter = "Comma-Separated files|*.csv|All files|*.*" })
            {
                DialogResult dialogResult = csvOutFolderDialog.ShowDialog();
                if (dialogResult != DialogResult.OK)
                    Environment.Exit(0);
                csvOutPath = csvOutFolderDialog.FileName;
            };
            Console.WriteLine($"Selected output path: {csvOutPath}\r\n");
            Console.WriteLine("Please put how many documents should be merged into one file.\r\nLeave empty for 100:");
            if (!int.TryParse(Console.ReadLine(), out int filesToMerge))
                filesToMerge = 100;
            CSVOperator cSVOperator = new CSVOperator();
           cSVOperator.AddDJVUToCSV(Directory.GetFiles(csvFolderPath, "*.csv"), csvOutPath);

        }
    }
}
