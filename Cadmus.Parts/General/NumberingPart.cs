using System.Collections.Generic;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Generic numbering part, providing the ordinal number representing the
    /// position of an item in the context of its "sibling" items, plus an
    /// optional arbitrarily defined number label.
    /// Tag: <c>it.vedph.numbering</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.numbering")]
    public sealed class NumberingPart : PartBase
    {
        /// <summary>
        /// Gets or sets the optional "number" assigned to an item in the context
        /// of its siblings. This can be any alphanumeric arbitrary string
        /// (e.g. <c>A.1.II.</c>).
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the ordinal number representing the position of the item
        /// in the context of its siblings.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the optional tag. This property can be used to provide
        /// several different numberings to the same sequence of items, each
        /// representing a different numbering criterion.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>ordinal</c>=tag + space + ordinal.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new[]
            {
                CreateDataPin("ordinal", $"{Tag ?? ""} {Ordinal}")
            };
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.String,
                    "ordinal",
                    "The value built from tag + space + ordinal.")
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
            return $"[Numbering] {Ordinal}: {Number}";
        }
    }
}
