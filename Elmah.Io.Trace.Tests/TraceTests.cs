using System;
using System.Diagnostics;
using NUnit.Framework;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Moq;

namespace Elmah.Io.Trace.Tests
{
    public class TraceTests
    {
        Mock<IElmahioAPI> _clientMock;
        Mock<IMessages> _messagesMock;
        string _message;

        [SetUp]
        public void SetUp()
        {
            _clientMock = new Mock<IElmahioAPI>();
            _messagesMock = new Mock<IMessages>();
            _clientMock.Setup(x => x.Messages).Returns(_messagesMock.Object);
            _message = Guid.NewGuid().ToString();
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Add(new ElmahIoTraceListener(_clientMock.Object));
        }

        [Test]
        public void CanWrite()
        {
            System.Diagnostics.Trace.Write(_message);
            _messagesMock.Verify(x => x.Information(It.IsAny<Guid>(), _message));
        }

        [Test]
        public void CanWriteLine()
        {
            System.Diagnostics.Trace.WriteLine(_message);
            _messagesMock.Verify(x => x.Information(It.IsAny<Guid>(), _message));
        }

        [Test]
        public void CanFail()
        {
            System.Diagnostics.Trace.Fail(_message);
            _messagesMock.Verify(x => x.Error(It.IsAny<Guid>(), _message));
        }

        [Test]
        public void CanFailWithDetails()
        {
            var detailMessage = Guid.NewGuid().ToString();
            System.Diagnostics.Trace.Fail(_message, detailMessage);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Error.ToString() && msg.Title == _message &&
                                msg.Detail == detailMessage)));
        }

        [Test]
        public void CanTaceInformation()
        {
            System.Diagnostics.Trace.TraceInformation(_message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Information.ToString() && msg.Title == _message)));
        }

        [Test]
        public void CanTaceInformationWithArgs()
        {
            System.Diagnostics.Trace.TraceInformation("Hello {0}", "World");
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Information.ToString() && msg.Title == "Hello World")));
        }

        [Test]
        public void CanTraceError()
        {
            System.Diagnostics.Trace.TraceError(_message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Error.ToString() && msg.Title == _message)));
        }

        [Test]
        public void CanTraceErrorWithArgs()
        {
            System.Diagnostics.Trace.TraceError("Hello {0}", "World");
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Error.ToString() && msg.Title == "Hello World")));
        }

        [Test]
        public void CanTraceWarning()
        {
            System.Diagnostics.Trace.TraceWarning(_message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Warning.ToString() && msg.Title == _message)));
        }

        [Test]
        public void CanTraceWarningWithArgs()
        {
            System.Diagnostics.Trace.TraceWarning("Hello {0}", "World");
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Warning.ToString() && msg.Title == "Hello World")));
        }
    }
}