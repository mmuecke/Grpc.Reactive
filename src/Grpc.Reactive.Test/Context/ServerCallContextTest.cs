using Grpc.Core;
using Grpc.Reactive.Test.Mocks;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Reactive.Test.Context
{
    public class ServerCallContextTest
    {
        [Fact]
        public async Task AddContext()
        {
            var message = new MessageMock();
            var context = new ServerCallContextMock();
            var messageWithContext = await Observable.Return(message).AddContext(context).FirstAsync();

            Assert.Equal(message, messageWithContext.Current);
            Assert.Equal(context, messageWithContext.Context);
        }

        [Fact]
        public void AddContext_ArgumentNullException_Observable()
        {
            var context = new ServerCallContextMock();

            Assert.Throws<ArgumentNullException>(() => ((IObservable<MessageMock>)null).AddContext(context));
        }

        [Fact]
        public void AddContext_ArgumentNullException_Context()
        {
            var message = new MessageMock();
            var context = new ServerCallContextMock();

            Assert.Throws<ArgumentNullException>(() => Observable.Return(message).AddContext(null));
        }

        public class ServerCallContextMock : ServerCallContext
        {
            protected override AuthContext AuthContextCore => throw new NotImplementedException();
            protected override CancellationToken CancellationTokenCore => throw new NotImplementedException();
            protected override DateTime DeadlineCore => throw new NotImplementedException();
            protected override string HostCore => throw new NotImplementedException();
            protected override string MethodCore => throw new NotImplementedException();
            protected override string PeerCore => throw new NotImplementedException();
            protected override Metadata RequestHeadersCore => throw new NotImplementedException();
            protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

            protected override Status StatusCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            protected override WriteOptions WriteOptionsCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions options)
            {
                throw new NotImplementedException();
            }

            protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
            {
                throw new NotImplementedException();
            }
        }
    }
}