using System;
using System.Collections.Generic;

namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// A corpus' item.
    /// </summary>
    public class Item : IItem
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
        public string FacetId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items in a list.
        /// </summary>
        public string SortKey { get; set; }

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
        /// Gets or sets generic flags for the item.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets the item's parts.
        /// </summary>
        public List<IPart> Parts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item()
        {
            Id = Guid.NewGuid().ToString("N");
            TimeModified = DateTime.UtcNow;
            Parts = new List<IPart>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Title}";
        }
    }
}
