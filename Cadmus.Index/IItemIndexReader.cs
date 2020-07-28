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
        /// Searches for items the index with the specified query.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>Page of results.</returns>
        DataPage<ItemInfo> SearchItems(string query, PagingOptions options);

        /// <summary>
        /// Searches for pins the index with the specified query.
        /// </summary>
        /// <param name="query">The query text.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>Page of results.</returns>
        DataPage<DataPinInfo> SearchPins(string query, PagingOptions options);

        /// <summary>
        /// Closes the connection to the target database.
        /// </summary>
        void Close();
    }
}
