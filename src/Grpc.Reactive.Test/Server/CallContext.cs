using Autofac.Core;
using Autofac.Extras.Moq;
using Google.Protobuf;
using Grpc.Core;
using Moq;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Reactive.Test.Server
{

    public class CallContext
    {
        [Fact]
        public void Test1()
        {
            using var mock = AutoMock.GetLoose();
            mock.Mock<IClientStreamWriter<IMessage<MessageMock>>>();
            mock.Mock<IAsyncStreamReader<IMessage<MessageMock>>>();
            var mockIClientStreamWriter =  mock.Create<IClientStreamWriter<IMessage<MessageMock>>>();
            var mockIAsyncStreamReader =  mock.Create<IAsyncStreamReader<IMessage<MessageMock>>>();
            Task<Metadata> responseHeadersAsync = Task.FromResult(new Metadata());
            Func<Status> getStatusFunc = () => new Status();
            Func<Metadata> getTrailersFunc = () => new Metadata();
            Action disposeAction = () => { };

            var m = new AsyncDuplexStreamingCall<IMessage<MessageMock>, IMessage<MessageMock>>(
                mockIClientStreamWriter, mockIAsyncStreamReader, responseHeadersAsync, getStatusFunc, getTrailersFunc, disposeAction);
                    }
    }
}
