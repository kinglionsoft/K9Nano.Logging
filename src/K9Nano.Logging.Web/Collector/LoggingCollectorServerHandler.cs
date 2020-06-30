using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using K9Nano.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Debugging;

namespace K9Nano.Logging.Web.Collector
{
    public class LoggingCollectorServerHandler :  SimpleChannelInboundHandler<DatagramPacket>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ILoggingManager _loggingManager;

        public LoggingCollectorServerHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILogger<LoggingCollectorServerHandler>>();
            _loggingManager = serviceProvider.GetService<ILoggingManager>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            SelfLog.WriteLine($"Server Received => {packet}");

            if (!packet.Content.IsReadable() || !packet.Content.HasArray)
            {
                SelfLog.WriteLine("Content of UDP packet is null");
                return;
            }

            try
            {
                var buffer = new byte[packet.Content.ReadableBytes];
                packet.Content.ReadBytes(buffer);
                _loggingManager.Post(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ChannelRead0 Error:" + ex);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("LoggingCollectorServerHandler Exception: " + exception);
        }
    }
}