using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace K9Nano.Logging.Web.Collector
{
    public interface ILoggingStore
    {
        void Save(LogEntity entity);

        bool TrySave(LogEntity entity);

        Task<Stream> DownloadAsync(string application, DateTime from, DateTime to, CancellationToken cancellation);
    }
}