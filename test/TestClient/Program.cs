using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace TestClient
{
    class Program
    {
        static async Task RunClientAsync()
        {
            var group = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<SocketDatagramChannel>()
                    .Option(ChannelOption.SoBroadcast, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast("Logging", new LoggingClientHandler());
                    }));

                IChannel clientChannel = await bootstrap.BindAsync(IPEndPoint.MinPort);

                Console.WriteLine("Sending");

                byte[] bytes = Encoding.UTF8.GetBytes("Hello");
                IByteBuffer buffer = Unpooled.WrappedBuffer(bytes);
                var ipaddrList = await Dns.GetHostAddressesAsync("localhost");

                var remoteIp = ipaddrList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                if (remoteIp == null)
                {
                   throw new Exception($"Can not resolve ip of ");
                }
                await clientChannel.WriteAndFlushAsync(
                    new DatagramPacket(
                        buffer,
                        new IPEndPoint(remoteIp, 6253)));

                Console.WriteLine("Waiting for response.");

                await Task.Delay(5000);
                Console.WriteLine("Waiting for response time 5000 completed. Closing client channel.");

                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }

        static void Main() => RunClientAsync().Wait();
    }

    public class LoggingClientHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket packet)
        {
            Console.WriteLine($"Client Received => {packet}");

            if (!packet.Content.IsReadable())
            {
                return;
            }

            string message = packet.Content.ToString(Encoding.UTF8);
            Console.WriteLine($"Client received: {message}");
            ctx.CloseAsync();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}
