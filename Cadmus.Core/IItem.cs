using System.Collections.Generic;

namespace Cadmus.Core
{
    /// <summary>
    /// Item.
    /// </summary>
    public interface IItem : IHasVersion
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Item's facet ID.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        string FacetId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items
        /// in a list.
        /// </summary>
        string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        int Flags { get; set; }

        /// <summary>
        /// Gets or sets the item's parts.
        /// </summary>
        List<IPart> Parts { get; set; }
    }
}
