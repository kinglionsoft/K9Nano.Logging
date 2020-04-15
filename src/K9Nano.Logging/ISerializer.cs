using Serilog.Events;

namespace K9Nano.Logging
{
    public interface ISerializer
    {
        LogEntity Deserialize(byte[] data);
        byte[] Serialize(LogEntity entity);

        LogEntity Map(LogEvent logEvent);
    }
}
