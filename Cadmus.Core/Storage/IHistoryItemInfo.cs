using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Essential information about a history item.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Blocks.IHasVersion" />
    /// <seealso cref="Cadmus.Core.Storage.IHasHistory" />
    public interface IHistoryItemInfo : IHasVersion, IHasHistory
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
        /// Item's facet.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        string Facet { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items in a list.
        /// </summary>
        string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        int Flags { get; set; }
    }
}
