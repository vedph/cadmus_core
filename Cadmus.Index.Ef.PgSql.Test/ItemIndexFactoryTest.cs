using Cadmus.Index.Config;
using Fusi.Microsoft.Extensions.Configuration.InMemoryJson;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Cadmus.Index.Ef.PgSql.Test;

public sealed class ItemIndexFactoryTest
{
    private static IHost GetHost(string config)
    {
        // build the container for seeders
        Assembly[] indexAssemblies = new[]
        {
            // Cadmus.Index.Ef.PgSql
            typeof(EfPgSqlItemIndexWriter).Assembly,
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

    private static string LoadResourceText(string name)
    {
        using StreamReader reader = new(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"Cadmus.Index.Ef.PgSql.Test.Assets.{name}")!,
            Encoding.UTF8);
        return reader.ReadToEnd();
    }

    [Fact]
    public void GetIndex_Ok()
    {
        const string CS = "Server=localhost;Database=x;" +
            "User Id=postgres;Password=postgres;";
        ItemIndexFactory factory = new(
            GetHost(LoadResourceText("Config.json")), CS);

        IItemIndexWriter? writer = factory.GetItemIndexWriter();
        Assert.NotNull(writer);
        EfItemIndexWriter? efw = writer as EfItemIndexWriter;
        Assert.NotNull(efw);
        Assert.Equal(CS, efw.ConnectionString);

        IItemIndexReader? reader = factory.GetItemIndexReader();
        Assert.NotNull(reader);
        EfItemIndexReader? efr = reader as EfItemIndexReader;
        Assert.NotNull(efr);
        Assert.Equal(CS, efr.ConnectionString);
    }
}
