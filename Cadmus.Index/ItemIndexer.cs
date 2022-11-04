using Cadmus.Core;
using Cadmus.Core.Storage;
using Cadmus.Index.Config;
using Fusi.Tools;
using Fusi.Tools.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Index
{
    /// <summary>
    /// Items indexer. This uses an <see cref="IItemIndexWriter"/> to write
    /// into the index the data pins from a single item or a set of items.
    /// You can create an <see cref="IItemIndexWriter"/> using an
    /// <see cref="ItemIndexFactory"/>, which requires a configuration
    /// for the index writer, and a connection string.
    /// The indexer can either add or update the indexes for a single item with its
    /// parts, or build the whole index from all the items matching a specified
    /// <see cref="ItemFilter"/>.
    /// </summary>
    public sealed class ItemIndexer
    {
        private readonly IItemIndexWriter _writer;

        /// <summary>
        /// Gets or sets the optional logger.
        /// </summary>
        public ILogger? Logger { get; set; }

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
        /// Adds the specified item to the index.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void AddItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _writer.WriteItem(item);
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
            IProgress<ProgressReport>? progress = null)
        {
            if (filter == null) filter = new ItemFilter();
            filter.PageNumber = 1;
            filter.PageSize = 100;

            // first page
            DataPage<ItemInfo> page = repository.GetItems(filter);
            if (page.Total == 0) return;

            ProgressReport? report = progress != null
                ? new ProgressReport()
                : null;
            ItemInfo? currentInfo = null;

            do
            {
                try
                {
                    // index all the items in page
                    foreach (ItemInfo info in page.Items)
                    {
                        currentInfo = info;
                        IItem? item = repository.GetItem(info.Id!);
                        if (item == null) continue;
                        _writer.WriteItem(item);
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(
                        $"Error indexing {currentInfo}: {ex.Message}: {ex}");
                }
                // handle cancel and progress
                if (cancel.IsCancellationRequested) break;

                if (progress != null)
                {
                    report!.Count += page.Items.Count;
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
