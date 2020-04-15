using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using K9Nano.Logging;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Netty
{
    public class UdpSink : PeriodicBatchingSink
    {
        private readonly SerilogNettyClientOptions _options;
        private readonly ISerializer _serializer;
        private readonly IChannel _clientChannel;
        private readonly MultithreadEventLoopGroup _group;

        public UdpSink(SerilogNettyClientOptions options, ISerializer serializer)
            : base(options.BatchSize, TimeSpan.FromMilliseconds(options.PeriodMicroseconds))
        {
            _options = options;
            _serializer = serializer;

            _group = new MultithreadEventLoopGroup();

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_group)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("Logging", new LoggingClientHandler());
                }));

            _clientChannel = bootstrap.BindAsync(IPEndPoint.MinPort)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        #region PeriodicBatchingSink Members

        /// <inheritdoc />
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                try
                {
                    var entity = _serializer.Map(logEvent);
                    SelfLog.WriteLine($"Starting to send: {entity}");
                    var data = _serializer.Serialize(entity);
                    var buffer = Unpooled.WrappedBuffer(data);
                    var packet = new DatagramPacket(buffer, _options.RemoteEndPoint);
                    await _clientChannel.WriteAndFlushAsync(packet)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    SelfLog.WriteLine("Failed to send UDP package. {0}", e);
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
#if NET4
                // UdpClient does not implement IDisposable, but calling Close disables the
                // underlying socket and releases all managed and unmanaged resources associated
                // with the instance.
                client?.Close();
#else
                _clientChannel.CloseAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
#endif
            }
        }

        #endregion
    }
}