using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core.Blocks;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// An envelope including any number of items. This is not an archival unit entering the
    /// archive hierarchy, but only a physical container. Note that an envelope can never
    /// include other envelopes.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// 	<item>
    /// 		<term>archive-envelope-nr</term>
    /// 		<description>envelope number</description>
    /// 	</item>
    /// </list>
    /// </remarks>
    /// <seealso cref="Cadmus.Core.Blocks.PartBase" />
    [Tag("archive-envelope")]
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
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return new[]
            {
                CreateDataPin("archive-envelope-nr",
                    Number.ToString(CultureInfo.InvariantCulture))
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
            return $"{nameof(ArchiveEnvelopePart)}: @{Level} {Number}";
        }
    }
}
