﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CSVMergerCore;
using Logger;

namespace CsvMerger
{
    public class CSVOperator
    {
        CancellationTokenSource cancellationTokenSource;
        public bool AddDJVUToCSV(string[] paths, string outputFile, int maxParallels, int dpi = 250) //Gets the list of CSV files, adds djvu document and merges all csvs to output file.
        {
            List<CSVDocument> mergedCSV = new List<CSVDocument>();
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                Parallel.ForEach(paths, new ParallelOptions() { MaxDegreeOfParallelism = maxParallels, CancellationToken = token }, file =>
                {
                    LogWriter.LogMessage($"Starting parallel for {file}", LogDepth.Debug);
                    mergedCSV.Add(AddDJVUToCsv(file, dpi, cancellationTokenSource)); // Add csv to list of all csvs.
                });
                string content = "";
                LogWriter.LogMessage($"Opening file {outputFile} to write.", LogDepth.UserLevel);
                content += mergedCSV[0].HeadersToString(); //Write headers
                
                foreach (var document in mergedCSV)
                {
                    foreach (CSVRow page in document.Rows.OrderBy(o => int.Parse(o.Content["PAGES"]))) //Sort rows in each csv (djvu should be on a first place with PAGES = 0)
                        content += "\r\n" + page.ToString();
                }
                using (StreamWriter writer = new StreamWriter(outputFile, false)) //Write all csv's with djvu to output "Merged" csv
                {
                    writer.WriteLine(content);
                }
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.Message, LogDepth.UserLevel);
                LogWriter.LogMessage(e.StackTrace, LogDepth.Debug);
                if (e.InnerException != null)
                {
                    LogWriter.LogMessage(e.InnerException.Message, LogDepth.UserLevel);
                    LogWriter.LogMessage(e.InnerException.StackTrace, LogDepth.Debug);
                    if (e.InnerException.InnerException != null)
                    {
                        LogWriter.LogMessage(e.InnerException.InnerException.Message, LogDepth.UserLevel);
                        LogWriter.LogMessage(e.InnerException.InnerException.StackTrace, LogDepth.Debug);
                    }
                }
                return false;
            }
        }
        public void Endloop()
        {
            cancellationTokenSource.Cancel();
        }

        private CSVDocument AddDJVUToCsv(string file, int requestedDPI, CancellationTokenSource source)
        {
            try
            {
                LogWriter.LogMessage($"Reading file {file}", LogDepth.UserLevel);
                CSVDocument csvDocument = CSVDocument.FromFile(file);
                CSVRow anyRow = csvDocument.Rows[0];
                var pagePaths = csvDocument.Rows.Select(s => s.PathToPDF); //Paths to PDF documents within one csv
                foreach (var pagePath in pagePaths)
                {
                    if (!File.Exists(pagePath))
                    {
                        throw new FileNotFoundException("File not found", file);
                    }
                }
                Regex regex = new Regex("[-][0-9]{4}[.]pdf");
                var outputDJVUPath = regex.Replace(pagePaths.FirstOrDefault(), ".djvu"); //Form djvu name/path from the first PDF name (document-0001.pdf => document.djvu)   
                try
                {
                    PDFToDJVU.Executor.Convert(pagePaths.ToArray(), outputDJVUPath, source, requestedDPI); // Convert pdfs to djvu
                }
                catch (Exception)
                {
                    source.Cancel();                    
                }
                CSVRow DJVUFullDocument = (CSVRow)anyRow.Clone(); //Template for djvu
                DJVUFullDocument.Content["PAGES"] = "0";
                DJVUFullDocument.PathToPDF = outputDJVUPath;
                LogWriter.LogMessage($"Adding a row with DJVUDocument {outputDJVUPath}", LogDepth.UserLevel);
                csvDocument.Add(DJVUFullDocument); //Add DJVU to csv
                return csvDocument;
            }
            catch (FileNotFoundException ex)
            {
                LogWriter.LogMessage($"{ex.Message}: {ex.FileName}", LogDepth.UserLevel);
                throw;
            }
        }
    }
}
