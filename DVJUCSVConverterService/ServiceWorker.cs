using CsvMerger;
using Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DVJUCSVConverterService
{
    internal class ServiceWorker
    {
        CancellationToken _token;
        List<string> processedFiles;
        List<string> FilesInProcess;
        Config config;
        FileLogger logger;
        CSVOperator converter;
        public ServiceWorker(CancellationToken token)
        {
            _token = token;
        }
        internal bool Prepare()
        {
            if ((config = Serializer.DeserializeItem<Config>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""))) == null)
            {
                config = Config.GetDefaultConfig();
                Serializer.SerializeItem(config, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""));
                return false;
            }
            Directory.CreateDirectory(config.InputPath);
            Directory.CreateDirectory(config.LogPath);
            Directory.CreateDirectory(config.OutputFolder);
            logger = new FileLogger(config.LogPath);
            LogWriter.AddLogger(logger, config.LogLevel);
            try
            {
                processedFiles = Serializer.DeserializeItem<List<string>>(config.ProcessedCSVs);
                if (processedFiles == null)
                    processedFiles = new List<string>();
                converter = new CSVOperator();
                return true;
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.Message, LogDepth.UserLevel);
                LogWriter.LogMessage(e.StackTrace, LogDepth.Debug);
                return false;
            }
        }
        internal void Start()
        {
#if DEBUG
            Thread.Sleep(15000);
#endif
            try
            {
                FilesInProcess = new List<string>();
                while (!_token.IsCancellationRequested)
                {
                    LogWriter.LogMessage($"Checking files in {config.InputPath}", LogDepth.UserLevel);
                    List<string> Allfiles = Directory.GetFiles(config.InputPath, "*.csv").ToList();
                    LogWriter.LogMessage("Found files: " + string.Join(",\r\n", Allfiles), LogDepth.UserLevel);
                    List<string> files = Allfiles.Where(w => !processedFiles.Contains(w) && !FilesInProcess.Contains(w)).ToList();
                    LogWriter.LogMessage("Files to process: " + string.Join(",\r\n", files), LogDepth.UserLevel);
                    if (files.Count >= config.BatchSize)
                    {

                        Parallel.ForEach(
                            files
                            .SplitList(config.BatchSize)
                            .Where(w => config.TakeLess ? w.Count() <= config.BatchSize : w.Count() == config.BatchSize)
                            , new ParallelOptions()
                            {
                                MaxDegreeOfParallelism = config.MaxBatchesParallel
                            },
                            batch =>
                        {
                            var batchEndRegistration = _token.Register(converter.Endloop);

                            LogWriter.LogMessage("Starting parallel for batch", LogDepth.Debug);
                            Stopwatch watch = new Stopwatch();
                            watch.Start();
                            if (converter.AddDJVUToCSV(batch.ToArray(), Path.Combine(config.OutputFolder, $"{DateTime.Now:dd-MM-yyyy_HHmmss}_{Guid.NewGuid():N}.csv"), maxParallels: config.MaxParallelsPerBatch, dpi: config.DPI))
                            {
                                batchEndRegistration.Dispose();                                
                                processedFiles.AddRange(batch);                                
                                Serializer.SerializeItem(processedFiles, config.ProcessedCSVs);
                            }
                            FilesInProcess.RemoveAll(r => batch.Contains(r));
                            watch.Stop();
                            LogWriter.LogMessage($"Batch conversion ended. It took {watch.ElapsedMilliseconds}ms to process {batch.Count} files.", LogDepth.Debug);
                        });
                    }
                    else
                    {
                        LogWriter.LogMessage($"Not enough files: {files.Count} from {config.BatchSize}", LogDepth.UserLevel);
                    }
                    int i = 0;

                    while (!_token.IsCancellationRequested && i < config.Timeout / 5000)
                    {
                        i++;
                        LogWriter.LogMessage("Sleeping", LogDepth.Debug);
                        Thread.Sleep(5000);
                    }

                }
            }
            catch (FileNotFoundException ex)
            {
                LogWriter.LogMessage(ex.Message, LogDepth.UserLevel);
                LogWriter.LogMessage(ex.StackTrace, LogDepth.Debug);
            }
            catch (Exception ex)
            {
                LogWriter.LogMessage(ex.Message, LogDepth.UserLevel);
                LogWriter.LogMessage(ex.StackTrace, LogDepth.Debug);
            }
        }
    }
}
