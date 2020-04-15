using Serilog.Core;
using Serilog.Events;

namespace Serilog.Sinks.Netty
{
    internal class NullSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
        }
    }
}