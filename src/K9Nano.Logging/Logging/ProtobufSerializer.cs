using System;
using System.IO;
using ProtoBuf;

namespace K9Nano.Logging
{
    public class ProtobufSerializer : SerializerBase
    {
        public override LogEntity Deserialize(byte[] data)
        {
            using (var memory = new MemoryStream(data))
            {
                return Serializer.Deserialize<LogEntity>(memory);
            }
        }

        public override byte[] Serialize(LogEntity entity)
        {
            using (var memory = new MemoryStream())
            {
                Serializer.Serialize(memory, entity);
                return memory.ToArray();
            }
        }
    }
}
