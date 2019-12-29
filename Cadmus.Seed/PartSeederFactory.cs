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
    /// The configuration has these sections:
    /// <list type="bullet">
    /// <item>
    /// <term>SeedOptions</term>
    /// <description>general seed options (see <see cref="SeedOptions"/>).
    /// </description>
    /// </item>
    /// <item>
    /// <term>ItemSortKeyBuilder</term>
    /// <description>the optional item sort key builder ID and its eventual
    /// options. When not specified, the <see cref="StandardItemSortKeyBuilder"/>
    /// is used.
    /// </description>
    /// </item>
    /// <item>
    /// <term>PartSeeders</term>
    /// <description>the part seeders. Array with <c>Id</c> and eventual
    /// <c>Options</c>.</description>
    /// </item>
    /// <item>
    /// <term>FragmentSeeders</term>
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
        /// <c>Pythia.Core</c>.
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
        }

        /// <summary>
        /// Gets the seed options.
        /// </summary>
        /// <returns>The seed options</returns>
        public SeedOptions GetSeedOptions()
        {
            IConfigurationSection section = Configuration.GetSection("SeedOptions");
            return section.Get<SeedOptions>();
        }

        /// <summary>
        /// Gets the optional item sort key builder. If not specified, the
        /// <see cref="StandardItemSortKeyBuilder"/> will be used.
        /// </summary>
        /// <returns>Item sort key builder.</returns>
        public IItemSortKeyBuilder GetItemSortKeyBuilder()
        {
            return GetComponent<IItemSortKeyBuilder>(
                Configuration["ItemSortKeyBuilder:Id"],
                "ItemSortKeyBuilder:Options",
                false);
        }

        /// <summary>
        /// Gets the fragment seeders.
        /// </summary>
        /// <returns>Dictionary where each key is a fragment type ID, and each
        /// value is the corresponding seeder.</returns>
        public Dictionary<string, IFragmentSeeder> GetFragmentSeeders()
        {
            IList<ComponentFactoryConfigEntry> entries =
                ComponentFactoryConfigEntry.ReadComponentEntries(
                Configuration, "FragmentSeeders");

            SeedOptions options = GetSeedOptions();

            IList<IFragmentSeeder> seeders =
                GetComponents<IFragmentSeeder>(entries, false, true);

            int i = 0;
            Dictionary<string, IFragmentSeeder> result =
                new Dictionary<string, IFragmentSeeder>();

            foreach (IFragmentSeeder seeder in seeders)
            {
                string id = entries[i++].Id;
                id = id.Substring("seed.".Length);

                seeder.Configure(options);
                result[id] = seeder;
            }

            return result;
        }

        /// <summary>
        /// Gets the part seeders.
        /// </summary>
        /// <returns>Dictionary where each key is a part type ID, and each
        /// value is the corresponding seeder.</returns>
        public Dictionary<string, IPartSeeder> GetPartSeeders()
        {
            IList<ComponentFactoryConfigEntry> entries =
                ComponentFactoryConfigEntry.ReadComponentEntries(
                Configuration, "PartSeeders");

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

                seeder.Configure(options);
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
