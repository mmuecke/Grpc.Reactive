using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Grpc.Reactive.Test.Mocks
{
    public class MessageMock : IMessage<MessageMock>
    {
        public MessageDescriptor Descriptor => throw new NotImplementedException();

        public int CalculateSize()
        {
            throw new NotImplementedException();
        }

        public MessageMock Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals([AllowNull] MessageMock other)
        {
            return true;
        }

        public void MergeFrom(MessageMock message)
        {
            throw new NotImplementedException();
        }

        public void MergeFrom(CodedInputStream input)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(CodedOutputStream output)
        {
            throw new NotImplementedException();
        }
    }
}