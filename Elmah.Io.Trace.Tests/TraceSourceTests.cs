using System;
using System.Diagnostics;
using Elmah.Io.Client;
using Moq;
using NUnit.Framework;

namespace Elmah.Io.Trace.Tests
{
    public class TraceSourceTests
    {
        Mock<ILogger> _loggerMock;
        string _message;
        string _source;
        TraceSource _traceSource;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _message = Guid.NewGuid().ToString();
            _source = "source";
            _traceSource = new TraceSource(_source, SourceLevels.All);
            _traceSource.Listeners.Clear();
            _traceSource.Listeners.Add(new ElmahIoTraceListener(_loggerMock.Object));
        }

        [Test]
        public void CanTraceInformation()
        {
            _traceSource.TraceInformation(_message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Information && m.Title == _message && m.Source == _source)));
        }

        [Test]
        public void CanTraceInformationWithArgs()
        {
            _traceSource.TraceInformation("Hello {0}", "World");
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Information && m.Title == "Hello World" && m.Source == _source)));
        }

        [Test]
        public void CanTraceData()
        {
            _traceSource.TraceData(TraceEventType.Verbose, 1, _message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Verbose && m.Title == _message && m.Source == _source)));
        }

        [Test]
        public void CanTraceDatas()
        {
            _traceSource.TraceData(TraceEventType.Verbose, 1, _message, _message);
            var expected = string.Format("{0},{0}", _message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Verbose && m.Title == expected && m.Source == _source)));
        }

        [Test]
        public void CanTraceEvent()
        {
            _traceSource.TraceEvent(TraceEventType.Warning, 1, _message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Warning && m.Title == _message && m.Source == _source)));
        }

        [Test]
        public void CanTraceEventWithArgs()
        {
            _traceSource.TraceEvent(TraceEventType.Warning, 1, "Hello {0}", "World");
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Warning && m.Title == "Hello World" && m.Source == _source)));
        }

        [Test]
        public void CanTraceTransfer()
        {
            _traceSource.TraceTransfer(1, _message, Guid.NewGuid());
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Debug && m.Title == _message && m.Source == _source)));
        }
    }
}