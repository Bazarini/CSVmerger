using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    public enum LogDepth
    {
        None = 0,
        CriticalErrors = 1,
        UserLevel = 2,
        Default = 3,
        Debug = 5,
    }
    public static class LogWriter
    {
        static Dictionary<Logger, LogDepth> _loggers;
        public static void LogMessage(string message, LogDepth LogDepth = LogDepth.Default)

        {
            Parallel.ForEach(_loggers, logger =>
            {

                if (LogDepth <= logger.Value)
                    logger.Key.LogMessage(message);
            });
        }
        public static void AddLogger(Logger logger, LogDepth logLevel)
        {
            if (_loggers == null)
                _loggers = new Dictionary<Logger, LogDepth>();
            _loggers.Add(logger, logLevel);
        }
    }
}
