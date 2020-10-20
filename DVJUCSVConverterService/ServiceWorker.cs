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
        List<string> _processedFiles;
        List<string> _filesInProcess;
        Config _config;
        FileLogger _logger;
        FileLogger _userLogger;
        CSVOperator _converter;
        public ServiceWorker(CancellationToken token)
        {
            _token = token;
        }
        internal bool Prepare()
        {
            if ((_config = Serializer.DeserializeItem<Config>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""))) == null)
            {
                _config = Config.GetDefaultConfig();
                Serializer.SerializeItem(_config, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "config.config").Replace("file:\\", ""));
                return false;
            }
            Directory.CreateDirectory(_config.InputPath);
            Directory.CreateDirectory(_config.LogPath);
            Directory.CreateDirectory(_config.OutputFolder);
            Directory.CreateDirectory(_config.UserLogFolder);
            _logger = new FileLogger(_config.LogPath);
            _userLogger = new FileLogger(_config.UserLogFolder);
            _filesInProcess = new List<string>();
            LogWriter.AddLogger(_logger, _config.LogLevel);
            LogWriter.AddLogger(_userLogger, LogDepth.UserLevel);
            try
            {
                _processedFiles = Serializer.DeserializeItem<List<string>>(_config.ProcessedCSVs);
                if (_processedFiles == null)
                    _processedFiles = new List<string>();
                _converter = new CSVOperator();
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
            try
            {
                while (!_token.IsCancellationRequested)
                {
                    LogWriter.LogMessage($"Checking files in {_config.InputPath}", LogDepth.UserLevel);
                    List<string> Allfiles = Directory.GetFiles(_config.InputPath, "*.csv").ToList();
                    LogWriter.LogMessage("Found files: " + string.Join(",\r\n", Allfiles), LogDepth.UserLevel);
                    List<string> files = Allfiles.Where(w => !_processedFiles.Contains(w) && !_filesInProcess.Contains(w)).ToList();
                    LogWriter.LogMessage("Files to process: " + string.Join(",\r\n", files), LogDepth.UserLevel);
                    if (files.Count >= _config.BatchSize || (_config.TakeLess && files.Count > 0))
                    {

                        Parallel.ForEach(
                            files
                            .SplitList(_config.BatchSize)
                            .Where(w =>
                            {
                                if (_config.TakeLess)
                                    return true;
                                else
                                    return w.Count <= _config.BatchSize;
                            }), new ParallelOptions()
                            {
                                MaxDegreeOfParallelism = _config.MaxBatchesParallel
                            },
                            batch =>
                        {
                            var batchEndRegistration = _token.Register(_converter.Endloop);

                            LogWriter.LogMessage("Start parallel for batch", LogDepth.Debug);
                            Stopwatch watch = new Stopwatch();
                            watch.Start();
                            if (_converter.AddDJVUToCSV(batch.ToArray(), Path.Combine(_config.OutputFolder, $"{DateTime.Now:dd-MM-yyyy_HHmmss}_{Guid.NewGuid():N}.csv"), maxParallels: _config.MaxParallelsPerBatch, dpi: _config.DPI, _config.ShortestPath))
                            {
                                batchEndRegistration.Dispose();
                                _processedFiles.AddRange(batch);
                                Serializer.SerializeItem(_processedFiles, _config.ProcessedCSVs);
                            }
                            _filesInProcess.RemoveAll(r => batch.Contains(r));
                            watch.Stop();
                            LogWriter.LogMessage($"Batch conversion ended. It took {watch.ElapsedMilliseconds}ms to process {batch.Count} files.", LogDepth.Debug);
                        });
                    }
                    else
                    {
                        LogWriter.LogMessage($"Not enough files: Found {files.Count} from needed {_config.BatchSize}", LogDepth.UserLevel);
                    }
                    int i = 0;

                    while (!_token.IsCancellationRequested && i < _config.Timeout / 5000)
                    {
                        i++;
                        LogWriter.LogMessage("Waiting for new files", LogDepth.UserLevel);
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
