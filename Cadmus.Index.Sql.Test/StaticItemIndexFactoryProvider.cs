using Cadmus.Index.Config;
using Cadmus.Index.MySql;
using Fusi.Microsoft.Extensions.Configuration.InMemoryJson;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Cadmus.Index.Sql.Test;

public sealed class StaticItemIndexFactoryProvider :
    IItemIndexFactoryProvider
{
    private readonly string _connectionString;

    public StaticItemIndexFactoryProvider(string connectionString)
    {
        _connectionString = connectionString ??
            throw new ArgumentNullException(nameof(connectionString));
    }

    private static IHost GetHost(string config)
    {
        // build the container for seeders
        Assembly[] indexAssemblies = new[]
        {
            // Cadmus.Index.MySql
            typeof(MySqlItemIndexWriter).Assembly,
        };

        return new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                ItemIndexFactory.ConfigureServices(
                    services,
                    indexAssemblies);
            })
            .AddInMemoryJson(config)
            .Build();
    }

    public ItemIndexFactory GetFactory(string profile)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        return new ItemIndexFactory(GetHost(profile), _connectionString);
    }
}
