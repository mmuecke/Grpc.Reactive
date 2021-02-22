using Autofac.Extras.Moq;
using Grpc.Core;
using Grpc.Reactive.Test.Context;
using Grpc.Reactive.Test.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Reactive.Test.Client
{
    public class ClientTest
    {
        public static IEnumerable<object[]> ThrowsArgumentNullException()
        {
            {
                Action action = () => ((AsyncDuplexStreamingCall<MessageMock, MessageMock>)null).OnResponse();
                yield return new object[] { action };
            }
            {
                Action action = () => ((AsyncServerStreamingCall<MessageMock>)null).OnResponse();
                yield return new object[] { action };
            }

            {
                using var mock = AutoMock.GetLoose();
                mock.Mock<IObservable<MessageMock>>();
                var observalbe = mock.Create<IObservable<MessageMock>>();
                Action action = () => ((AsyncDuplexStreamingCall<MessageMock, MessageMock>)null).RequestFrom(observalbe);
                yield return new object[] { action };
            }
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                    data.MockIClientStreamWriter, data.MockIAsyncStreamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                Action action = () => asyncDuplexStreamingCall.RequestFrom(null);
                yield return new object[] { action };
            }

            {
                using var mock = AutoMock.GetLoose();
                mock.Mock<IObservable<MessageMock>>();
                var observalbe = mock.Create<IObservable<MessageMock>>();
                Action action = () => ((AsyncClientStreamingCall<MessageMock, MessageMock>)null).RequestFrom(observalbe);
                yield return new object[] { action };
            }
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncClientStreamingCall<MessageMock, MessageMock>(
                    data.MockIClientStreamWriter, Task.FromResult<MessageMock>(null), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                Action action = () => asyncDuplexStreamingCall.RequestFrom(null);
                yield return new object[] { action };
            }
        }

        [Theory]
        [MemberData(nameof(ThrowsArgumentNullException))]
        public void ArgumentNullException(Action action)
        {
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public async Task OnResponse_AsyncDuplexStreamingCall()
        {
            var data = new CallContextData();
            var streamReader = new AsyncStreamReaderMock(2);
            var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                data.MockIClientStreamWriter, streamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            var result = await asyncDuplexStreamingCall.OnResponse().ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.First().Current);
            Assert.NotNull(result.Last().Current);
            Assert.NotNull(result.First().Context);
            Assert.NotNull(result.Last().Context);
        }

        [Fact]
        public async Task OnResponse_AsyncServerStreamingCall()
        {
            var data = new CallContextData();
            var streamReader = new AsyncStreamReaderMock(2);
            var asyncServerStreamingCall = new AsyncServerStreamingCall<MessageMock>(streamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            var result = await asyncServerStreamingCall.OnResponse().ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.First().Current);
            Assert.NotNull(result.Last().Current);
            Assert.NotNull(result.First().Context);
            Assert.NotNull(result.Last().Context);
        }

        [Fact]
        public async Task RequestFrom_AsyncClientStreamingCall()
        {
            var data = new CallContextData();
            var streamReader = new AsyncStreamReaderMock(2);
            var asyncDuplexStreamingCall = new AsyncClientStreamingCall<MessageMock, MessageMock>(
                data.MockIClientStreamWriter, Task.FromResult(new MessageMock()), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            var result = await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList();

            Assert.Equal(1, result.Count);
            Assert.NotNull(result.Single());
        }

        [Fact]
        public async Task RequestFrom_AsyncClientStreamingCall_Error_Response()
        {
            var data = new CallContextData();

            var streamReader = new AsyncStreamReaderMock(2, Task.FromException<bool>(new Exception()));
            var asyncDuplexStreamingCall = new AsyncClientStreamingCall<MessageMock, MessageMock>(
                data.MockIClientStreamWriter, Task.FromException<MessageMock>(new Exception()), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            await Assert.ThrowsAsync<Exception>(async () => await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList());
        }

        [Fact]
        public async Task RequestFrom_AsyncClientStreamingCall_Error_StreamWriter()
        {
            var data = new CallContextData();

            using var mock = AutoMock.GetLoose();
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.WriteAsync(It.IsAny<MessageMock>())).Returns(Task.FromException<MessageMock>(new Exception()));
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);
            mock.Mock<IAsyncStreamReader<MessageMock>>();
            var MockIClientStreamWriter = mock.Create<IClientStreamWriter<MessageMock>>();
            var MockIAsyncStreamReader = mock.Create<IAsyncStreamReader<MessageMock>>();

            var streamReader = new AsyncStreamReaderMock(2, Task.FromException<bool>(new Exception()));
            var asyncDuplexStreamingCall = new AsyncClientStreamingCall<MessageMock, MessageMock>(
                MockIClientStreamWriter, Task.FromResult(new MessageMock()), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            await Assert.ThrowsAsync<Exception>(async () => await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList());
        }

        [Fact]
        public async Task RequestFrom_AsyncDuplexStreamingCall()
        {
            var data = new CallContextData();
            var streamReader = new AsyncStreamReaderMock(2);
            var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                data.MockIClientStreamWriter, streamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            var result = await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList();

            Assert.Equal(2, result.Count);
            Assert.NotNull(result.First().Current);
            Assert.NotNull(result.Last().Current);
            Assert.NotNull(result.First().Context);
            Assert.NotNull(result.Last().Context);
        }

        [Fact]
        public async Task RequestFrom_AsyncDuplexStreamingCall_Error_StreamReader()
        {
            var data = new CallContextData();
            var streamReader = new AsyncStreamReaderMock(2, Task.FromException<bool>(new Exception()));
            var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                data.MockIClientStreamWriter, streamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            await Assert.ThrowsAsync<Exception>(async () => await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList());
        }

        [Fact]
        public async Task RequestFrom_AsyncDuplexStreamingCall_Error_StreamWriter()
        {
            var data = new CallContextData();

            using var mock = AutoMock.GetLoose();
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.WriteAsync(It.IsAny<MessageMock>())).Returns(Task.FromException<MessageMock>(new Exception()));
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);
            mock.Mock<IAsyncStreamReader<MessageMock>>();
            var MockIClientStreamWriter = mock.Create<IClientStreamWriter<MessageMock>>();
            var MockIAsyncStreamReader = mock.Create<IAsyncStreamReader<MessageMock>>();

            var streamReader = new AsyncStreamReaderMock(2);
            var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                MockIClientStreamWriter, streamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

            await Assert.ThrowsAsync<Exception>(async () => await asyncDuplexStreamingCall.RequestFrom(Observable.Repeat(new MessageMock(), 2)).ToList());
        }
    }
}