using System;
using K9Nano.Logging;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Netty;

namespace Serilog
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration Netty(
            this LoggerSinkConfiguration sinkConfiguration,
            SerilogNettyClientOptions options,
            ISerializer serializer,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));

            if (string.IsNullOrWhiteSpace(options.Remote))
            {
                SelfLog.WriteLine("Remote can not be null");
            }

            if (options.RemoteEndPoint == null)
            {
                return sinkConfiguration.Sink(new NullSink(), LevelAlias.Maximum, null);
            }

            try
            {
                var sink = new UdpSink(options, serializer);
                return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Unable to create UDP sink: {0}", e);
                return sinkConfiguration.Sink(new NullSink(), LevelAlias.Maximum, null);
            }
        }

        public static LoggerConfiguration Netty(
            this LoggerSinkConfiguration sinkConfiguration,
            SerilogNettyClientOptions options)
        {
            return Netty(sinkConfiguration, options, new ProtobufSerializer());
        }
    }
}