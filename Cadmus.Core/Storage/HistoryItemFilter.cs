namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history items.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.ItemFilter" />
    /// <seealso cref="Cadmus.Core.Storage.HistoryFilter" />
    public class HistoryItemFilter : HistoryFilter
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
        /// Gets or sets the flags filter.
        /// </summary>
        public int? Flags { get; set; }
    }
}
