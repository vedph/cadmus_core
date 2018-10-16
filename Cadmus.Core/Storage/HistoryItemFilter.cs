namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history items.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.ItemFilter" />
    /// <seealso cref="Cadmus.Core.Storage.IHistoryFilter" />
    public class HistoryItemFilter : ItemFilter, IHistoryFilter
    {
        /// <summary>
        /// Gets or sets the item identifier.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status filter.
        /// </summary>
        public EditStatus? Status { get; set; }
    }
}
