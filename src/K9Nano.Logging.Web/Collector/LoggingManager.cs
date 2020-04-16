using System;
using System.Threading.Tasks;
using K9Nano.Logging.Abstractions;
using Microsoft.Extensions.Hosting;

namespace K9Nano.Logging.Web.Collector
{
    public class LoggingManager : ILoggingManager
    {
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ISerializer _serializer;
        private readonly ILoggingStore _store;
        private readonly GreedyBatchBlock<byte[]> _greedyBatchBlock;

        public LoggingManager(IHostApplicationLifetime lifetime, ISerializer serializer, ILoggingStore store)
        {
            _lifetime = lifetime;
            _serializer = serializer;
            _store = store;
            _greedyBatchBlock = new GreedyBatchBlock<byte[]>(1000);
            Task.Factory
                .StartNew(Start, lifetime.ApplicationStopping, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void Start()
        {
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
            }
        }

        public virtual void Post(byte[] data)
        {
            _greedyBatchBlock.Post(data);
        }
    }
}