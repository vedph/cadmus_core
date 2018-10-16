using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history entries.
    /// </summary>
    public interface IHistoryFilter
    {
        /// <summary>
        /// Gets or sets the content identifier.
        /// </summary>
        string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier filter.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the status filter.
        /// </summary>
        EditStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the minimum modified date and time filter.
        /// </summary>
        DateTime? MinModified { get; set; }

        /// <summary>
        /// Gets or sets the maximum modified date and time filter.
        /// </summary>
        DateTime? MaxModified { get; set; }
    }
}
