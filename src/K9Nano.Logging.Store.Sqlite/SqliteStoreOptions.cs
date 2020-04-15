using System.Collections.Concurrent;

namespace K9Nano.Logging.Store.Sqlite
{
    public sealed class SqliteStoreOptions
    {
        public string LogPath { get; set; } = "logs";
    }
}
