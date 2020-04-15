using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace K9Nano.Logging.Web.Collector
{
    public sealed class GreedyBatchBlock<T>
    {
        private readonly BufferBlock<T> _bb = new BufferBlock<T>();

        private readonly int _batchSize;

        public GreedyBatchBlock(int batchSize)
        {
            _batchSize = batchSize;
        }

        public void Post(T item)
        {
            _bb.Post(item);
        }

        public async Task<IReadOnlyList<T>> ReceiveAsync(CancellationToken token)
        {
            while (true)
            {
                var l = new List<T>
                {
                    await _bb.ReceiveAsync(token)
                };
                while (_bb.TryReceive(null, out var item))
                {
                    l.Add(item);
                    if (l.Count == _batchSize) break;
                }
                return l;
            }
        }

        public IReadOnlyList<T> Receive()
        {
            while (true)
            {
                var l = new List<T>
                {
                    _bb.Receive()
                };
                while (_bb.TryReceive(null, out var item))
                {
                    l.Add(item);
                    if (l.Count == _batchSize) break;
                }
                return l;
            }
        }
    }
}