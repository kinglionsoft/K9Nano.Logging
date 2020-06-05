using System;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Google.Protobuf;
using K9Nano.Logging;

namespace ConsoleSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var group = new MultithreadEventLoopGroup();

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                }));

            var clientChannel = bootstrap.BindAsync(IPEndPoint.MinPort)
                 .ConfigureAwait(false)
                 .GetAwaiter()
                 .GetResult();

         
            while (true)
            {
                var entity = new LogEntity
                {
                    Level = LogEventLevel.Information,
                    Machine = "YC",
                    Application = "ConsoleSample",
                    Category = "Test",
                    Message = "proto test",
                    Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                    TraceId = "12313",
                    Exception = "errors"
                };
                var data = entity.ToByteArray();
                var buffer = Unpooled.WrappedBuffer(data);
                var packet = new DatagramPacket(buffer, new IPEndPoint(IPAddress.Parse("192.168.2.24"), 32204));
               // var packet = new DatagramPacket(buffer, new IPEndPoint(IPAddress.Loopback, 6253));
                await clientChannel.WriteAndFlushAsync(packet)
                    .ConfigureAwait(false);

                var cmd = Console.ReadLine();
                if (cmd == "q") break;
            }

            await clientChannel.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}
