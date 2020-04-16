using System.IO;
using K9Nano.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace K9Nano.Logging.Store.Sqlite
{
    public static class SqliteStartupExtensions
    {
        public static IServiceCollection AddSqliteLoggingStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SqliteStoreOptions>(configuration);

            services.AddSingleton<ILoggingStore, SqlteLoggingStore>();

            services.PostConfigure<SqliteStoreOptions>(options =>
            {
                var dir = new DirectoryInfo(options.LogPath);
                if(!dir.Exists) dir.Create();
                options.LogPath = dir.FullName;
            });

            return services;
        }
    }
}