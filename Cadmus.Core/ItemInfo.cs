using System;

namespace Cadmus.Core
{
    /// <summary>
    /// Summary information about an item.
    /// </summary>
    public class ItemInfo : IHasVersion
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Item's facet.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        public string? FacetId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier. This is an arbitrary string
        /// which can be used to group items into a set. For instance, you
        /// might have a set of items belonging to the same literary work,
        /// a set of lemmata belonging to the same dictionary letter, etc.
        /// </summary>
        public string? GroupId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items
        /// in a list.
        /// </summary>
        public string? SortKey { get; set; }

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
        /// Gets or sets the payload, a general-purpose payload optionally
        /// associated to this info. This can be provided by item browsers.
        /// </summary>
        public object? Payload { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInfo"/> class.
        /// </summary>
        public ItemInfo()
        {
            CreatorId = UserId = "";
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
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
