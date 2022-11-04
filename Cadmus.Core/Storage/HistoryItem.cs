using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// History item.
    /// </summary>
    public sealed class HistoryItem : IHasHistory, IHasFlags
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; }

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
        /// Gets or sets the group identifier. This is an arbitrary string
        /// which can be used to group items into a set. For instance, you
        /// might have a set of items belonging to the same literary work,
        /// a set of lemmata belonging to the same dictionary letter, etc.
        /// </summary>
        public string? GroupId { get; set; }

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
        /// Creation date and time (UTC).
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// ID of the user who created the resource.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// ID of the user who last saved the resource.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data record this history record
        /// refers to.
        /// </summary>
        public string ReferenceId { get; }

        /// <summary>
        /// Gets or sets the record status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryItem"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <exception cref="ArgumentNullException">id or referenceId</exception>
        public HistoryItem(string id, string referenceId)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            ReferenceId = referenceId
                ?? throw new ArgumentNullException(nameof(referenceId));
            TimeCreated = TimeModified = DateTime.UtcNow;
            FacetId = "";
            Title = Description = "";
            CreatorId = UserId = "";
            SortKey = "";
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Title}" + (FacetId != null ? $" [{FacetId}]" : "");
        }
    }
}
