using Fusi.Tools.Configuration;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System;
using System.Linq;
using Cadmus.Core;
using System.Collections.Generic;
using Cadmus.Core.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Cadmus.Seed;

/// <summary>
/// Parts seeders factory based on a configuration.
/// </summary>
/// <remarks>
/// The seed configuration is added under a <c>seed</c> property to a
/// standard Cadmus profile; this property has the following sections:
/// <list type="bullet">
/// <item>
/// <term>options</term>
/// <description>general seed options (see <see cref="SeedOptions"/>).
/// </description>
/// </item>
/// <item>
/// <term>itemSortKeyBuilder</term>
/// <description>the optional item sort key builder ID and its eventual
/// options. When not specified, the <see cref="StandardItemSortKeyBuilder"/>
/// is used.
/// </description>
/// </item>
/// <item>
/// <term>partSeeders</term>
/// <description>the part seeders. Array with <c>Id</c> and eventual
/// <c>Options</c>.</description>
/// </item>
/// <item>
/// <term>fragmentSeeders</term>
/// <description>the fragment seeders. Array with <c>Id</c> and eventual
/// <c>Options</c>.</description>
/// </item>
/// </list>
/// Also, besides the <c>seed</c> property we might find an <c>imports</c>
/// property, which lists import sources as an array of strings. These can
/// be retrieved with <see cref="GetImports"/>, and are used to import
/// items and parts from JSON dumps.
/// </remarks>
public sealed class PartSeederFactory : ComponentFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartSeederFactory"/>
    /// class.
    /// </summary>
    /// <param name="host">The host.</param>
    public PartSeederFactory(IHost host) : base(host)
    {
    }

    /// <summary>
    /// Configures the container services to use components from
    /// <c>Cadmus.Core</c> and <c>Cadmus.Seed</c>, plus the assemblies
    /// specified by <paramref name="additionalAssemblies"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="partTypeProvider">The part type provider.</param>
    /// <param name="additionalAssemblies">The optional additional
    /// assemblies.</param>
    /// <exception cref="ArgumentNullException">container or part type
    /// provider</exception>
    public static void ConfigureServices(IServiceCollection services,
        IPartTypeProvider partTypeProvider,
        params Assembly[] additionalAssemblies)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        if (partTypeProvider is null)
            throw new ArgumentNullException(nameof(partTypeProvider));

        services.AddSingleton(partTypeProvider);

        // https://simpleinjector.readthedocs.io/en/latest/advanced.html?highlight=batch#batch-registration
        Assembly[] assemblies = new[]
        {
            // Cadmus.Seed
            typeof(PartSeederFactory).Assembly,
            // Cadmus.Core
            typeof(StandardItemSortKeyBuilder).Assembly
        };
        if (additionalAssemblies?.Length > 0)
            assemblies = assemblies.Concat(additionalAssemblies).ToArray();

        foreach (Type it in new[]
        {
            typeof(IItemSortKeyBuilder),
            typeof(IPartSeeder),
            typeof(IFragmentSeeder),
        })
        {
            foreach (Type t in GetAssemblyConcreteTypes(assemblies, it))
            {
                services.AddTransient(it, t);
            }
        }
    }

    /// <summary>
    /// Gets the seed options from <c>Seed:Options</c>.
    /// </summary>
    /// <returns>The seed options</returns>
    public SeedOptions GetSeedOptions()
    {
        IConfigurationSection? section =
            Configuration.GetSection("Seed")?.GetSection("Options");
        SeedOptions options = section?.Get<SeedOptions>()
            ?? new SeedOptions();

        options.FacetDefinitions = Configuration.GetSection("Facets")
            .Get<FacetDefinition[]>();

        return options;
    }

    /// <summary>
    /// Gets the optional item sort key builder from
    /// <c>Seed:ItemSortKeyBuilder</c>. If not specified, the
    /// <see cref="StandardItemSortKeyBuilder"/> will be used.
    /// </summary>
    /// <returns>Item sort key builder.</returns>
    public IItemSortKeyBuilder? GetItemSortKeyBuilder() =>
        GetComponent<IItemSortKeyBuilder>("Seed:ItemSortKeyBuilder", false);

    /// <summary>
    /// Gets the fragment seeder for the specified fragment type ID,
    /// which is located in <c>Seed:FragmentSeeders</c>.
    /// </summary>
    /// <param name="typeId">The fragment seeder type ID.</param>
    /// <returns>Seeder, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">typeId</exception>
    public IFragmentSeeder? GetFragmentSeeder(string typeId)
    {
        if (typeId == null) throw new ArgumentNullException(nameof(typeId));

        var entry = ComponentFactoryConfigEntry.ReadComponentEntry(
            Configuration, "Seed:FragmentSeeders", typeId);
        if (entry == null) return null;

        SeedOptions options = GetSeedOptions();

        IFragmentSeeder? seeder = GetComponent<IFragmentSeeder>(
            typeId, entry.OptionsPath, false);
        if (seeder == null) return null;

        seeder.SetSeedOptions(options);
        return seeder;
    }

    /// <summary>
    /// Gets the part seeders from <c>Seed:PartSeeders</c>.
    /// </summary>
    /// <returns>Dictionary where each key is a part type ID, and each
    /// value is the corresponding seeder.</returns>
    public Dictionary<string, IPartSeeder> GetPartSeeders()
    {
        IList<ComponentFactoryConfigEntry> entries =
            ComponentFactoryConfigEntry.ReadComponentEntries(
            Configuration, "Seed:partSeeders");

        SeedOptions options = GetSeedOptions();

        IList<IPartSeeder> seeders = GetRequiredComponents<IPartSeeder>(entries);

        int i = 0;
        Dictionary<string, IPartSeeder> result = new();

        foreach (IPartSeeder seeder in seeders)
        {
            string id = entries[i++].Tag!;
            id = id["seed.".Length..];

            seeder.SetSeedOptions(options);
            result[id] = seeder;
        }

        return result;
    }

    /// <summary>
    /// Gets the list of imports sources defined in the seeder configuration.
    /// The value of each source depends on its host; it might be a file
    /// path when the source is just a file; a web resource when the source
    /// is an HTTP URI; etc. In the configuration, the list of imports is
    /// an array property named <c>imports</c>.
    /// </summary>
    /// <returns>List of imports, or null.</returns>
    public IList<string>? GetImports()
    {
        return Configuration.GetSection("imports")?.Get<List<string>>();
    }

    /// <summary>
    /// Gets the item seeder.
    /// </summary>
    /// <returns>The item seeder.</returns>
    public ItemSeeder GetItemSeeder()
    {
        return new ItemSeeder(GetSeedOptions());
    }
}
