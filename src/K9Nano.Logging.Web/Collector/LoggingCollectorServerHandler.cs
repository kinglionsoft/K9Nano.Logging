using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Debugging;

namespace K9Nano.Logging.Web.Collector
{
    public class LoggingCollectorServerHandler :  SimpleChannelInboundHandler<DatagramPacket>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        public LoggingCollectorServerHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serializer = serviceProvider.GetService<ISerializer>();
            _logger = serviceProvider.GetService<ILogger<LoggingCollectorServerHandler>>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            SelfLog.WriteLine($"Server Received => {packet}");

            if (!packet.Content.IsReadable() || !packet.Content.HasArray)
            {
                SelfLog.WriteLine("Content of UDP packet is null");
                return;
            }

            var buffer = new byte[packet.Content.ReadableBytes];
            packet.Content.ReadBytes(buffer);
            var logEntity = _serializer.Deserialize(buffer);
            _logger.LogInformation($"Server received: {logEntity}");
            // Broadcast to client 

            // Save
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}