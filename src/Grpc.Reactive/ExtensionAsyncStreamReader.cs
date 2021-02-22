using Google.Protobuf;
using Grpc.Core;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Grpc.Reactive
{
    /// <summary>
    /// Observable extension for <see cref="IAsyncStreamReader{T}"/>
    /// </summary>
    public static class ExtensionAsyncStreamReader
    {
        /// <summary>
        /// Reads all values from the stream into a observable.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="asyncStreamReader">The stream reader source.</param>
        /// <returns>The observable with the values.</returns>
        public static IObservable<T> ReadAll<T>(this IAsyncStreamReader<T> asyncStreamReader)
            where T : class, IMessage<T>
        {
            if (asyncStreamReader is null)
                throw new ArgumentNullException(nameof(asyncStreamReader));

            return Observable.Using(
               () => new CancellationDisposable(),
               cancellationDisposable => Observable
                    // Scheduler.CurrentThread is required for Repeat().TakeWhile(...)
                    // https://stackoverflow.com/a/58529840
                    .FromAsync(() => asyncStreamReader.MoveNext(cancellationDisposable.Token), Scheduler.Default)
                    .Select(state => new { state, asyncStreamReader.Current })
                    .Repeat()
                    .TakeUntil(read => !read.state)
                    .Select(read => read.Current));
        }
    }
}