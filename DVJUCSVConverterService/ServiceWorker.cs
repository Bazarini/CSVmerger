using CsvMerger;
using Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DVJUCSVConverterService
{
    internal class ServiceWorker
    {
        CancellationToken _token;
        List<string> processedFiles;
        Config config;
        FileLogger logger;
        CSVOperator converter;
        public ServiceWorker(CancellationToken token)
        {
            _token = token;
        }
        internal void Prepare()
        {
            if ((config = Serializer.DeserializeItem<Config>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""))) == null)
            {
                config = Config.GetDefaultConfig();
                Serializer.SerializeItem(config, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""));
            }
            logger = new FileLogger(config.LogPath);
            LogWriter.InitializeLogger(logger);
            try
            {
                processedFiles = Serializer.DeserializeItem<List<string>>(config.ProcessedCSVs);
                if (processedFiles == null)
                    processedFiles = new List<string>();
                converter = new CSVOperator();
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.Message);
                LogWriter.LogMessage(e.StackTrace);
            }
        }
        internal void Start()
        {
            try
            {                
                while (!_token.IsCancellationRequested)
                {
                    LogWriter.LogMessage($"Checking files in {config.InputPath}");
                    List<string> Allfiles = Directory.GetFiles(config.InputPath, "*.csv").ToList();
                    LogWriter.LogMessage("Found files: " + string.Join(",\r\n", Allfiles));
                    List<string> files = Allfiles.Where(w => !processedFiles.Contains(w)).ToList();
                    LogWriter.LogMessage("Files to process: " + string.Join(",\r\n", files));
                    if (files.Count >= config.BatchSize)
                    {
                        List<Task> tasks = new List<Task>();                        
                        foreach (List<string> batch in files.SplitList(config.BatchSize).Where(w => w.Count() == config.BatchSize))
                        {
                            Task task = new Task(() => converter.AddDJVUToCSV(batch.ToArray(), Path.Combine(config.OutputFolder, $"{DateTime.Now:dd-MM-yyyy_HHmmss}_{Guid.NewGuid():N}.csv"), config.DPI));
                            tasks.Add(task);
                            task.Start();                            
                            try
                            {
                                task.Wait();                          
                                processedFiles.AddRange(batch);
                                Serializer.SerializeItem(processedFiles, config.ProcessedCSVs);
                            }
                            catch (Exception)
                            {
                                LogWriter.LogMessage(task.Exception.Message + "\r\n" + task.Exception.InnerException.Message + "\r\n" + task.Exception.InnerException.StackTrace);
                            }  
                        }
                        Task.WaitAll(tasks.ToArray());
                    }
                    else
                    {
                        LogWriter.LogMessage($"Not enough files: {files.Count} from {config.BatchSize}");
                    }
                    int i = 0;

                    while (!_token.IsCancellationRequested && i < config.Timeout / 1000)
                    {
                        i++;
                        LogWriter.LogMessage("Sleeping");
                        Thread.Sleep(1000);
                    }

                }
                Stop();
            }
            catch (FileNotFoundException ex)
            {
                LogWriter.LogMessage(ex.Message);
                LogWriter.LogMessage(ex.StackTrace);
            }
            catch (Exception ex)
            {
                LogWriter.LogMessage(ex.Message);
                LogWriter.LogMessage(ex.StackTrace);                
            }
        }
        internal void Stop()
        {
            LogWriter.Dispose();
        }
    }
}
