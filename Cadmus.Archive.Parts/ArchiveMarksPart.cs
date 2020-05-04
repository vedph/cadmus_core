using System;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Archive marks part ("segnature").
    /// Tag: <c>net.fusisoft.archive-marks</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.archive-marks")]
    public sealed class ArchiveMarksPart : PartBase
    {
        /// <summary>
        /// Gets or sets the optional tag. This property can be used to provide
        /// several different marks to the same item, each reflecting a different
        /// classification.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the marks.
        /// </summary>
        public List<string> Marks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveMarksPart"/> class.
        /// </summary>
        public ArchiveMarksPart()
        {
            Marks = new List<string>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>mark (multiple)</term>
        /// <description>value</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Marks?.Count == 0) return Array.Empty<DataPin>();

            return from s in Marks
                select CreateDataPin("mark", s);
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "[ArchiveMarks] " +
                (Marks?.Count == 0
                ? "0"
                : $"{Marks.Count} [{Tag ?? ""}]: {string.Join(", ", Marks)}");
        }
    }
}
