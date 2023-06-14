using MessagePack;
using System;
using System.Runtime.Serialization;

namespace SimpleIpc
{
    [DataContract]
    public class MessageWrapper
    {
        [SerializationConstructor]
        public MessageWrapper(Type contentType, byte[] content, MessageType messageType, Guid responseTo)
        {
            Id = Guid.NewGuid();
            ContentType = contentType;
            Content = content;
            MessageType = messageType;
            ResponseTo = responseTo;
        }

        public MessageWrapper(Type contentType, object content, MessageType messageType)
        {
            Id = Guid.NewGuid();
            Content = MessagePackSerializer.Serialize(contentType, content);
            ContentType = contentType;
            MessageType = messageType;
        }

        public MessageWrapper(Type contentType, object content, Guid responseTo)
            : this(contentType, content, MessageType.Response)
        {
            ResponseTo = responseTo;
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public byte[] Content { get; set; } = Array.Empty<byte>();

        [DataMember]
        public Type ContentType { get; set; }

        [DataMember]
        public MessageType MessageType { get; set;}

        [DataMember]
        public Guid ResponseTo { get; set; }
    }
}
