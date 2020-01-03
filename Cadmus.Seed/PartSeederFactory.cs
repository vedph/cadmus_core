using Fusi.Tools.Config;
using SimpleInjector;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System;
using System.Linq;
using Cadmus.Core;
using System.Collections.Generic;
using Cadmus.Core.Config;

namespace Cadmus.Seed
{
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
    /// </remarks>
    /// <seealso cref="Fusi.Tools.Config.ComponentFactoryBase" />
    public sealed class PartSeederFactory : ComponentFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartSeederFactory"/>
        /// class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="configuration">The configuration.</param>
        public PartSeederFactory(Container container,
            IConfiguration configuration) : base(container, configuration)
        {
        }

        /// <summary>
        /// Configures the container services to use components from
        /// <c>Cadmus.Core</c> and <c>Cadmus.Seed</c>, plus the assemblies
        /// specified by <paramref name="additionalAssemblies"/>.
        /// This is just a helper method: at any rate, the configuration of
        /// the container is external to the VSM factory. You could use this
        /// method as a model and create your own, or call this method to
        /// register the components from these two assemblies, and then
        /// further configure the container, or add more assemblies when
        /// calling this via <paramref name="additionalAssemblies"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="partTypeProvider">The part type provider.</param>
        /// <param name="additionalAssemblies">The optional additional
        /// assemblies.</param>
        /// <exception cref="ArgumentNullException">container or part type
        /// provider</exception>
        public static void ConfigureServices(Container container,
            IPartTypeProvider partTypeProvider,
            params Assembly[] additionalAssemblies)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (partTypeProvider is null)
                throw new ArgumentNullException(nameof(partTypeProvider));

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

            container.RegisterInstance(partTypeProvider);
            container.Collection.Register<IItemSortKeyBuilder>(assemblies);
            container.Collection.Register<IPartSeeder>(assemblies);
            container.Collection.Register<IFragmentSeeder>(assemblies);
        }

        /// <summary>
        /// Gets the seed options from <c>/seed/options</c>.
        /// </summary>
        /// <returns>The seed options</returns>
        public SeedOptions GetSeedOptions()
        {
            IConfigurationSection section =
                Configuration.GetSection("seed")?.GetSection("options");
            SeedOptions options = section.Get<SeedOptions>();

            options.FacetDefinitions = Configuration.GetSection("facets")
                .Get<FacetDefinition[]>();

            return options;
        }

        /// <summary>
        /// Gets the optional item sort key builder from
        /// <c>seed/itemSortKeyBuilder</c>. If not specified, the
        /// <see cref="StandardItemSortKeyBuilder"/> will be used.
        /// </summary>
        /// <returns>Item sort key builder.</returns>
        public IItemSortKeyBuilder GetItemSortKeyBuilder()
        {
            IConfigurationSection section =
                Configuration.GetSection("seed:itemSortKeyBuilder");
            if (!section.Exists()) return null;

            return GetComponent<IItemSortKeyBuilder>(
                section["id"],
                "itemSortKeyBuilder:options",
                false);
        }

        /// <summary>
        /// Gets the fragment seeder for the specified fragment type ID,
        /// which is located in <c>seed:fragmentSeeders</c>.
        /// </summary>
        /// <param name="typeId">The fragment seeder type ID.</param>
        /// <returns>Seeder, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">typeId</exception>
        public IFragmentSeeder GetFragmentSeeder(string typeId)
        {
            if (typeId == null) throw new ArgumentNullException(nameof(typeId));

            var entry = ComponentFactoryConfigEntry.ReadComponentEntry(
                Configuration, "seed:fragmentSeeders", typeId);
            if (entry == null) return null;

            SeedOptions options = GetSeedOptions();

            IFragmentSeeder seeder = GetComponent<IFragmentSeeder>(
                typeId, entry.OptionsPath, false);
            if (seeder == null) return null;

            seeder.SetSeedOptions(options);
            return seeder;
        }

        /// <summary>
        /// Gets the part seeders from <c>seed/partSeeders</c>.
        /// </summary>
        /// <returns>Dictionary where each key is a part type ID, and each
        /// value is the corresponding seeder.</returns>
        public Dictionary<string, IPartSeeder> GetPartSeeders()
        {
            IList<ComponentFactoryConfigEntry> entries =
                ComponentFactoryConfigEntry.ReadComponentEntries(
                Configuration, "seed:partSeeders");

            SeedOptions options = GetSeedOptions();

            IList<IPartSeeder> seeders =
                GetComponents<IPartSeeder>(entries, false, true);

            int i = 0;
            Dictionary<string, IPartSeeder> result =
                new Dictionary<string, IPartSeeder>();

            foreach (IPartSeeder seeder in seeders)
            {
                string id = entries[i++].Id;
                id = id.Substring("seed.".Length);

                seeder.SetSeedOptions(options);
                result[id] = seeder;
            }

            return result;
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
}
