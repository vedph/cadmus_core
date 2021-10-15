using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Cadmus.Refs.Bricks;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// External IDs part.
    /// <para>Tag: <c>it.vedph.external-ids</c>.</para>
    /// </summary>
    [Tag("it.vedph.external-ids")]
    public sealed class ExternalIdsPart : PartBase
    {
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public List<ExternalId> Ids { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalIdsPart"/> class.
        /// </summary>
        public ExternalIdsPart()
        {
            Ids = new List<ExternalId>();
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tot-count</c> and a collection of pins with
        /// the <c>id</c> key.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item)
        {
            DataPinBuilder builder = new DataPinBuilder();

            builder.Set("tot", Ids?.Count ?? 0, false);

            if (Ids?.Count > 0)
                builder.AddValues("id", Ids.Select(i => i.Value));

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
               "id",
               "The IDs.",
               "M")
        });
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[ExternalIds]");

            if (Ids?.Count > 0)
            {
                sb.Append(' ');
                int n = 0;
                foreach (var entry in Ids)
                {
                    if (++n > 3) break;
                    if (n > 1) sb.Append("; ");
                    sb.Append(entry);
                }
                if (Ids.Count > 3)
                    sb.Append("...(").Append(Ids.Count).Append(')');
            }

            return sb.ToString();
        }
    }
}
