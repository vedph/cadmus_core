using System;
using System.Collections.Generic;
using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// History item.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.IHistoryItem" />
    public sealed class HistoryItem : IHistoryItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data record this history record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the record status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.</remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item's facet ID.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        public string FacetId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items in a list.
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets the item's parts.
        /// </summary>
        public List<IPart> Parts { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Id}: {Title}";
        }
    }
}
