using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive units counts part ("consistenze").
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>counts.K where K is a key</term>
    /// <description>value</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Tag("archive-counts")]
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
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            if (Counts?.Count == 0) return Array.Empty<DataPin>();

            return from p in Counts
                select CreateDataPin($"counts.{p.Key}",
                    p.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Counts?.Count == 0) return nameof(ArchiveCountsPart);

            return nameof(ArchiveCountsPart) + ": " +
                   string.Join(", ", from p in Counts
                       select $"{p.Key}={p.Value}");
        }
    }
}
