﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive units counts part ("consistenze").
    /// Tag: <c>net.fusisoft.archive-counts</c>.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>count-K where K is a key</term>
    /// <description>value</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Tag("net.fusisoft.archive-counts")]
    public sealed class ArchiveCountsPart : PartBase
    {
        /// <summary>
        /// Gets or sets the counts.
        /// </summary>
        public Dictionary<string,int> Counts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveCountsPart"/> class.
        /// </summary>
        public ArchiveCountsPart()
        {
            Counts = new Dictionary<string, int>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Counts?.Count == 0) return Array.Empty<DataPin>();

            return from p in Counts
                select CreateDataPin($"count-{p.Key}",
                    p.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Counts?.Count == 0) return "[ArchiveCounts] 0";

            return $"[ArchiveCounts] {Counts.Count}: " +
                   string.Join(", ", from p in Counts
                                     select $"{p.Key}={p.Value}");
        }
    }
}
