namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Item's filter.
    /// </summary>
    public class ItemFilter : VersionFilter
    {
        /// <summary>
        /// Gets or sets the title filter.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description filter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the facet ID filter.
        /// </summary>
        public string FacetId { get; set; }

        /// <summary>
        /// Gets or sets the group ID filter.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Gets or sets the flags filter.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets the flag matching mode to use for <see cref="Flags"/>
        /// when it is not null.
        /// </summary>
        public FlagMatching FlagMatching {get;set;}
    }

    /// <summary>
    /// Matching mode for item's flags.
    /// </summary>
    public enum FlagMatching
    {
        /// <summary>
        /// All the bits specified must be set.
        /// </summary>
        BitsAllSet = 0,
        /// <summary>
        /// Any of the bits specified must be set.
        /// </summary>
        BitsAnySet,
        /// <summary>
        /// All the bits specified must be clear.
        /// </summary>
        BitsAllClear,
        /// <summary>
        /// Any of the bits specified must be clear.
        /// </summary>
        BitsAnyClear
    }
}
