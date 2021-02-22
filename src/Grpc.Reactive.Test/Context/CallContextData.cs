using Autofac.Extras.Moq;
using Grpc.Core;
using Grpc.Reactive.Test.Mocks;
using Moq;
using System.Threading.Tasks;

namespace Grpc.Reactive.Test.Context
{
    public class CallContextData
    {
        public CallContextData()
        {
            using var mock = AutoMock.GetLoose();
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.WriteAsync(It.IsAny<MessageMock>())).Returns(Task.CompletedTask);
            mock.Mock<IClientStreamWriter<MessageMock>>().Setup(x => x.CompleteAsync()).Returns(Task.CompletedTask);
            mock.Mock<IAsyncStreamReader<MessageMock>>();
            MockIClientStreamWriter = mock.Create<IClientStreamWriter<MessageMock>>();
            MockIAsyncStreamReader = mock.Create<IAsyncStreamReader<MessageMock>>();
            MetaData = new Metadata();
            StatusData = new Status();
            ResponseHeadersAsync = Task.FromResult(MetaData);
        }

        public Metadata MetaData { get; }
        public IAsyncStreamReader<MessageMock> MockIAsyncStreamReader { get; }
        public IClientStreamWriter<MessageMock> MockIClientStreamWriter { get; }
        public Task<Metadata> ResponseHeadersAsync { get; }
        public Status StatusData { get; }
    }
}