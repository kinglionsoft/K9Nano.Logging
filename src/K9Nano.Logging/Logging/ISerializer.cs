using System;
using Serilog.Events;

namespace K9Nano.Logging
{
    public interface ISerializer
    {
        LogEntity Deserialize(byte[] data);
        byte[] Serialize(LogEntity entity);

        bool TryDeserialize(byte[] data, out LogEntity result);
        LogEntity Map(LogEvent logEvent);
    }
}
