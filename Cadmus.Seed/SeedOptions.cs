using Cadmus.Core.Config;

namespace Cadmus.Seed
{
    /// <summary>
    /// General seeding options.
    /// </summary>
    public class SeedOptions
    {
        /// <summary>
        /// Gets or sets the optional seed to be used for randomizers.
        /// Set this to get reproducible results.
        /// </summary>
        public int? Seed { get; set; }

        /// <summary>
        /// Gets or sets the users names to be randomly picked for assigning
        /// users to seeded data.
        /// </summary>
        public string[] Users { get; set; }

        /// <summary>
        /// Gets or sets the facets definitions to be used.
        /// </summary>
        public FacetDefinition[] FacetDefinitions { get; set; }
    }
}
