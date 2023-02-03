using System.Collections.Generic;

namespace Cadmus.Core;

/// <summary>
/// Item.
/// </summary>
public interface IItem : IHasVersion, IHasFlags
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
    /// Gets or sets the group identifier. This is an arbitrary string
    /// which can be used to group items into a set. For instance, you
    /// might have a set of items belonging to the same literary work,
    /// a set of lemmata belonging to the same dictionary letter, etc.
    /// </summary>
    string? GroupId { get; set; }

    /// <summary>
    /// The sort key for the item. This is a value used to sort items
    /// in a list.
    /// </summary>
    string SortKey { get; set; }

    /// <summary>
    /// Gets or sets the item's parts.
    /// </summary>
    List<IPart> Parts { get; set; }
}
