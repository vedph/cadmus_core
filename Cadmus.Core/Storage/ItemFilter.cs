using Fusi.Tools.Data;
using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Item's filter.
    /// </summary>
    public class ItemFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the title filter.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description filter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the facet ID filter.
        /// </summary>
        public string FacetId { get; set; }

        /// <summary>
        /// Gets or sets the flags filter.
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Gets or sets the minimum modified time.
        /// </summary>
        public DateTime? MinModified { get; set; }

        /// <summary>
        /// Gets or sets the maximum modified time.
        /// </summary>
        public DateTime? MaxModified { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }
    }
}
