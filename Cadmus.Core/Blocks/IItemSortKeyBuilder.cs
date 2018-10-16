namespace Cadmus.Core.Blocks
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
        /// <returns>sort key</returns>
        string BuildKey(IItem item);
    }
}
