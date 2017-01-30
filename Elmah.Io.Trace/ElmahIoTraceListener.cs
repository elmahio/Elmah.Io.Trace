using Elmah.Io.Client;
using System;
using System.Diagnostics;
using Elmah.Io.Client.Models;

namespace Elmah.Io.Trace
{
    public class ElmahIoTraceListener : TraceListener
    {
        readonly IElmahioAPI _client;
        readonly Guid _logId;

        public ElmahIoTraceListener(IElmahioAPI client)
        {
            _client = client;
        }

        public ElmahIoTraceListener(string apiKey, Guid logId)
        {
            _client = ElmahioAPI.Create(apiKey);
            _logId = logId;
        }

        public override void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _client.Messages.Information(_logId, message);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override void Fail(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _client.Messages.Error(_logId, message);
        }

        public override void Fail(string message, string detailMessage)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _client.Messages.CreateAndNotify(_logId,
                new CreateMessage {Title = message, Detail = detailMessage, Severity = Severity.Error.ToString()});
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
            if (string.IsNullOrWhiteSpace(message)) return;
            _client.Messages.CreateAndNotify(_logId,
                new CreateMessage { Title = message, Source = source, Severity = severity.ToString() });
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
