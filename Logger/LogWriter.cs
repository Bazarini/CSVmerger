using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    public static class LogWriter
    {
        static List<Logger> _loggers;
        public static void LogMessage(string message)
        {
            Parallel.ForEach(_loggers, logger => logger.LogMessage(message));
        }
        public static void AddLogger(Logger logger)
        {
            if (_loggers == null)
                _loggers = new List<Logger>();
            _loggers.Add(logger);
        }
    }
}
