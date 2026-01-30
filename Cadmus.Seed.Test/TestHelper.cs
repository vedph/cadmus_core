using Cadmus.Core;
using Cadmus.Core.Config;
using Fusi.Microsoft.Extensions.Configuration.InMemoryJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cadmus.Seed.Test;

/// <summary>
/// Test helper for seeder tests.
/// </summary>
internal static class TestHelper
{
    private static IHost? _host;

    /// <summary>
    /// Gets the embedded resource stream from the test assembly.
    /// </summary>
    /// <param name="name">The resource name.</param>
    /// <returns>Stream.</returns>
    private static Stream GetResourceStream(string name)
    {
        return Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"Cadmus.Seed.Test.{name}")!;
    }

    /// <summary>
    /// Gets the embedded resource text from the test assembly.
    /// </summary>
    /// <param name="name">The resource name.</param>
    /// <returns>Text.</returns>
    public static string GetResourceText(string name)
    {
        using StreamReader reader = new(GetResourceStream(name),
            Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets a configured host for the test.
    /// </summary>
    /// <returns>Host.</returns>
    public static IHost GetHost()
    {
        if (_host != null) return _host;

        string config = GetResourceText("Assets.SeedConfig.json");

        _host = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                // add part type provider with test types
                TagAttributeToTypeMap map = new();
                map.Add([
                    typeof(CategoriesPart).Assembly,  // test types
                ]);
                IPartTypeProvider partTypeProvider =
                    new StandardPartTypeProvider(map);

                // configure factory services
                PartSeederFactory.ConfigureServices(services,
                    partTypeProvider,
                    typeof(CategoriesPartSeeder).Assembly);
            })
            .AddInMemoryJson(config)
            .Build();

        return _host;
    }

    /// <summary>
    /// Gets a new part seeder factory.
    /// </summary>
    /// <returns>Factory.</returns>
    public static PartSeederFactory GetFactory()
    {
        return new PartSeederFactory(GetHost());
    }

    /// <summary>
    /// Gets a sample item for testing.
    /// </summary>
    /// <param name="facetId">The optional facet ID.</param>
    /// <returns>Item.</returns>
    public static IItem GetItem(string? facetId = null)
    {
        return new Item
        {
            Title = "Test item",
            Description = "Test item description",
            FacetId = facetId ?? "default",
            GroupId = "test",
            SortKey = "test",
            CreatorId = "zeus",
            UserId = "zeus"
        };
    }

    /// <summary>
    /// Asserts that the part metadata is properly set.
    /// </summary>
    /// <param name="part">The part.</param>
    public static void AssertPartMetadata(IPart part)
    {
        ArgumentNullException.ThrowIfNull(part);

        Assert.False(string.IsNullOrEmpty(part.Id));
        Assert.False(string.IsNullOrEmpty(part.ItemId));
        Assert.False(string.IsNullOrEmpty(part.TypeId));
        Assert.False(string.IsNullOrEmpty(part.CreatorId));
        Assert.False(string.IsNullOrEmpty(part.UserId));
    }
}
