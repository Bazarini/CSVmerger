using System;
using System.Collections.Generic;
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
            info.AddValue("Timeout", _timeout, typeof(int));
            info.AddValue("OutputFolder", _outputFolder, typeof(string));
        }
        public Config()
        {
                       
        }

        internal static Config GetDefaultConfig()
        {
            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BTI Technologies");
            string inputPath = Path.Combine(rootPath, "Input");
            Directory.CreateDirectory(inputPath);
            string outputPath = Path.Combine(rootPath, "Output");
            Directory.CreateDirectory(outputPath);
            string logPath = Path.Combine(rootPath, "Logs");
            Directory.CreateDirectory(logPath);
            return new Config()
            {
                DPI = 150,
                InputPath = inputPath,
                LogPath = logPath,
                ProcessedCSVs = Path.Combine(logPath, "ProcessedCSVs.log"),
                Timeout = 240 * 1000,
                BatchSize = 50,
                OutputFolder = outputPath                
            };
        }

    }
}
