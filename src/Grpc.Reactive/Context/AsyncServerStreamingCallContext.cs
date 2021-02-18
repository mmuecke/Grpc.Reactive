using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Reactive.Context
{
    internal class AsyncServerStreamingCallContext<TResponse> : IStreamingCallContext
    {
        private readonly AsyncServerStreamingCall<TResponse> asyncServerStreamingCall;

        public AsyncServerStreamingCallContext(AsyncServerStreamingCall<TResponse> asyncServerStreamingCall)
        {
            this.asyncServerStreamingCall = asyncServerStreamingCall;
        }

        public Task<Metadata> ResponseHeadersAsync => asyncServerStreamingCall.ResponseHeadersAsync;

        public Status GetStatus() => asyncServerStreamingCall.GetStatus();

        public Metadata GetTrailers() => asyncServerStreamingCall.GetTrailers();
    }
}