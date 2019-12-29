using Cadmus.Core;
using Cadmus.Core.Layers;
using System;

namespace Cadmus.Seed
{
    /// <summary>
    /// Base class for <see cref="IFragmentSeeder"/>'s.
    /// </summary>
    /// <seealso cref="Cadmus.Seed.IFragmentSeeder" />
    public abstract class FragmentSeederBase : IFragmentSeeder
    {
        /// <summary>
        /// Gets the options.
        /// </summary>
        protected SeedOptions Options { get; private set; }

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        protected Random Random { get; private set; }

        /// <summary>
        /// Configures this seeder with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(SeedOptions options)
        {
            Options = options ??
                throw new ArgumentNullException(nameof(options));
            Random = options.Seed != null
                ? new Random(options.Seed.Value)
                : new Random();
        }

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <returns>Type.</returns>
        public abstract Type GetFragmentType();

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="location">The location.</param>
        /// <param name="baseText">The base text.</param>
        /// <returns>A new fragment.</returns>
        public abstract ITextLayerFragment GetFragment(
            IItem item, string location, string baseText);
    }
}
