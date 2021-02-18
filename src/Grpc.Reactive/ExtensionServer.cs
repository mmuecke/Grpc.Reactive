using Google.Protobuf;
using Grpc.Core;
using Grpc.Reactive.Context;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Grpc.Reactive
{
    /// <summary>
    /// Extensions for a gRPC server.
    /// </summary>
    public static class ExtensionServer
    {
        /// <summary>
        /// Add a rpc context form the observable.
        /// </summary>
        /// <typeparam name="T">The type of message data.</typeparam>
        /// <param name="observable">The source observable  without context.</param>
        /// <param name="context">The context for the server call.</param>
        /// <returns>The observable with context.</returns>
        public static IObservable<MessageWithContext<T, ServerCallContext>> AddContext<T>(this IObservable<T> observable, ServerCallContext context)
            where T : class, IMessage<T>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            return observable.Select(withoutContext => new MessageWithContext<T, ServerCallContext>(withoutContext, context));
        }

        /// <summary>
        /// Transforms the stream  of request data in <paramref name="asyncStreamReader"/> into a observable of data with the request context.
        /// </summary>
        /// <remarks>
        /// In case the <see cref="ServerCallContext.CancellationToken"/> of the <paramref name="context"/> is canceled the observable completes.
        /// </remarks>
        /// <typeparam name="T">Type of data reed from the <paramref name="asyncStreamReader"/>.</typeparam>
        /// <param name="asyncStreamReader">The source data stream.</param>
        /// <param name="context">The context of the rpc call.</param>
        /// <returns>A observable of the received request data.</returns>
        public static IObservable<MessageWithContext<T, ServerCallContext>> OnRequest<T>(
            this IAsyncStreamReader<T> asyncStreamReader, ServerCallContext context)
            where T : class, IMessage<T>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            return asyncStreamReader.ReadAll().AddContext(context);
        }

        /// <summary>
        /// Processes the request stream to a output stream by a selector function.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// In case of a <see cref="Exception"/> in the <paramref name="selector"/> the <paramref name="context"/> is set to a error <see cref="StatusCode"/>,
        /// if the status is set to <see cref="StatusCode.OK"/>.
        /// </item>
        /// <item>
        /// In case the <see cref="ServerCallContext.CancellationToken"/> of the <paramref name="context"/> is canceled the processing is canceled.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="asyncStreamReader">The source data stream.</param>
        /// <param name="serverStreamWriter">The target steam writer to write data to.</param>
        /// <param name="context">The context of the rpc call.</param>
        /// <param name="selector">The result selector function.</param>
        /// <returns>The process task.</returns>
        public static Task ProcessRequest<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IAsyncStreamReader{TRequest}, IServerStreamWriter{TResponse}, ServerCallContext, Func{TRequest, TResponse})"/>
        public static Task ProcessRequest<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, ServerCallContext, int, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IAsyncStreamReader{TRequest}, IServerStreamWriter{TResponse}, ServerCallContext, Func{TRequest, TResponse})"/>
        public static Task ProcessRequest<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, ServerCallContext, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <summary>
        /// Process a stream of data in a rpc call.
        /// </summary>
        /// <remarks>
        /// The first <see cref="Exception"/> in a selector call is catch and forwarded to the subscriber.
        /// After the first exception no more data is processed by the <paramref name="selector"/>.
        /// </remarks>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="observable">The source of observable.</param>
        /// <param name="selector">The result selector function.</param>
        /// <returns>A observable for the processing results.</returns>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));
            return observable
                .Select(request => request.DeferSelectorRun(selector))
                .Concat();
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IObservable{MessageWithContext{TRequest, ServerCallContext}}, Func{TRequest, TResponse})"/>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, ServerCallContext, int, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return observable
                .Select((request, index) => request.DeferSelectorRun(r => selector(r, request.Context, index)))
                .Concat();
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IObservable{MessageWithContext{TRequest, ServerCallContext}}, Func{TRequest, TResponse})"/>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, ServerCallContext, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return observable
                .Select(request => request.DeferSelectorRun(r => selector(r, request.Context)))
                .Concat();
        }

        /// <summary>
        /// Processes the request stream to a output stream by a async selector function.
        /// </summary>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// In case of a <see cref="Exception"/> in the <paramref name="selector"/> the <paramref name="context"/> is set to a error <see cref="StatusCode"/>,
        /// if the status is set to <see cref="StatusCode.OK"/>.
        /// </item>
        /// <item>
        /// In case the <see cref="ServerCallContext.CancellationToken"/> of the <paramref name="context"/> is canceled the processing is canceled.
        /// </item>
        /// <item>
        /// Each item is processed sequential.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="asyncStreamReader">The source data stream.</param>
        /// <param name="serverStreamWriter">The target steam writer to write data to.</param>
        /// <param name="context">The context of the rpc call.</param>
        /// <param name="selector">The async result selector function.</param>
        public static Task ProcessRequestAsync<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, Task<TResponse>> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <inheritdoc cref="ProcessRequestAsync{TRequest, TResponse}(IAsyncStreamReader{TRequest}, IServerStreamWriter{TResponse}, ServerCallContext, Func{TRequest, Task{TResponse}})"/>
        public static Task ProcessRequestAsync<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, ServerCallContext, int, Task<TResponse>> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <inheritdoc cref="ProcessRequestAsync{TRequest, TResponse}(IAsyncStreamReader{TRequest}, IServerStreamWriter{TResponse}, ServerCallContext, Func{TRequest, Task{TResponse}})"/>
        public static Task ProcessRequestAsync<TRequest, TResponse>(this IAsyncStreamReader<TRequest> asyncStreamReader,
            IServerStreamWriter<TResponse> serverStreamWriter, ServerCallContext context, Func<TRequest, ServerCallContext, Task<TResponse>> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return asyncStreamReader
                 .OnRequest(context)
                 .ProcessRequest(selector)
                 .WriteResponseTo(serverStreamWriter, context);
        }

        /// <summary>
        /// Process a stream of data in a rpc call async.
        /// </summary>
        /// <remarks>
        /// The first <see cref="Exception"/> in a selector call is catch and forwarded to the subscriber.
        /// After the first exception no more data is processed by the <paramref name="selector"/>.
        /// </remarks>
        /// <typeparam name="TRequest">The type of request data.</typeparam>
        /// <typeparam name="TResponse">The type of response data.</typeparam>
        /// <param name="observable">The source of observable.</param>
        /// <param name="selector">The async result selector function.</param>
        /// <returns>A observable for the processing results.</returns>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, Task<TResponse>> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return observable
                .Select(request => request.DeferAasyncSelectorRun(selector))
                .Concat();
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IObservable{MessageWithContext{TRequest, ServerCallContext}}, Func{TRequest, Task{TResponse}})"/>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, ServerCallContext, int, Task<TResponse>> selector)
        where TRequest : class, IMessage<TRequest>
        where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return observable
                .Select((request, index) => request.DeferAasyncSelectorRun(r => selector(r, request.Context, index)))
                .Concat();
        }

        /// <inheritdoc cref="ProcessRequest{TRequest, TResponse}(IObservable{MessageWithContext{TRequest, ServerCallContext}}, Func{TRequest, Task{TResponse}})"/>
        public static IObservable<MessageWithContext<TResponse, ServerCallContext>> ProcessRequest<TRequest, TResponse>(
            this IObservable<MessageWithContext<TRequest, ServerCallContext>> observable, Func<TRequest, ServerCallContext, Task<TResponse>> selector)
        where TRequest : class, IMessage<TRequest>
        where TResponse : class, IMessage<TResponse>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return observable
                .Select(request => request.DeferAasyncSelectorRun(r => selector(r, request.Context)))
                .Concat();
        }

        /// <summary>
        /// Removes the rpc context form the observable.
        /// </summary>
        /// <typeparam name="T">The type of message data.</typeparam>
        /// <param name="observable">The source observable with context.</param>
        /// <returns>The observable without context.</returns>
        public static IObservable<T> RemoveContext<T>(this IObservable<MessageWithContext<T, ServerCallContext>> observable)
            where T : class, IMessage<T>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            return observable.Select(withContext => withContext.Current);
        }

        /// <inheritdoc cref="WriteResponseTo{T}(IObservable{T}, IServerStreamWriter{T}, ServerCallContext)"/>
        public static Task WriteResponseTo<T>(this IObservable<MessageWithContext<T, ServerCallContext>> observable, IServerStreamWriter<T> serverStreamWriter, ServerCallContext context)
            where T : class, IMessage<T>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            return observable.RemoveContext().WriteResponseTo(serverStreamWriter, context);
        }

        /// <summary>
        /// Writes the stream of observed <see cref="IMessage{T}"/>s responses of a request to the <paramref name="serverStreamWriter"/>.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// In case of a <see cref="Exception"/> in the <paramref name="observable"/> the <paramref name="context"/> is set to a error <see cref="StatusCode"/>,
        /// if the status is set to <see cref="StatusCode.OK"/>.
        /// </item>
        /// <item>
        /// In case the <see cref="ServerCallContext.CancellationToken"/> of the <paramref name="context"/> is canceled the observer is unsubscribed.
        /// </item>
        /// </list>
        /// </remarks>
        /// <typeparam name="T">Type of data to write to the <paramref name="serverStreamWriter"/>.</typeparam>
        /// <param name="observable">The observable of data to write to the <paramref name="serverStreamWriter"/></param>
        /// <param name="serverStreamWriter">The target steam writer to write data to.</param>
        /// <param name="context">The context of the rpc call.</param>
        /// <returns>A task running over the live time of the request.</returns>
        public static Task WriteResponseTo<T>(this IObservable<T> observable, IServerStreamWriter<T> serverStreamWriter, ServerCallContext context)
            where T : class, IMessage<T>
        {
            if (observable is null)
                throw new ArgumentNullException(nameof(observable));

            if (serverStreamWriter is null)
                throw new ArgumentNullException(nameof(serverStreamWriter));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            return observable
                .Select(response => Observable.Defer(() => serverStreamWriter.WriteAsync(response).ToObservable()))
                .Concat()
                .Catch<Unit, Exception>(ex =>
                {
                    if (context.Status.StatusCode == StatusCode.OK)
                    {
                        context.Status = new Status(GetCode(), ex.Message);
                    }
                    return Observable.Empty<Unit>();

                    StatusCode GetCode()
                    {
                        if (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException)
                            return StatusCode.OutOfRange;
                        if (ex is ArgumentNullException || ex is ArgumentException)
                            return StatusCode.InvalidArgument;
                        if (ex is NotImplementedException)
                            return StatusCode.Unimplemented;
                        else
                            return StatusCode.Unknown;
                    }
                })
                .ToTask(context.CancellationToken);
        }

        private static IObservable<MessageWithContext<TResponse, ServerCallContext>> DeferAasyncSelectorRun<TRequest, TResponse>(
            this MessageWithContext<TRequest, ServerCallContext> input, Func<TRequest, Task<TResponse>> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return Observable.Defer(() => selector(input.Current).ToObservable().Select(reslut => new MessageWithContext<TResponse, ServerCallContext>(reslut, input.Context)));
        }

        private static IObservable<MessageWithContext<TResponse, ServerCallContext>> DeferSelectorRun<TRequest, TResponse>(
            this MessageWithContext<TRequest, ServerCallContext> input, Func<TRequest, TResponse> selector)
            where TRequest : class, IMessage<TRequest>
            where TResponse : class, IMessage<TResponse>
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            return Observable.Defer(() =>
            {
                try
                {
                    return Observable.Return(new MessageWithContext<TResponse, ServerCallContext>(selector(input.Current), input.Context));
                }
                catch (Exception ex)
                {
                    return Observable.Throw<MessageWithContext<TResponse, ServerCallContext>>(ex);
                }
            });
        }
    }
}