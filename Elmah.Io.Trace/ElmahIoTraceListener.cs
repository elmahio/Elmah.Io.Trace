using Elmah.Io.Client;
using System;
using System.Diagnostics;

namespace Elmah.Io.Trace
{
    public class ElmahIoTraceListener : TraceListener
    {
        readonly ILogger _logger;

        public ElmahIoTraceListener(ILogger logger)
        {
            _logger = logger;
        }

        public ElmahIoTraceListener(Guid logId)
        {
            _logger = new Logger(logId);
        }

        public override void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _logger.Information(message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override void Fail(string message)
        {
            _logger.Log(new Message(message) { Severity = Severity.Error });
        }

        public override void Fail(string message, string detailMessage)
        {
            _logger.Log(new Message(message) {Detail = detailMessage, Severity = Severity.Error});
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            Trace(data.ToString(), eventType, source);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            Trace(string.Join(",", data), eventType, source);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Trace(message, eventType, source);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            var message = args != null ? string.Format(format, args) : format;
            Trace(message, eventType, source);
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            Trace(message, Severity.Debug, source);
        }

        private void Trace(string message, TraceEventType eventType, string source)
        {
            Trace(message, TraceEventTypeToSeverity(eventType), source);
        }

        private void Trace(string message, Severity severity, string source)
        {
            _logger.Log(new Message(message) { Severity = severity, Source = source });
        }

        private Severity TraceEventTypeToSeverity(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    return Severity.Fatal;
                case TraceEventType.Error:
                    return Severity.Error;
                case TraceEventType.Verbose:
                    return Severity.Verbose;
                case TraceEventType.Warning:
                    return Severity.Warning;
                default:
                    return Severity.Information;
            }
        }
    }
}
