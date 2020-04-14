using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogCenter.Web.Collector
{
    public class CollectorHostedService: IHostedService
    {
        private readonly ILogger _logger;
        private  readonly IEventLoopGroup _bossGroup;
        private readonly IEventLoopGroup _workerGroup;
        private IChannel _boundChannel;
        private readonly Bootstrap _bootstrap;
        private readonly ServerOptions _options;

        public CollectorHostedService(IOptionsMonitor<ServerOptions> optionsMonitor, ILogger<CollectorHostedService> logger)
        {
            _logger = logger;
            _options = optionsMonitor.CurrentValue;

            if (_options.UseLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                _bossGroup = dispatcher;
                _workerGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                _bossGroup = new MultithreadEventLoopGroup(1);
                _workerGroup = new MultithreadEventLoopGroup();
            }

            _bootstrap = new Bootstrap();
            _bootstrap.Group(_workerGroup)
                .Channel<SocketDatagramChannel>()
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("echo", new EchoServerHandler());
                }));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _boundChannel = await _bootstrap.BindAsync(_options.Port);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_boundChannel != null)
            {
                await _boundChannel.CloseAsync();
            }

            await Task.WhenAll(
                _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }
    }
}