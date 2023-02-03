using Cadmus.Core.Storage;

namespace Cadmus.Core;

/// <summary>
/// Item's sort key builder.
/// </summary>
public interface IItemSortKeyBuilder
{
    /// <summary>
    /// Builds the sort key from the specified item.
    /// </summary>
    /// <param name="item">The item. If your builder requires to lookup
    /// specific item's parts, you should first check for parts in the
    /// received item object, and then fallback to the repository to
    /// retrieve a missing part.</param>
    /// <param name="repository">The optional repository.</param>
    /// <returns>Sort key.</returns>
    string BuildKey(IItem item, ICadmusRepository? repository);
}
