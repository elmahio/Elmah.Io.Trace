using System;
using System.Diagnostics;
using NUnit.Framework;
using Elmah.Io.Client;
using Moq;

namespace Elmah.Io.Trace.Tests
{
    public class TraceTests
    {
        Mock<ILogger> _loggerMock;
        string _message;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _message = Guid.NewGuid().ToString();
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new ElmahIoTraceListener(_loggerMock.Object));
        }

        [Test]
        public void CanWrite()
        {
            System.Diagnostics.Trace.Write(_message);
            _loggerMock.Verify(x => x.Information(_message));
        }

        [Test]
        public void CanWriteLine()
        {
            System.Diagnostics.Trace.WriteLine(_message);
            _loggerMock.Verify(x => x.Information(_message));
        }

        [Test]
        public void CanFail()
        {
            System.Diagnostics.Trace.Fail(_message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Error && m.Title == _message)));
        }

        [Test]
        public void CanFailWithDetails()
        {
            var detailMessage = Guid.NewGuid().ToString();
            System.Diagnostics.Trace.Fail(_message, detailMessage);
            _loggerMock.Verify(
                x =>
                    x.Log(
                        It.Is<Message>(
                            m => m.Severity == Severity.Error && m.Title == _message && m.Detail == detailMessage)));
        }

        [Test]
        public void CanTaceInformation()
        {
            System.Diagnostics.Trace.TraceInformation(_message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Information && m.Title == _message)));
        }

        [Test]
        public void CanTaceInformationWithArgs()
        {
            System.Diagnostics.Trace.TraceInformation("Hello {0}", "World");
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Information && m.Title == "Hello World")));
        }

        [Test]
        public void CanTraceError()
        {
            System.Diagnostics.Trace.TraceError(_message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Error && m.Title == _message)));
        }

        [Test]
        public void CanTraceErrorWithArgs()
        {
            System.Diagnostics.Trace.TraceError("Hello {0}", "World");
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Error && m.Title == "Hello World")));
        }

        [Test]
        public void CanTraceWarning()
        {
            System.Diagnostics.Trace.TraceWarning(_message);
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Warning && m.Title == _message)));
        }

        [Test]
        public void CanTraceWarningWithArgs()
        {
            System.Diagnostics.Trace.TraceWarning("Hello {0}", "World");
            _loggerMock.Verify(x => x.Log(It.Is<Message>(m => m.Severity == Severity.Warning && m.Title == "Hello World")));
        }
    }
}