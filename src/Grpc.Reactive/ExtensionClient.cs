using Google.Protobuf;
using Grpc.Core;
using Grpc.Reactive.Context;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Grpc.Reactive
{
    /// <summary>
    /// Extensions for a gRPC client.
    /// </summary>
    public static class ExtensionClient
    {
        /// <summary>
        /// Get the context of the call.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncDuplexStreamingCall">The context source.</param>
        /// <returns>The call context.</returns>
        public static IStreamingCallContext GetCallContext<TRequest, TResponse>(
            this AsyncDuplexStreamingCall<TRequest, TResponse> asyncDuplexStreamingCall)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncDuplexStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncDuplexStreamingCall));

            return new AsyncDuplexStreamingCallContext<TRequest, TResponse>(asyncDuplexStreamingCall);
        }

        /// <summary>
        /// Get the context of the call.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncClientStreamingCall">The context source.</param>
        /// <returns>The call context.</returns>
        public static IStreamingCallContext GetCallContext<TRequest, TResponse>(
            this AsyncClientStreamingCall<TRequest, TResponse> asyncClientStreamingCall)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncClientStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncClientStreamingCall));

            return new AsyncClientStreamingCallContext<TRequest, TResponse>(asyncClientStreamingCall);
        }

        /// <summary>
        /// Get the context of the call.
        /// </summary>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncServerStreamingCall">The context source.</param>
        /// <returns>The call context.</returns>
        public static IStreamingCallContext GetCallContext<TResponse>(this AsyncServerStreamingCall<TResponse> asyncServerStreamingCall)
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncServerStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncServerStreamingCall));

            return new AsyncServerStreamingCallContext<TResponse>(asyncServerStreamingCall);
        }

        /// <summary>
        /// Get the context of the call.
        /// </summary>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncUnaryCall">The context source.</param>
        /// <returns>The call context.</returns>
        public static IStreamingCallContext GetCallContext<TResponse>(this AsyncUnaryCall<TResponse> asyncUnaryCall)
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncUnaryCall is null)
                throw new ArgumentNullException(nameof(asyncUnaryCall));

            return new AsyncUnaryCallContext<TResponse>(asyncUnaryCall);
        }

        /// <summary>
        /// Observe the responses of the request.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncDuplexStreamingCall">The call to receive data on.</param>
        /// <returns>The responses stream as observer.</returns>
        public static IObservable<MessageWithContext<TResponse, IStreamingCallContext>> OnResponse<TRequest, TResponse>(
            this AsyncDuplexStreamingCall<TRequest, TResponse> asyncDuplexStreamingCall)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncDuplexStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncDuplexStreamingCall));

            return asyncDuplexStreamingCall.ResponseStream.OnResponse(asyncDuplexStreamingCall.GetCallContext(), asyncDuplexStreamingCall);
        }

        /// <summary>
        /// Observe the responses of the request.
        /// </summary>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncServerStreamingCall">The call to receive data on.</param>
        /// <returns>The responses stream as observer.</returns>
        public static IObservable<MessageWithContext<TResponse, IStreamingCallContext>> OnResponse<TResponse>(
            this AsyncServerStreamingCall<TResponse> asyncServerStreamingCall)
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncServerStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncServerStreamingCall));

            return asyncServerStreamingCall.ResponseStream.OnResponse(asyncServerStreamingCall.GetCallContext(), asyncServerStreamingCall);
        }

        /// <summary>
        /// Send request stream from a observable source and receive the response stream.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncDuplexStreamingCall">The call to send data on.</param>
        /// <param name="observable">The data source.</param>
        /// <returns>The observable for the response stream.</returns>
        public static IObservable<MessageWithContext<TResponse, IStreamingCallContext>> RequestFrom<TRequest, TResponse>(
            this AsyncDuplexStreamingCall<TRequest, TResponse> asyncDuplexStreamingCall, IObservable<TRequest> observable)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncDuplexStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncDuplexStreamingCall));

            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            return Observable.Defer(() => observable
                .WriteTo(asyncDuplexStreamingCall.RequestStream)
                .IgnoreElementsAndCast<MessageWithContext<TResponse, IStreamingCallContext>>()
                .Merge(asyncDuplexStreamingCall.OnResponse()));
        }

        /// <summary>
        /// Send request stream from a observable source and receive the single response.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncClientStreamingCall">The call to receive data on.</param>
        /// <param name="observable">The data source.</param>
        /// <returns>The observable for the single response.</returns>
        public static IObservable<TResponse> RequestFrom<TRequest, TResponse>(
            this AsyncClientStreamingCall<TRequest, TResponse> asyncClientStreamingCall, IObservable<TRequest> observable)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncClientStreamingCall is null)
                throw new ArgumentNullException(nameof(asyncClientStreamingCall));

            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            return Observable.Defer(() => observable
                .WriteTo(asyncClientStreamingCall.RequestStream)
                .IgnoreElementsAndCast<TResponse>()
                .Merge(asyncClientStreamingCall.ResponseAsync.ToObservable()));
        }

        private static IObservable<T> IgnoreElementsAndCast<T>(this IObservable<Unit> observable)
        {
            return Observable.Create<T>(observer => observable.Subscribe(_ => { }, ex => observer.OnError(ex), () => observer.OnCompleted()));
        }

        /// <summary>
        /// Observe the responses of the request.
        /// </summary>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="asyncStreamReader">The source stream reader.</param>
        /// <param name="streamingCallContext">The call context.</param>
        /// <param name="callCleanUp">The call clean up.</param>
        /// <returns>The response observer.</returns>
        private static IObservable<MessageWithContext<TResponse, IStreamingCallContext>> OnResponse<TResponse>(
            this IAsyncStreamReader<TResponse> asyncStreamReader, IStreamingCallContext streamingCallContext, IDisposable callCleanUp)
            where TResponse : class, IMessage<TResponse>
        {
            return Observable.Using(
                () => callCleanUp,
                _ => asyncStreamReader
                    .ReadAll()
                    .Select(response => new MessageWithContext<TResponse, IStreamingCallContext>(response, streamingCallContext)));
        }

        /// <summary>
        /// Write the requests to the stream form the observable.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <param name="observable">The request source.</param>
        /// <param name="clientStreamWriter">The client to write the requests to.</param>
        /// <returns>A observable that completes when all requests are send.</returns>
        private static IObservable<Unit> WriteTo<TRequest>(this IObservable<TRequest> observable, IClientStreamWriter<TRequest> clientStreamWriter)
            where TRequest : class, IMessage<TRequest>
        {
            return observable
                .Select(request => Observable.FromAsync(() => clientStreamWriter.WriteAsync(request)))
                .Concat()
                .Catch<Unit, Exception>(ex => CleanUp().Concat(Observable.Throw<Unit>(ex)))
                .Concat(CleanUp());

            IObservable<Unit> CleanUp() => clientStreamWriter.CompleteAsync().ToObservable();
        }
    }
}