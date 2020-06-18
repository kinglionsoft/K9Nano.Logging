using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using K9Nano.Logging.Abstractions;
using K9Nano.Logging.Web.Extensions;
using K9Nano.Logging.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace K9Nano.Logging.Web.Collector
{
    public class LoggingManager : ILoggingManager
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ISerializer _serializer;
        private readonly ILoggingStore _store;
        private readonly IHubContext<LogHub> _hubContext;
        private readonly GreedyBatchBlock<byte[]> _greedyBatchBlock;
        private readonly ServerOptions _options;

        public LoggingManager(IHostApplicationLifetime lifetime,
            ISerializer serializer,
            ILoggingStore store,
            IOptionsMonitor<ServerOptions> optionsMonitor,
            IHubContext<LogHub> hubContext)
        {
            _options = optionsMonitor.CurrentValue;
            _lifetime = lifetime;
            _serializer = serializer;
            _store = store;
            _hubContext = hubContext;
            _greedyBatchBlock = new GreedyBatchBlock<byte[]>(1000);
            Task.Factory
                .StartNew(Start, lifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void Start()
        {
            var rolling = _options.KeepDays > 0;
            var lastRollingTime = DateTime.Today.AddHours(2);
            var bufferList = new List<byte[]>(1000);
            while (true)
            {
                _greedyBatchBlock.Receive(bufferList);
                foreach (var data in bufferList)
                {
                    if (_serializer.TryDeserialize(data, out var result))
                    {
                        // Broadcast to clients
                        _hubContext.Clients.Group("ALL").SendAsync("ReceiveMessage", result)
                            .ConfigureAwait(false);
                        _hubContext.Clients.Group(result.Application).SendAsync("ReceiveMessage", result)
                            .ConfigureAwait(false);
                        // save
                        _store.TrySave(result);
                    }
                }

                // todo To be optimized: Schedule rolling job during the lowest period of the day
                if (rolling && (DateTime.Now - lastRollingTime).TotalHours >= 24)
                {
                    Roll();
                    lastRollingTime = DateTime.Now;
                }
            }
        }

        protected virtual void Roll()
        {
            try
            {
                _store.Delete(_options.KeepDays);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Rolling failed: {e}");
            }
        }

        protected virtual string Format(LogEntity entity)
        {
            // todo  timezone ?
            var localTime = DateTimeOffset.FromUnixTimeMilliseconds(entity.Timestamp)
                .ToLocalTime()
                .ToString("HH:mm:ss.fff");
            var sb = new StringBuilder();

            sb.Append(localTime)
                .Append("  ")
                .Append(entity.GetLevel())
                .Append("  ")
                .Append(entity.Machine, 15)
                .Append("  ")
                .Append(entity.Application, 15)
                .Append("  ")
                .Append(entity.Category, 20)
                .Append("  ")
                .Append(entity.Message)
                .Append("  ")
                .Append(entity.Exception);

            return sb.ToString();
        }

        public virtual void Post(byte[] data)
        {
            _greedyBatchBlock.Post(data);
        }
    }
}