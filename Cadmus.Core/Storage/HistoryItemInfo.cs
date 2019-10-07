using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Essential information about a history item.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.IHistoryItemInfo" />
    public sealed class HistoryItemInfo : IHistoryItemInfo
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item's facet.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        public string Facet { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items in
        /// a list.
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.
        /// </remarks>
        public string UserId { get; set; }

        /// <summary> Gets or sets the identifier of the data record this 
        /// history record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the record status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Title}";
        }
    }
}
