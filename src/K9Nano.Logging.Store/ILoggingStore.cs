using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace K9Nano.Logging.Abstractions
{
    public interface ILoggingStore
    {
        void Save(LogEntity entity);

        bool TrySave(LogEntity entity);

        Task<IReadOnlyList<LogEntity>> QueryAsync(string application, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellation);
    }
}
