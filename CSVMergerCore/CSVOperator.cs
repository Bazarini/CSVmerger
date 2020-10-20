using System;
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
        public bool AddDJVUToCSV(string[] paths, string outputFile, int maxParallels, int dpi = 250, string shortestPath = "D:\\1")
        {
            List<CSVDocument> mergedCSV = new List<CSVDocument>();
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                var token = cancellationTokenSource.Token;
                Parallel.ForEach(paths, new ParallelOptions() { MaxDegreeOfParallelism = maxParallels, CancellationToken = token }, file =>
                {
                    LogWriter.LogMessage($"Start parallel for {file}", LogDepth.Debug);
                    mergedCSV.Add(AddDJVUToCsv(file, dpi, cancellationTokenSource, shortestPath));
                    LogWriter.LogMessage($"End parallel for {file}", LogDepth.Debug);
                });
                string content = "";
                LogWriter.LogMessage($"Opening file {outputFile} to write.", LogDepth.UserLevel);
                content += mergedCSV[0].HeadersToString();

                foreach (var document in mergedCSV)
                {
                    foreach (CSVRow page in document.Rows.OrderBy(o => int.Parse(o.Content["PAGES"])))
                        content += "\r\n" + page.ToString();
                }
                using (StreamWriter writer = new StreamWriter(outputFile, false))
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

        private CSVDocument AddDJVUToCsv(string file, int requestedDPI, CancellationTokenSource source, string rootFolder)
        {
            try
            {
                LogWriter.LogMessage($"Reading file {file}", LogDepth.UserLevel);
                CSVDocument csvDocument = null;
                try
                {
                    csvDocument = CSVDocument.FromFile(file);
                }
                catch (FileNotFoundException ex)
                {
                    LogWriter.LogMessage(ex.Message);
                }
                CSVRow anyRow = csvDocument.Rows[0];
                var imagesForDJVU = csvDocument.Rows.Select(s =>
                {
                    if (s.Content.ContainsKey("DJVUIMAGES"))
                        return s.Content["DJVUIMAGES"];
                    else if (s.Content.ContainsKey("FILES"))
                        return s.Content["FILES"];
                    else throw new Exception("Columns in CSV are incorrect.");
                });
                var outputDJVUPath = Regex.Replace(csvDocument.Rows.FirstOrDefault().PathToPDF, "[.]pdf", ".djvu");
                try
                {
                    bool singlePDFInput = imagesForDJVU.Distinct().Count() == 1;
                    if (singlePDFInput)
                        PDFToDJVU.Executor.NewPrepareDJVU(imagesForDJVU.FirstOrDefault(), outputDJVUPath, source, requestedDPI);
                    else
                        PDFToDJVU.Executor.PrepareDJVU(imagesForDJVU.ToArray(), outputDJVUPath, source, rootFolder, requestedDPI);
                }
                catch (Exception)
                {
                    source.Cancel();
                    throw;
                }
                CSVRow DJVUFullDocument = (CSVRow)anyRow.Clone();
                DJVUFullDocument.Content["PAGES"] = "0";
                DJVUFullDocument.PathToPDF = outputDJVUPath;
                LogWriter.LogMessage($"Adding a row with DJVUDocument {outputDJVUPath}", LogDepth.Debug);
                csvDocument.Add(DJVUFullDocument);
                return csvDocument;
            }
            catch (Exception ex)
            {
                LogWriter.LogMessage(ex.Message, LogDepth.Debug);
                throw;
            }
        }
    }
}
