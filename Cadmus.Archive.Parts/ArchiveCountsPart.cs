using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive units counts part ("consistenze").
    /// Tag: <c>it.vedph.archive-counts</c>.
    /// </summary>
    [Tag("it.vedph.archive-counts")]
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
        /// <returns>The pins: <c>tot-count</c> and a list of pins with keys:
        /// <c>c-KEY-count</c>.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Set("tot", Counts?.Count ?? 0, false);

            if (Counts?.Count > 0)
            {
                foreach (KeyValuePair<string, int> count in Counts)
                    builder.Set($"c-{count.Key}", count.Value, false);
            }

            return builder.Build(this);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "tot-count",
                    "The total count of counts."),
                new DataPinDefinition(DataPinValueType.Integer,
                    "c-{KEY}-count",
                    "The list of counts per key."),
            });
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
