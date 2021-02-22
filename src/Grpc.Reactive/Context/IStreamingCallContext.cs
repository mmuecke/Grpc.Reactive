using Grpc.Core;
using System.Threading.Tasks;

namespace Grpc.Reactive.Context
{
    /// <summary>
    /// The context for streaming client call.
    /// </summary>
    public interface IStreamingCallContext
    {
        /// <summary>
        /// Asynchronous access to response headers.
        /// </summary>
        Task<Metadata> ResponseHeadersAsync { get; }

        //
        // Summary:
        //     Gets the call status if the call has already finished. Throws InvalidOperationException
        //     otherwise.
        /// <summary>
        /// Gets the call status if the call has already finished. Throws InvalidOperationException otherwise.
        /// </summary>
        Status GetStatus();

        /// <summary>
        /// Gets the call trailing metadata if the call has already finished. Throws InvalidOperationException otherwise.
        /// </summary>
        Metadata GetTrailers();
    }
}