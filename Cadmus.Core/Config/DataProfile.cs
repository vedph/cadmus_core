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
        public FacetDefinition[] Facets { get; set; }

        /// <summary>
        /// Gets the flags definitions.
        /// </summary>
        public FlagDefinition[] Flags { get; set; }

        /// <summary>
        /// Gets the thesauri.
        /// </summary>
        public Thesaurus[] Thesauri { get; set; }
    }
}
