using Cadmus.Core;
using Cadmus.Core.Storage;
using Fusi.Tools;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Index
{
    /// <summary>
    /// Items indexer. This uses an <see cref="IItemIndexWriter"/> to write
    /// into the index the data pins from a single item or a set of items.
    /// </summary>
    public sealed class ItemIndexer
    {
        private readonly IItemIndexWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemIndexer"/> class.
        /// </summary>
        /// <param name="writer">The index writer.</param>
        /// <exception cref="ArgumentNullException">repository or writer</exception>
        public ItemIndexer(IItemIndexWriter writer)
        {
            _writer = writer
                ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>
        /// Gets the index pins from the parts of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="indexTime">The index time to be set.</param>
        /// <returns>List of index pins.</returns>
        public static IList<IndexPin> GetIndexPins(IItem item, DateTime indexTime)
        {
            List<IndexPin> pins = new List<IndexPin>();

            foreach (IPart part in item.Parts)
            {
                foreach (DataPin pin in part.GetDataPins())
                {
                    pins.Add(new IndexPin
                    {
                        ItemId = pin.ItemId,
                        PartId = pin.PartId,
                        PartTypeId = part.TypeId,
                        RoleId = pin.RoleId,
                        Name = pin.Name,
                        Value = pin.Value,
                        TimeIndexed = indexTime
                    });
                }
            }
            return pins;
        }

        /// <summary>
        /// Adds the specified item to the index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void AddItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _writer.Write(item);
        }

        /// <summary>
        /// Clears the whole index.
        /// </summary>
        public Task Clear() => _writer.Clear();

        /// <summary>
        /// Builds the whole index by indexing all the items in the specified
        /// repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="filter">The optional item filter; paging information
        /// will be overwritten.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <param name="progress">The optional progress reporter.</param>
        public void Build(
            ICadmusRepository repository,
            ItemFilter filter,
            CancellationToken cancel,
            IProgress<ProgressReport> progress = null)
        {
            if (filter == null) filter = new ItemFilter();
            filter.PageNumber = 1;
            filter.PageSize = 100;

            // first page
            DataPage<ItemInfo> page = repository.GetItems(filter);
            if (page.Total == 0) return;

            ProgressReport report = progress != null
                ? new ProgressReport()
                : null;

            do
            {
                // index all the items in page
                foreach (ItemInfo info in page.Items)
                {
                    IItem item = repository.GetItem(info.Id);
                    if (item == null) continue;
                    _writer.Write(item);
                }

                // handle cancel and progress
                if (cancel.IsCancellationRequested) break;

                if (progress != null)
                {
                    report.Count += page.Items.Count;
                    report.Percent = report.Count * 100 / page.Total;
                    progress.Report(report);
                }

                // next page
                if (++filter.PageNumber > page.PageCount) break;
                page = repository.GetItems(filter);
            } while (page.Items.Count != 0);
        }
    }
}
