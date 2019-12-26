using Cadmus.Core.Storage;

namespace Cadmus.Core
{
    /// <summary>
    /// Item's sort key builder.
    /// </summary>
    public interface IItemSortKeyBuilder
    {
        /// <summary>
        /// Builds the sort key from the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="repository">The repository.</param>
        /// <returns>Sort key.</returns>
        string BuildKey(IItem item, ICadmusRepository repository);
    }
}
