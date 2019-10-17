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
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    [Tag("fr.chronology")]
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
        /// Gets or sets the date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        public IEnumerable<DataPin> GetDataPins()
        {
            if (Date == null)
                return Enumerable.Empty<DataPin>();

            return new[]
            {
                new DataPin
                {
                    Name = "date-value",
                    Value = Date.GetSortValue().ToString(CultureInfo.InvariantCulture)
                }
            };
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Chronology: {Location} {Date}";
        }
    }
}
