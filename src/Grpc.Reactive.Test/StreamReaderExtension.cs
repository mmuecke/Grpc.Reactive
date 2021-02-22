using Grpc.Core;
using Grpc.Reactive.Test.Mocks;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace Grpc.Reactive.Test
{
    public class StreamReaderExtension
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ReadStream(int count)
        {
            var stream = new AsyncStreamReaderMock(count);
            var reads = await stream.ReadAll().ToList();
            Assert.Equal(count, reads.Count);
        }

        [Fact]
        public async Task ReadStream_Throws()
        {
            var exception = new Exception();
            var stream = new AsyncStreamReaderMock(2, Task.FromException<bool>(exception));
            await Assert.ThrowsAsync<Exception>(async () => await stream.ReadAll().ToList());
        }

        [Fact]
        public void ReadStream_ArguementNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ((IAsyncStreamReader<MessageMock>)null).ReadAll());
        }

        [Fact]
        public void ReadStream_Cancellation()
        {
            var stream = new AsyncStreamReaderMock(1, Observable.Never<bool>().ToTask());

            var readall = stream.ReadAll().Subscribe();
            readall.Dispose();

            Assert.True(stream.CancellationToken.IsCancellationRequested);
        }

        [Fact]
        public async Task ReadStreamTest()
        {
            var stream = new AsyncStreamReaderMock(2);
            Assert.Null(stream.Current);
            Assert.True(await stream.MoveNext());
            Assert.NotNull(stream.Current);
            Assert.False(await stream.MoveNext());
            Assert.NotNull(stream.Current);
            Assert.False(await stream.MoveNext());
            Assert.NotNull(stream.Current);
        }
    }
}