using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Reactive.Context
{
    internal class AsyncClientStreamingCallContext<TRequest, TResponse> : IStreamingCallContext
    {
        private readonly AsyncClientStreamingCall<TRequest, TResponse> asyncClientStreamingCall;

        public AsyncClientStreamingCallContext(AsyncClientStreamingCall<TRequest, TResponse> asyncClientStreamingCall)
        {
            this.asyncClientStreamingCall = asyncClientStreamingCall;
        }

        public Task<Metadata> ResponseHeadersAsync => asyncClientStreamingCall.ResponseHeadersAsync;

        public Status GetStatus() => asyncClientStreamingCall.GetStatus();

        public Metadata GetTrailers() => asyncClientStreamingCall.GetTrailers();
    }
}