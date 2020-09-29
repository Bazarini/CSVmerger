using System;

namespace Logger
{
    internal class LogEvent
    {
        internal DateTime EventDate { get; private set; }
        internal string Message { get; private set; }

        internal LogEvent(string message)
        {
            EventDate = DateTime.Now;
            Message = message;
        }
    }
}
