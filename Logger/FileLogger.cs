using System;
using System.IO;

namespace Logger
{
    public class FileLogger : Logger
    {
        StreamWriter latestLogWriter;
        StreamWriter currentLogWriter;
        private bool disposedValue;

        protected override void Log(string message)
        {
            try
            {
                base.Log(message);
                string content = $"Time: {DateTime.Now}\r\nMessage: {message}";
                currentLogWriter.WriteLine(content);
                latestLogWriter.WriteLine(content);
            }
            catch (Exception ex)
            {
                using (FileStream stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Critical Errors", "errors.txt"), FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(ex.Message);
                        writer.WriteLine(ex.StackTrace);
                    }
                }
            }
        }
        public FileLogger(string LogFolderPath) : base()
        {
            try
            {
                if (string.IsNullOrEmpty(LogFolderPath))
                    throw new Exception("Log folder is not initialized");

                string currentLogFile = Path.Combine(LogFolderPath, $"{DateTime.Now:yyyy_MM_dd}.log");
                FileStream currentLogStream = new FileStream(currentLogFile, FileMode.Append, FileAccess.Write);
                currentLogWriter = new StreamWriter(currentLogStream);

                string latestLogFile = Path.Combine(LogFolderPath, "latest.log");
                FileStream latestLogStream = new FileStream(latestLogFile, FileMode.Create, FileAccess.Write);
                latestLogWriter = new StreamWriter(latestLogStream);
            }
            catch (Exception ex)
            {
                using (FileStream stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Critical Errors", "errors.txt"), FileMode.Append))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(ex.Message);
                        writer.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    currentLogWriter.BaseStream.Dispose();
                    currentLogWriter.Dispose();
                    latestLogWriter.BaseStream.Dispose();
                    latestLogWriter.Dispose();
                }
                disposedValue = true;
            }
        }
        public override void SaveLog()
        {
            string latestLogFile = ((FileStream)latestLogWriter.BaseStream).Name;
            string currentLogFile = ((FileStream)latestLogWriter.BaseStream).Name;

            latestLogWriter.BaseStream.Dispose();
            var newLatestStream = new FileStream(latestLogFile, FileMode.Append, FileAccess.Write);
            latestLogWriter.BaseStream.Dispose();
            latestLogWriter = new StreamWriter(newLatestStream);
            
            currentLogWriter.BaseStream.Dispose();
            var newCurrentStream = new FileStream(currentLogFile, FileMode.Append, FileAccess.Write);
            currentLogWriter.Dispose();
            currentLogWriter = new StreamWriter(newCurrentStream);
        }
    }
}
