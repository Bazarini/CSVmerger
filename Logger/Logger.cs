using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Logger
{
    public class Logger
    {
        private object locker = new object();
        private List<LogEvent> _sessionEvents;
        public List<string> SessionEvents
        {
            get
            {
                if (_sessionEvents == null) _sessionEvents = new List<LogEvent>();
                return _sessionEvents.Select(s => s.Message).ToList();
            }
            private set
            {
                List<LogEvent> temp = new List<LogEvent>();
                foreach (var item in value)
                    temp.Add(new LogEvent(item));
                _sessionEvents = new List<LogEvent>(temp);
            }
        }
        protected virtual void Log(string message)
        {
            _sessionEvents.Add(new LogEvent(message));
        }
        private void LogObj(object obj)
        {
            lock (locker)
            {
                Log((string)obj);
            }
        }
        internal void LogMessage(string message)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(LogObj));
            thread.Start(message);
        }   
        public Logger()
        {
            _sessionEvents = new List<LogEvent>();
        }

    }
}
