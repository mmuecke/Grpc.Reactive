using Grpc.Core;
using Grpc.Reactive.Context;
using Grpc.Reactive.Test.Mocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Reactive.Test.Context
{
    public class ExtensionCallContextTest
    {
        public static IEnumerable<object[]> Data()
        {
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncDuplexStreamingCall<MessageMock, MessageMock>(
                   data.MockIClientStreamWriter, data.MockIAsyncStreamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                yield return new object[] { nameof(AsyncDuplexStreamingCall<MessageMock, MessageMock>), data, asyncDuplexStreamingCall.GetCallContext() };
            }
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncClientStreamingCall<MessageMock, MessageMock>(
                   data.MockIClientStreamWriter, Task.FromResult<MessageMock>(null), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                yield return new object[] { nameof(AsyncClientStreamingCall<MessageMock, MessageMock>), data, asyncDuplexStreamingCall.GetCallContext() };
            }
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncServerStreamingCall<MessageMock>(
                   data.MockIAsyncStreamReader, data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                yield return new object[] { nameof(AsyncServerStreamingCall<MessageMock>), data, asyncDuplexStreamingCall.GetCallContext() };
            }
            {
                var data = new CallContextData();
                var asyncDuplexStreamingCall = new AsyncUnaryCall<MessageMock>(
                   Task.FromResult<MessageMock>(null), data.ResponseHeadersAsync, () => data.StatusData, () => data.MetaData, () => { });

                yield return new object[] { nameof(AsyncUnaryCall<MessageMock>), data, asyncDuplexStreamingCall.GetCallContext() };
            }
        }

        public static IEnumerable<object[]> ThrowsArgumentNullException()
        {
            {
                Action action = () => ((AsyncDuplexStreamingCall<MessageMock, MessageMock>)null).GetCallContext();
                yield return new object[] { action };
            }
            {
                Action action = () => ((AsyncClientStreamingCall<MessageMock, MessageMock>)null).GetCallContext();
                yield return new object[] { action };
            }
            {
                Action action = () => ((AsyncServerStreamingCall<MessageMock>)null).GetCallContext();
                yield return new object[] { action };
            }
            {
                Action action = () => ((AsyncUnaryCall<MessageMock>)null).GetCallContext();
                yield return new object[] { action };
            }
        }

        [Theory]
        [MemberData(nameof(ThrowsArgumentNullException))]
        public void ArgumentNullException(Action action)
        {
            Assert.Throws<ArgumentNullException>(action);
        }

        [Theory]
        [MemberData(nameof(Data))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "<Pending>")]
        public async Task CreateContext(string _, CallContextData data, IStreamingCallContext context)
        {
            Assert.NotNull(context);
            Assert.Equal(data.MetaData, await context.ResponseHeadersAsync);
            Assert.Equal(data.MetaData, context.GetTrailers());
            Assert.Equal(data.StatusData, context.GetStatus());
            Assert.Equal(data.StatusData, context.GetStatus());
        }
    }
}