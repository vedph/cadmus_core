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
/// Parts seeders factory based on a configuration. This instantiates and
/// configures all the components needed to seed parts and fragments.
/// </summary>
/// <remarks>
/// <para>The seed configuration is added under a <c>seed</c> property to a
/// standard Cadmus profile; this property has the following sections:</para>
/// <list type="bullet">
/// <item>
/// <term>options</term>
/// <description>general seed options (see <see cref="SeedOptions"/>).
/// </description>
/// </item>
/// <item>
/// <term>itemSortKeyBuilder</term>
/// <description>the optional item sort key builder ID with its options if any.
/// When not specified, the <see cref="StandardItemSortKeyBuilder"/>
/// is used.
/// </description>
/// </item>
/// <item>
/// <term>partSeeders</term>
/// <description>the part seeders. This is an array with <c>Id</c> and their
/// optional <c>Options</c>. The ID can optionally include a role suffix
/// (e.g., <c>seed.it.vedph.categories:function</c>) to provide different
/// options for the same part type with different roles.</description>
/// </item>
/// <item>
/// <term>fragmentSeeders</term>
/// <description>the fragment seeders. This is an array with <c>Id</c> and
/// their optional <c>Options</c>. The ID can optionally include a role
/// suffix (e.g., <c>seed.fr.it.vedph.comment:scholarly</c>) to provide
/// different options for the same fragment type with different roles.
/// </description>
/// </item>
/// </list>
/// <para>Also, besides the <c>seed</c> property we might find an
/// <c>imports</c> property, which lists import sources as an array of
/// strings. These can be retrieved with <see cref="GetImports"/>, and are
/// used to import items and parts from JSON dumps.</para>
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
        ArgumentNullException.ThrowIfNull(services);

        ArgumentNullException.ThrowIfNull(partTypeProvider);

        services.AddSingleton(partTypeProvider);

        // https://simpleinjector.readthedocs.io/en/latest/advanced.html?highlight=batch#batch-registration
        Assembly[] assemblies =
        [
            // Cadmus.Seed
            typeof(PartSeederFactory).Assembly,
            // Cadmus.Core
            typeof(StandardItemSortKeyBuilder).Assembly
        ];
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
    /// <remarks>
    /// <para>This method supports role-aware lookup. If the
    /// <paramref name="typeId"/> includes a role suffix (e.g.,
    /// <c>seed.fr.it.vedph.comment:scholarly</c>), it first tries to find
    /// a seeder with that exact ID. If not found, it falls back to the
    /// type-only ID (e.g., <c>seed.fr.it.vedph.comment</c>).</para>
    /// <para>This allows having different seeding options for the same
    /// fragment type with different roles.</para>
    /// </remarks>
    /// <param name="typeId">The fragment seeder type ID, optionally including
    /// a role suffix (e.g., <c>seed.fr.it.vedph.comment</c> or
    /// <c>seed.fr.it.vedph.comment:scholarly</c>).</param>
    /// <returns>Seeder, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">typeId</exception>
    public IFragmentSeeder? GetFragmentSeeder(string typeId)
    {
        ArgumentNullException.ThrowIfNull(typeId);

        // try exact match first (may include role suffix)
        ComponentFactoryConfigEntry? entry =
            ComponentFactoryConfigEntry.ReadComponentEntry(
                Configuration, "Seed:FragmentSeeders", typeId);

        // if not found and typeId has a role suffix, try without it
        if (entry == null)
        {
            int colonIndex = typeId.IndexOf(':');
            if (colonIndex > 0)
            {
                string typeIdWithoutRole = typeId[..colonIndex];
                entry = ComponentFactoryConfigEntry.ReadComponentEntry(
                    Configuration, "Seed:FragmentSeeders", typeIdWithoutRole);
            }
        }

        if (entry == null) return null;

        // For component lookup, always use the base tag (without role suffix)
        // The role suffix is only used to find the correct config entry/options
        string lookupTag = typeId;
        int colonIdx = typeId.IndexOf(':');
        if (colonIdx > 0)
            lookupTag = typeId[..colonIdx];

        SeedOptions options = GetSeedOptions();

        IFragmentSeeder? seeder = GetComponent<IFragmentSeeder>(
            lookupTag, entry.OptionsPath, false);
        if (seeder == null) return null;

        seeder.SetSeedOptions(options);
        return seeder;
    }

    /// <summary>
    /// Gets the fragment seeder for the specified fragment type ID and
    /// optional role, which is located in <c>Seed:FragmentSeeders</c>.
    /// </summary>
    /// <remarks>
    /// <para>This overload is a convenience method that combines the
    /// fragment type ID and role into a single lookup key. It first tries
    /// <c>{typeId}:{role}</c>, then falls back to just <c>{typeId}</c>.</para>
    /// </remarks>
    /// <param name="typeId">The fragment seeder type ID (e.g.,
    /// <c>seed.fr.it.vedph.comment</c>).</param>
    /// <param name="role">The optional fragment role (e.g.,
    /// <c>scholarly</c>).</param>
    /// <returns>Seeder, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">typeId</exception>
    public IFragmentSeeder? GetFragmentSeeder(string typeId, string? role)
    {
        ArgumentNullException.ThrowIfNull(typeId);

        if (string.IsNullOrEmpty(role))
            return GetFragmentSeeder(typeId);

        // try with role first, method will fall back to type-only if not found
        return GetFragmentSeeder($"{typeId}:{role}");
    }

    /// <summary>
    /// Gets the part seeders from <c>Seed:PartSeeders</c>.
    /// </summary>
    /// <remarks>
    /// <para>The returned dictionary keys are the seeder IDs with the
    /// <c>seed.</c> prefix stripped. If a seeder has a role suffix in its
    /// ID (e.g., <c>seed.it.vedph.categories:function</c>), the key will
    /// include the role (e.g., <c>it.vedph.categories:function</c>).</para>
    /// <para>This allows having different seeders with different options
    /// for the same part type but with different roles.</para>
    /// </remarks>
    /// <returns>Dictionary where each key is a part type ID (optionally
    /// with role suffix), and each value is the corresponding seeder.
    /// </returns>
    public Dictionary<string, IPartSeeder> GetPartSeeders()
    {
        IList<ComponentFactoryConfigEntry> entries =
            ComponentFactoryConfigEntry.ReadComponentEntries(
            Configuration, "Seed:partSeeders");

        SeedOptions options = GetSeedOptions();
        Dictionary<string, IPartSeeder> result = new();

        foreach (ComponentFactoryConfigEntry entry in entries)
        {
            string fullTag = entry.Tag!;
            string dictKey = fullTag["seed.".Length..];

            // Extract the base tag (without role suffix) for component lookup
            string lookupTag = fullTag;
            int colonIndex = fullTag.IndexOf(':');
            if (colonIndex > 0)
                lookupTag = fullTag[..colonIndex];

            // Get seeder using base tag but with role-specific options
            IPartSeeder? seeder = GetComponent<IPartSeeder>(
                lookupTag, entry.OptionsPath, true);

            if (seeder != null)
            {
                seeder.SetSeedOptions(options);
                result[dictKey] = seeder;
            }
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
