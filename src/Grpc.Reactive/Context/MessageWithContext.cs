using Google.Protobuf;
using Grpc.Core;
using System;

namespace Grpc.Reactive.Context
{
    /// <summary>
    /// A <see cref="IMessage{T}"/> with the server rpc context <see cref="ServerCallContext"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of message data.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class MessageWithContext<TMessage, TContext>
        where TMessage : class, IMessage<TMessage>
        where TContext : class
    {
        /// <summary>
        /// Create a <see cref="IMessage{T}"/> with the server rpc context <see cref="ServerCallContext"/>.
        /// </summary>
        /// <param name="current">The current message value.</param>
        /// <param name="context">The server rpc context.</param>
        public MessageWithContext(TMessage current, TContext context)
        {
            Current = current ?? throw new ArgumentNullException(nameof(current));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// The current message value.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// The server rpc context.
        /// </summary>
        public TMessage Current { get; }
    }
}