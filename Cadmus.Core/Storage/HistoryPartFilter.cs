namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history parts.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.PartFilter" />
    /// <seealso cref="Cadmus.Core.Storage.IHistoryFilter" />
    public class HistoryPartFilter : PartFilter, IHistoryFilter
    {
        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status filter.
        /// </summary>
        public EditStatus? Status { get; set; }
    }
}
