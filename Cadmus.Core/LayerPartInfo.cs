using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Core
{
    /// <summary>
    /// Essential information about a layer part.
    /// </summary>
    /// <seealso cref="Cadmus.Core.PartInfo" />
    public class LayerPartInfo : PartInfo
    {
        /// <summary>
        /// Gets or sets the count of fragments in the part.
        /// </summary>
        public int FragmentCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether such a part is effectively
        /// not stored in the database. This is true when this information
        /// comes from a part layer definition only, without any corresponding
        /// part existing in the database. Some methods may return both
        /// existing and non-existing parts, when we want to provide a full
        /// list of layers to pick from, whether they are effectively present
        /// or not.
        /// </summary>
        public bool IsAbsent { get; set; }
    }
}
