using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Date assigned to any archive's content.
    /// Tag: <c>net.fusisoft.archive-date</c>.
    /// </summary>
    [Tag("net.fusisoft.archive-date")]
    public sealed class ArchiveDatePart : PartBase
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public ArchiveDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <term>date-sort-value</term>
        /// <description>sort value</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new[]
            {
                CreateDataPin("date-sort-value",
                    Date?.SortValue.ToString(CultureInfo.InvariantCulture))
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
            return $"[ArchiveDate] {Date}";
        }
    }
}
