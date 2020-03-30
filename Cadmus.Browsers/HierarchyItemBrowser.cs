using Cadmus.Core;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using System;

namespace Cadmus.Browsers
{
    /// <summary>
    /// Hierarchy-based items browser.
    /// <para>Tag: <c>item-browser.hierarchy</c>.</para>
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.IItemBrowser" />
    [Tag("item-browser.hierarchy")]
    public sealed class HierarchyItemBrowser : IItemBrowser
    {
        private ICadmusRepository _repository;

        /// <summary>
        /// Sets the repository to be used.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public void SetRepository(ICadmusRepository repository)
        {
            _repository = repository ??
                throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Browses the items using the specified options.
        /// </summary>
        /// <param name="options">The paging and filtering options.
        /// You can set the page size to 0 when you want to retrieve all
        /// the items at the specified level.</param>
        /// <returns>
        /// Page of items, or null if no repository set.
        /// </returns>
        /// <exception cref="ArgumentNullException">options</exception>
        public DataPage<ItemInfo> Browse(IPagingOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (_repository == null) return null;

            // get all the hierarchy parts with the specified Y level,
            // and eventually with the specified tag

            // their count is equal to the items count

            // collect all their items, in the paging range
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Filter for <see cref="HierarchyItemBrowser"/>.
    /// </summary>
    /// <seealso cref="PagingOptions" />
    public sealed class HierarchyItemBrowserFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the Y-level filter to be matched on the
        /// <see cref="HierarchyPart"/>.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the optional tag filter to be matched on the
        /// <see cref="HierarchyPart"/>.
        /// </summary>
        public string Tag { get; set; }
    }
}
