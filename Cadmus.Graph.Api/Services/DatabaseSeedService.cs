using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Fusi.DbManager.PgSql;
using Cadmus.Graph.Ef.PgSql;

namespace Cadmus.Graph.Api.Services;

/// <summary>
/// Database seed service.
/// See https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-3.
/// </summary>
/// <seealso cref="IHostedService" />
public sealed class DatabaseSeedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    static private Stream GetResourceStream(string name) =>
        typeof(DatabaseSeedService).Assembly.GetManifestResourceStream(
        "Cadmus.Graph.Api.Assets." + name)!;

    private static void FillGraph(IGraphRepository repository)
    {
        IGraphPresetReader reader = new JsonGraphPresetReader();

        // nodes
        using (Stream stream = GetResourceStream("Petrarch-n.json"))
        using (ItemFlusher<UriNode> nodeFlusher = new(nodes =>
            repository.ImportNodes(nodes)))
        {
            foreach (UriNode node in reader.ReadNodes(stream))
                nodeFlusher.Add(node);
        }

        // triples
        using (Stream stream = GetResourceStream("Petrarch-t.json"))
        using (ItemFlusher<UriTriple> tripleFlusher = new(triples =>
            repository.ImportTriples(triples)))
        {
            foreach (UriTriple triple in reader.ReadTriples(stream))
                tripleFlusher.Add(triple);
        }
    }

    private static void SeedGraphDatabase(
        IGraphRepository repository,
        IConfiguration config,
        ILogger? logger)
    {
        // nope if database exists
        string cst = config.GetConnectionString("Template")!;
        string db = config.GetValue<string>("DatabaseName")!;

        PgSqlDbManager dbManager = new(cst);
        if (dbManager.Exists(db))
        {
            logger?.LogInformation("Database {DatabaseName} exists", db);
            return;
        }

        // else create and seed it
        logger?.LogInformation("Creating database {DatabaseName}", db);
        dbManager.CreateDatabase(db, EfPgSqlGraphRepository.GetSchema(), null);

        // fill with sample data
        FillGraph(repository);
    }

    private static Task SeedGraphDatabaseAsync(IServiceProvider serviceProvider)
    {
        return Policy.Handle<DbException>()
            .WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(60)
            }, (exception, timeSpan, _) =>
            {
                ILogger? logger = serviceProvider
                    .GetService<ILoggerFactory>()?
                    .CreateLogger(typeof(DatabaseSeedService));

                string message = "Unable to connect to DB" +
                    $" (sleep {timeSpan}): {exception.Message}";
                Console.WriteLine(message);
                logger?.LogError(exception, message);
            }).Execute(() =>
            {
                IConfiguration config =
                    serviceProvider.GetService<IConfiguration>()!;

                ILogger? logger = serviceProvider
                    .GetService<ILoggerFactory>()?
                    .CreateLogger(typeof(DatabaseSeedService));

                IGraphRepository repository =
                    serviceProvider.GetService<IGraphRepository>()!;

                Console.WriteLine("Seeding database...");
                SeedGraphDatabase(repository, config, logger);
                Console.WriteLine("Seeding completed");
                return Task.CompletedTask;
            });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;

        try
        {
            await SeedGraphDatabaseAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ILogger? logger = serviceProvider.GetService<ILoggerFactory>()!
                .CreateLogger(typeof(DatabaseSeedService));
            logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
