using Cadmus.Core;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// External bibliography entries part.
    /// Tag: <c>it.vedph.ext-bibliography</c>.
    /// </summary>
    [Tag("it.vedph.ext-bibliography")]
    public sealed class ExtBibliographyPart : PartBase
    {
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public List<ExtBibEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtBibliographyPart"/>
        /// class.
        /// </summary>
        public ExtBibliographyPart()
        {
            Entries = new List<ExtBibEntry>();
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tot-count</c> and a collection of pins with
        /// these keys: ....</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item)
        {
            DataPinBuilder builder = new DataPinBuilder(
                DataPinHelper.DefaultFilter);

            builder.Set("tot", Entries?.Count ?? 0, false);

            if (Entries?.Count > 0)
            {
                foreach (var entry in Entries)
                {
                    builder.AddValue("label", entry.Label,
                        filter: true, filterOptions: true);
                    builder.AddValue("tag", entry.Tag);
                }
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
                   "The total count of entries."),
                new DataPinDefinition(DataPinValueType.String,
                   "label",
                   "The list of labels.",
                   "Mf"),
                new DataPinDefinition(DataPinValueType.String,
                   "tag",
                   "The list of tags.",
                   "M")
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
            StringBuilder sb = new StringBuilder();

            sb.Append("[ExtBibliography]");
            if (Entries?.Count > 0)
            {
                sb.Append(' ');
                var groups = from e in Entries
                             group e by e.Tag into g
                             select g;
                int n = 0;
                foreach (var g in groups)
                {
                    if (++n > 1) sb.Append(", ");
                    sb.Append(g.Key).Append('=').Append(g.Count());
                }
            }

            return sb.ToString();
        }
    }
}
