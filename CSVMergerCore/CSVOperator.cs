using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CSVMergerCore;
using Logger;

namespace CsvMerger
{
    public class CSVOperator
    {
        public void AddDJVUToCSV(string[] paths, string outputFile, int dpi = 250) //Gets the list of CSV files, adds djvu document and merges all csvs to output file.
        {
            List<CSVDocument> mergedCSV = new List<CSVDocument>();
            List<Task> tasks = new List<Task>();
            try
            {
                Parallel.ForEach(paths, file =>
                {                    
                    LogWriter.LogMessage($"Reading file {file}");
                    CSVDocument csvDocument = CSVDocument.FromFile(file);
                    CSVRow anyRow = csvDocument.Rows[0];
                    var pagePaths = csvDocument.Rows.Select(s => s.PathToPDF); //Paths to PDF documents within one csv
                    foreach (var pagePath in pagePaths)
                    {
                        if (!File.Exists(pagePath))
                        {
                            Exception exception = new FileNotFoundException(pagePath + " does not exists");
                            throw exception;
                        }
                    }
                    Regex regex = new Regex("[-][0-9]{4}[.]pdf");
                    var outputDJVUPath = regex.Replace(pagePaths.FirstOrDefault(), ".djvu"); //Form djvu name/path from the first PDF name (document-0001.pdf => document.djvu)                
                    PDFToDJVU.Executor.Convert(pagePaths.ToArray(), outputDJVUPath, dpi); // Convert pdfs to djvu
                    CSVRow DJVUFullDocument = (CSVRow)anyRow.Clone(); //Template for djvu
                    LogWriter.LogMessage(anyRow.PathToPDF);
                    LogWriter.LogMessage(DJVUFullDocument.PathToPDF);
                    DJVUFullDocument.Content["PAGES"] = "0";
                    DJVUFullDocument.PathToPDF = outputDJVUPath;
                    LogWriter.LogMessage(anyRow.PathToPDF);
                    LogWriter.LogMessage(DJVUFullDocument.PathToPDF);
                    LogWriter.LogMessage("Adding a row with DJVUDocument");
                    LogWriter.LogMessage(string.Join(";", DJVUFullDocument.Keys) + "\r\n" + string.Join(";", DJVUFullDocument.Values));
                    csvDocument.Add(DJVUFullDocument); //Add DJVU to csv                       
                    mergedCSV.Add(csvDocument); // Add csv to list of all csvs.
                });
                using (StreamWriter writer = new StreamWriter(outputFile, false)) //Write all csv's with djvu to output "Merged" csv
                {
                    LogWriter.LogMessage($"Opening file {outputFile} to write.");
                    writer.WriteLine(mergedCSV[0].HeadersToString()); //Write headers
                    foreach (var document in mergedCSV)
                    {
                        foreach (CSVRow page in document.Rows.OrderBy(o => int.Parse(o.Content["PAGES"]))) //Sort rows in each csv (djvu should be on a first place with PAGES = 0)
                            writer.WriteLine(page.ToString()); //Write each row in csv
                    }
                }
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.Message);
                LogWriter.LogMessage(e.StackTrace);
                if (e.InnerException != null)
                {
                    LogWriter.LogMessage(e.InnerException.Message);
                    LogWriter.LogMessage(e.InnerException.StackTrace);
                    if (e.InnerException.InnerException != null)
                    {
                        LogWriter.LogMessage(e.InnerException.InnerException.Message);
                        LogWriter.LogMessage(e.InnerException.InnerException.StackTrace);
                    }
                }
            }
        }
    }
}
