using Google.Protobuf;
using Grpc.Core;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Grpc.Reactive
{
    internal static class ExtensionAsyncStreamReader
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
            return Observable.Create<T>(async observer =>
            {
                var cancellationDisposable = new CancellationDisposable();

                try
                {
                    while (await asyncStreamReader.MoveNext(cancellationDisposable.Token).ConfigureAwait(false))
                        observer.OnNext(asyncStreamReader.Current);

                    if (!cancellationDisposable.IsDisposed)
                        observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                return cancellationDisposable;
            });
        }
    }
}