using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Reactive.Context
{
    internal class AsyncUnaryCallContext<TResponse> : IStreamingCallContext
    {
        private readonly AsyncUnaryCall<TResponse> asyncUnaryCall;

        public AsyncUnaryCallContext(AsyncUnaryCall<TResponse> asyncUnaryCall)
        {
            this.asyncUnaryCall = asyncUnaryCall;
        }

        public Task<Metadata> ResponseHeadersAsync => asyncUnaryCall.ResponseHeadersAsync;

        public Status GetStatus() => asyncUnaryCall.GetStatus();

        public Metadata GetTrailers() => asyncUnaryCall.GetTrailers();
    }
}