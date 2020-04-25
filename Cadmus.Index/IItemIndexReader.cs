using Cadmus.Core;
using Fusi.Tools.Data;

namespace Cadmus.Index
{
    /// <summary>
    /// Item index reader.
    /// </summary>
    public interface IItemIndexReader
    {
        /// <summary>
        /// Searches the index with the specified query.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>Page of results.</returns>
        DataPage<ItemInfo> Search(string query, PagingOptions options);
    }
}
