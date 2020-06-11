using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace K9Nano.Logging.Web.Collector
{
    public class CollectorHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IEventLoopGroup _workerGroup;
        private IChannel _boundChannel;
        private readonly Bootstrap _bootstrap;
        private readonly ServerOptions _options;

        public CollectorHostedService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetService<ILogger<CollectorHostedService>>();
            _options = serviceProvider.GetService<IOptionsMonitor<ServerOptions>>().CurrentValue;

            _workerGroup = new MultithreadEventLoopGroup();

            _bootstrap = new Bootstrap();
            _bootstrap.Group(_workerGroup)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
                .Option(ChannelOption.RcvbufAllocator, new FixedRecvByteBufAllocator(65535)) // 最大接收、发送的长度
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("LOGGING", new LoggingCollectorServerHandler(serviceProvider));
                }));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _boundChannel = await _bootstrap.BindAsync(_options.Port);
            Console.WriteLine($"UDP server started at 0.0.0.0:{_options.Port}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_boundChannel != null)
            {
                await _boundChannel.CloseAsync();
            }

            await _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }
    }
}