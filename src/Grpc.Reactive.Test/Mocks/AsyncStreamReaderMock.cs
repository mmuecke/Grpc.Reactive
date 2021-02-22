using Grpc.Core;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Reactive.Test.Mocks
{
    public class AsyncStreamReaderMock : IAsyncStreamReader<MessageMock>
    {
        private readonly MessageMock[] Messages;
        private readonly Task<bool> trueTask;
        private int counter = -1;

        public CancellationToken CancellationToken { get; set; }

        public AsyncStreamReaderMock(int count, Task<bool> trueTask = null)
        {
            Messages = Enumerable.Range(0, count).Select(_ => new MessageMock()).ToArray();
            this.trueTask = trueTask ?? Task.FromResult(true);
        }

        public MessageMock Current => counter < 0 ? null : Messages[counter > Messages.Length - 1 ? Messages.Length - 1 : counter];

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;

            if (++counter < Messages.Length - 1)
                return trueTask;
            else
                return Task.FromResult(false);
        }
    }
}