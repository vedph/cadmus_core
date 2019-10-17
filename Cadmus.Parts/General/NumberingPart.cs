using System.Collections.Generic;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Generic numbering part, providing the ordinal number representing the
    /// position of an item in the context of its "sibling" items, plus an
    /// optional arbitrarily defined number label.
    /// Tag: <c>net.fusisoft.numbering</c>.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>ordinal</term>
    /// <description><see cref="Tag"/> + space + <see cref="Ordinal"/></description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.numbering")]
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
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return new[]
            {
                CreateDataPin("ordinal", $"{Tag ?? ""} {Ordinal}")
            };
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(NumberingPart)}: {Ordinal}: {Number}";
        }
    }
}
