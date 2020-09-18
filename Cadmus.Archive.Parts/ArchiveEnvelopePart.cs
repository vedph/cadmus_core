using System;
using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// An envelope including any number of items. This is not an archival unit
    /// entering the archive hierarchy, but only a physical container. Note that
    /// an envelope can never include other envelopes.
    /// Tag: <c>it.vedph.archive-envelope</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.archive-envelope")]
    public sealed class ArchiveEnvelopePart : PartBase
    {
        /// <summary>
        /// Gets or sets the envelope number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the level the envelope rests on.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the IDs of the items included in this envelope.
        /// </summary>
        public List<string> ItemIds { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>envelope-nr</term>
        /// <description>envelope number</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new[]
            {
                CreateDataPin("envelope-nr",
                    Number.ToString(CultureInfo.InvariantCulture))
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
                new DataPinDefinition(DataPinValueType.Integer,
                    "envelope-nr",
                    "The envelope number.")
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
            return $"[ArchiveEnvelope] @{Level} {Number}";
        }
    }
}
