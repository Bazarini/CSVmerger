using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    public static class LogWriter
    {
        static Logger _logger;
        public static void LogMessage(string message)
        {
            if (_logger == null)
                _logger = new Logger();
            _logger.LogMessage(message);

        }
        public static void InitializeLogger(Logger logger)
        {
            _logger = logger;
            LogMessage("Logger started");
        }
        public static void Save()
        {
            _logger.SaveLog();
            Thread.Sleep(5000);

        }
        public static void Dispose()
        {
            _logger.Dispose();
        }
    }
}
