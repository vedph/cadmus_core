using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Chronology layer fragment: a chronological indication linked to a
    /// specific portion of text.
    /// Tag: <c>fr.it.vedph.chronology</c>.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.it.vedph.chronology")]
    public sealed class ChronologyLayerFragment : ITextLayerFragment
    {
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a
        /// simple token-based coordinates system (e.g. 1.2=second token of
        /// first block), or a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the tag, representing some sort of classification for
        /// the chronological indication (e.g. battle, treatise, etc.).
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the label (e.g. "battle of Marathon").
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets an external event ID. This can be used to connect the
        /// datation in this fragment with a specific event in an external
        /// resource, e.g. an ontology.
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// When the date is specified, the pins are <c>fr.date-value</c>,
        /// and eventually <c>fr.tag</c> and <c>fr.event-id</c>.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Date == null) return Enumerable.Empty<DataPin>();

            List<DataPin> pins = new List<DataPin>
            {
                new DataPin
                {
                    Name = PartBase.FR_PREFIX + "date-value",
                    Value = Date.GetSortValue().ToString(CultureInfo.InvariantCulture)
                }
            };

            if (Tag != null)
            {
                pins.Add(new DataPin
                {
                    Name = PartBase.FR_PREFIX + "tag",
                    Value = Tag
                });
            }

            if (!string.IsNullOrEmpty(EventId))
            {
                pins.Add(new DataPin
                {
                    Name = PartBase.FR_PREFIX + "event-id",
                    Value = EventId
                });
            }

            return pins;
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Decimal,
                    PartBase.FR_PREFIX + "date-value",
                    "The sortable date value (0=undefined)."),
                new DataPinDefinition(DataPinValueType.Decimal,
                    PartBase.FR_PREFIX + "tag",
                    "The tag, if any."),
                new DataPinDefinition(DataPinValueType.Decimal,
                    PartBase.FR_PREFIX + "event-id",
                    "The event ID, if any.")
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
            return $"[Chronology] {Location} "
                + (Tag != null? $"({Tag}) " : "")
                + Date;
        }
    }
}
