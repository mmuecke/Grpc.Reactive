using Grpc.Reactive.Context;
using Grpc.Reactive.Test.Mocks;
using System;
using Xunit;

namespace Grpc.Reactive.Test.Context
{
    public class MessageWithContextTest
    {
        [Fact]
        public void ArgumentNullException_Message()
        {
            var message = new MessageMock();
            Assert.Throws<ArgumentNullException>(() => new MessageWithContext<MessageMock, string>(message, null));
        }

        [Fact]
        public void ArgumentNullException_Cobtext()
        {
            var context = "context";
            Assert.Throws<ArgumentNullException>(() => new MessageWithContext<MessageMock, string>(null, context));
        }

        [Fact]
        public void ContextObject()
        {
            var message = new MessageMock();
            var context = "context";
            var messageWithContext = new MessageWithContext<MessageMock, string>(message, context);

            Assert.Equal(message, messageWithContext.Current);
            Assert.Equal(context, messageWithContext.Context);
        }
    }
}