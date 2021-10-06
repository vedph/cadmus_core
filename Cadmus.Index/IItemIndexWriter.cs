using Cadmus.Core;
using Fusi.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Index
{
    /// <summary>
    /// Items index writer. The item index writer is used to add an item with
    /// all its parts to the index, but also has the ability of creating its
    /// target database if not present on he first write, and to remove all
    /// data from it on demand.
    /// </summary>
    public interface IItemIndexWriter
    {
        /// <summary>
        /// Gets or sets the optional data pin filter to use.
        /// </summary>
        IDataPinFilter DataPinFilter { get; set; }

        /// <summary>
        /// Writes the specified item and all its parts (if any) to the index.
        /// If the index does not exist, it is created.
        /// </summary>
        /// <param name="item">The item.</param>
        Task WriteItem(IItem item);

        /// <summary>
        /// Bulk-writes the specified items, assuming that they do not exist.
        /// This can be used to populate an empty index with higher performance.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The optional progress reporter.</param>
        Task WriteItems(IEnumerable<IItem> items,
            CancellationToken cancel, IProgress<ProgressReport> progress = null);

        /// <summary>
        /// Deletes the item with the specified identifier with all its pins
        /// entries.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        Task DeleteItem(string itemId);

        /// <summary>
        /// Writes the specified part to the index.
        /// </summary>
        /// <param name="item">The item the parts belongs to.</param>
        /// <param name="part">The part.</param>
        Task WritePart(IItem item, IPart part);

        /// <summary>
        /// Deletes the part with the specified ID from the index.
        /// </summary>
        /// <param name="partId">The part identifier.</param>
        Task DeletePart(string partId);

        /// <summary>
        /// Clears the whole index.
        /// </summary>
        Task Clear();

        /// <summary>
        /// Closes the connection to the target database.
        /// </summary>
        void Close();
    }
}
