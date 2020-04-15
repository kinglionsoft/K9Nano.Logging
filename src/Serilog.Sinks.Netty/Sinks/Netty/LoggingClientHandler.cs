using System;
using System.Text;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Serilog.Debugging;

namespace Serilog.Sinks.Netty
{
    public class LoggingClientHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            SelfLog.WriteLine($"Logging client received => {packet}");
            if (!packet.Content.IsReadable())
            {
                return;
            }

            var message = packet.Content.ToString(Encoding.UTF8);
            SelfLog.WriteLine($"Logging client  received: {message}");
            ctx.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SelfLog.WriteLine("Logging client exception: " + exception);
            context.CloseAsync();
        }
    }
}