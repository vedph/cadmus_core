namespace Cadmus.Core.Config
{
    /// <summary>
    /// Data profile. This includes metadata about facets and flags definitions
    /// and thesauri.
    /// </summary>
    public class DataProfile
    {
        /// <summary>
        /// Gets the facets.
        /// </summary>
        public FacetDefinition[] FacetDefinitions { get; set; }

        /// <summary>
        /// Gets the flags definitions.
        /// </summary>
        public FlagDefinition[] FlagDefinitions { get; set; }

        /// <summary>
        /// Gets the thesauri.
        /// </summary>
        public Thesaurus[] Thesauri { get; set; }
    }
}
