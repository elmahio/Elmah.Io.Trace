using System;
using System.Diagnostics;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Moq;
using NUnit.Framework;

namespace Elmah.Io.Trace.Tests
{
    public class TraceSourceTests
    {
        Mock<IElmahioAPI> _clientMock;
        Mock<IMessages> _messagesMock;
        string _message;
        string _source;
        TraceSource _traceSource;

        [SetUp]
        public void SetUp()
        {
            _clientMock = new Mock<IElmahioAPI>();
            _messagesMock = new Mock<IMessages>();
            _clientMock.Setup(x => x.Messages).Returns(_messagesMock.Object);
            _message = Guid.NewGuid().ToString();
            _source = "source";
            _traceSource = new TraceSource(_source, SourceLevels.All);
            _traceSource.Listeners.Clear();
            _traceSource.Listeners.Add(new ElmahIoTraceListener(_clientMock.Object));
        }

        [Test]
        public void CanTraceInformation()
        {
            _traceSource.TraceInformation(_message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Information.ToString() && msg.Title == _message &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceInformationWithArgs()
        {
            _traceSource.TraceInformation("Hello {0}", "World");
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Information.ToString() && msg.Title == "Hello World" &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceData()
        {
            _traceSource.TraceData(TraceEventType.Verbose, 1, _message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Verbose.ToString() && msg.Title == _message &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceDatas()
        {
            _traceSource.TraceData(TraceEventType.Verbose, 1, _message, _message);
            var expected = string.Format("{0},{0}", _message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Verbose.ToString() && msg.Title == expected &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceEvent()
        {
            _traceSource.TraceEvent(TraceEventType.Warning, 1, _message);
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Warning.ToString() && msg.Title == _message &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceEventWithArgs()
        {
            _traceSource.TraceEvent(TraceEventType.Warning, 1, "Hello {0}", "World");
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Warning.ToString() && msg.Title == "Hello World" &&
                                msg.Source == _source)));
        }

        [Test]
        public void CanTraceTransfer()
        {
            _traceSource.TraceTransfer(1, _message, Guid.NewGuid());
            _messagesMock.Verify(
                x =>
                    x.CreateAndNotify(It.IsAny<Guid>(),
                        It.Is<CreateMessage>(
                            msg =>
                                msg.Severity == Severity.Debug.ToString() && msg.Title == _message &&
                                msg.Source == _source)));
        }
    }
}