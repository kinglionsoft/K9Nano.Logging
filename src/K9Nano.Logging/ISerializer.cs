using System;
using Serilog.Events;

namespace LogCenter.Abstractions
{
    public interface ISerializer
    {
        LogEvent Deserialize(byte[] data);
        byte[] Serialize(LogEvent logEvent);
    }
}
