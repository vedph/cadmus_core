namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history entries.
    /// </summary>
    public class HistoryFilter : VersionFilter
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
