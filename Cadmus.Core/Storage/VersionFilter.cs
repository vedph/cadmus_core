using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// A filter for retrieving a page of versioned items.
    /// </summary>
    /// <seealso cref="Fusi.Tools.Data.PagingOptions" />
    public class VersionFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the user identifier filter.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the minimum modified date and time filter.
        /// </summary>
        public DateTime? MinModified { get; set; }

        /// <summary>
        /// Gets or sets the maximum modified date and time filter.
        /// </summary>
        public DateTime? MaxModified { get; set; }
    }
}
