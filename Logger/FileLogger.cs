using System;
using System.IO;

namespace Logger
{
    public class FileLogger : Logger
    {
        string currentLogFile;
        string latestLogFile;
        private bool IslatestCreated;

        protected override void Log(string message)
        {
            try
            {
                base.Log(message);
                string content = $"Time: {DateTime.Now}\r\nMessage: {message}";
                FileStream latestLogStream = new FileStream(latestLogFile, IslatestCreated ? FileMode.Append : FileMode.Create, FileAccess.Write);
                using (StreamWriter latestLogWriter = new StreamWriter(latestLogStream))
                {
                    latestLogWriter.WriteLine(content);
                }
                IslatestCreated = true;
                FileStream currentLogStream = new FileStream(currentLogFile, FileMode.Append, FileAccess.Write);
                using (StreamWriter currentLogWriter = new StreamWriter(currentLogStream))
                {
                    currentLogWriter.WriteLine(content);
                }
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
                IslatestCreated = false;
                currentLogFile = Path.Combine(LogFolderPath, $"{DateTime.Now:yyyy_MM_dd}.log");
                latestLogFile = Path.Combine(LogFolderPath, "latest.log");

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
    }
}
