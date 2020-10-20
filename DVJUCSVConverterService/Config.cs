using Logger;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace DVJUCSVConverterService
{
    public class Config : ISerializable
    {
        private string _logPath;
        private int _dpi;
        private string _inputPath;
        private string _processedCSVs;
        private int _batchSize;
        private int _timeout;
        private string _outputFolder;
        private bool _takeLess;
        private int _maxParallelsPerBatch;
        private int _maxBatchesParallel;
        private LogDepth _logLevel;
        private string _tempFolder;
        private string _userLogFolder;
        private string _shortestPath;

        public string ShortestPath
        {
            get { return _shortestPath; }
            set { _shortestPath = value; }
        }


        public string UserLogFolder
        {
            get { return _userLogFolder; }
            set { _userLogFolder = value; }
        }


        public string TempFolder
        {
            get { return _tempFolder; }
            set { _tempFolder = value; }
        }

        public LogDepth LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; }
        }


        public int MaxBatchesParallel
        {
            get { return _maxBatchesParallel; }
            set { _maxBatchesParallel = value; }
        }


        public int MaxParallelsPerBatch
        {
            get { return _maxParallelsPerBatch; }
            set { _maxParallelsPerBatch = value; }
        }


        public bool TakeLess
        {
            get { return _takeLess; }
            set { _takeLess = value; }
        }

        public string OutputFolder
        {
            get { return _outputFolder; }
            set { _outputFolder = value; }
        }
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public int BatchSize
        {
            get { return _batchSize; }
            set { _batchSize = value; }
        }
        public string ProcessedCSVs
        {
            get { return _processedCSVs; }
            set { _processedCSVs = value; }
        }
        public string LogPath
        {
            get { return _logPath; }
            set { _logPath = value; }
        }
        public int DPI
        {
            get { return _dpi; }
            set { _dpi = value; }
        }
        public string InputPath
        {
            get { return _inputPath; }
            set { _inputPath = value; }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TempPath", _logPath, typeof(string));
            info.AddValue("OutputPath", _inputPath, typeof(string));
            info.AddValue("DPI", _dpi, typeof(int));
            info.AddValue("ProcessedCSVs", _processedCSVs, typeof(string));
            info.AddValue("BatchSize", _batchSize, typeof(int));
            info.AddValue("MaxParallelsPerBatch", _maxParallelsPerBatch, typeof(int));
            info.AddValue("MaxBatchesParallel", _maxBatchesParallel, typeof(int));
            info.AddValue("Timeout", _timeout, typeof(int));
            info.AddValue("OutputFolder", _outputFolder, typeof(string));
            info.AddValue("TakeLess", _takeLess, typeof(bool));
            info.AddValue("LogLevel", _logLevel, typeof(LogDepth));
            info.AddValue("TempFolder", _tempFolder, typeof(string));
            info.AddValue("UserLogFolder", _userLogFolder, typeof(string));
            info.AddValue("ShortestPath", _shortestPath, typeof(string));
        }
        public Config()
        {

        }

        internal static Config GetDefaultConfig()
        {
            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BTI Technologies");
            string inputPath = Path.Combine(rootPath, "Input");
            string outputPath = Path.Combine(rootPath, "Output");
            string logPath = Path.Combine(rootPath, "Logs");
            return new Config()
            {
                DPI = 150,
                InputPath = inputPath,
                LogPath = logPath,
                ProcessedCSVs = Path.Combine(logPath, "ProcessedCSVs.log"),
                Timeout = 240 * 1000,
                BatchSize = 50,
                OutputFolder = outputPath,
                TakeLess = true,
                MaxBatchesParallel = 5,
                MaxParallelsPerBatch = 5,
                LogLevel = LogDepth.Debug,
                UserLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CSVMerger - Critical errors"),
                ShortestPath = "C:\\1"
            };
        }

    }
}
