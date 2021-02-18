using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Reactive.Context
{
    internal class AsyncDuplexStreamingCallContext<TRequest, TResponse> : IStreamingCallContext
    {
        private readonly AsyncDuplexStreamingCall<TRequest, TResponse> asyncClientStreamingCall;

        public AsyncDuplexStreamingCallContext(AsyncDuplexStreamingCall<TRequest, TResponse> asyncClientStreamingCall)
        {
            this.asyncClientStreamingCall = asyncClientStreamingCall;
        }

        public Task<Metadata> ResponseHeadersAsync => asyncClientStreamingCall.ResponseHeadersAsync;

        public Status GetStatus() => asyncClientStreamingCall.GetStatus();

        public Metadata GetTrailers() => asyncClientStreamingCall.GetTrailers();
    }
}