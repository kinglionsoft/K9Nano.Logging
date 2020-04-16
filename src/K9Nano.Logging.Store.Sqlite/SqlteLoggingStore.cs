using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using K9Nano.Logging.Abstractions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Serilog.Debugging;

namespace K9Nano.Logging.Store.Sqlite
{
    public class SqlteLoggingStore : ILoggingStore, IDisposable
    {
        private readonly SqliteStoreOptions _options;
        private readonly IDbConnection _dbConnection;

        public SqlteLoggingStore(IOptionsMonitor<SqliteStoreOptions> optionsMonitor)
        {
            _options = optionsMonitor.CurrentValue;
            var file = Path.Combine(_options.LogPath, "log.db");
            var newDb = !File.Exists(file);
            try
            {
                _dbConnection = new SqliteConnection($"Data Source={file};");

                if (newDb)
                {
                    _dbConnection.Execute(@"CREATE TABLE logs(
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Level INTEGER,
Timestamp INTEGER NOT NULL,
Machine TEXT,
Application TEXT NOT NULL,
Category TEXT,
Message TEXT,
Exception TEXT);

CREATE INDEX idx_logs_app_time ON logs (Application, Timestamp);
");
                }
                _dbConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Sqlite startup failed: " + e);
                throw;
            }
        }
        
        public void Save(LogEntity entity)
        {
            _dbConnection.Execute(
                @"insert into logs(Level,Timestamp,Machine,Application,Category,Message,Exception)
values (@Level,@Timestamp,@Machine,@Application,@Category,@Message,@Exception)", entity);
        }

        public bool TrySave(LogEntity entity)
        {
            try
            {
                Save(entity);
                return true;
            }
            catch (Exception e)
            {
                SelfLog.WriteLine("Save log failed: {0}", e);
                return false;
            }
        }

        public async Task<IReadOnlyList<LogEntity>> QueryAsync(string application, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellation)
        {
            var sql = "select Level,Timestamp,Machine,Application,Category,Message,Exception from logs where ";
            if (!string.IsNullOrEmpty(application))
            {
                sql += "Application=@application and Timestamp between @from and @to";
            }
            else
            {
                sql += "Timestamp between @from and @to";
            }
            return (await _dbConnection.QueryAsync<LogEntity>(sql,
                new
                {
                    application,
                    from = from.ToUnixTimeMilliseconds(),
                    to = to.ToUnixTimeMilliseconds(),
                }))
                .ToList();
        }

        public void Delete(int keepDays)
        {
            var time = DateTimeOffset.Now.AddDays(-1 * keepDays)
                 .ToUnixTimeMilliseconds();
            _dbConnection.Execute("delete from logs where Timestamp<@time", new { time });
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }
    }
}