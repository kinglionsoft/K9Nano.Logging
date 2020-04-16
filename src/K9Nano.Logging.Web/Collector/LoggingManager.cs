using System;
using System.Threading.Tasks;
using K9Nano.Logging.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace K9Nano.Logging.Web.Collector
{
    public class LoggingManager : ILoggingManager
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ISerializer _serializer;
        private readonly ILoggingStore _store;
        private readonly GreedyBatchBlock<byte[]> _greedyBatchBlock;
        private readonly ServerOptions _options;

        public LoggingManager(IHostApplicationLifetime lifetime,
            ISerializer serializer,
            ILoggingStore store,
            IOptionsMonitor<ServerOptions> optionsMonitor)
        {
            _options = optionsMonitor.CurrentValue;
            _lifetime = lifetime;
            _serializer = serializer;
            _store = store;
            _greedyBatchBlock = new GreedyBatchBlock<byte[]>(1000);
            Task.Factory
                .StartNew(Start, lifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void Start()
        {
            var rolling = _options.KeepDays > 0;
            var lastRollingTime = DateTime.Today.AddHours(2);
            while (true)
            {
                var bufferList = _greedyBatchBlock.Receive();
                foreach (var data in bufferList)
                {
                    if (_serializer.TryDeserialize(data, out var result))
                    {
                        // Broadcast to clients

                        // save
                        _store.TrySave(result);
                    }
                }

                // todo To be optimized
                // Schedule rolling job during the lowest period of the day
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

        public virtual void Post(byte[] data)
        {
            _greedyBatchBlock.Post(data);
        }
    }
}