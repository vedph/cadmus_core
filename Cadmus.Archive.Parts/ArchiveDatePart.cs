using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Date assigned to any archive's content.
    /// Tag: <c>it.vedph.archive-date</c>.
    /// </summary>
    [Tag("it.vedph.archive-date")]
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
        /// <term>date-value</term>
        /// <description>sort value</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new[]
            {
                CreateDataPin("date-value",
                    Date?.SortValue.ToString(CultureInfo.InvariantCulture))
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
                    "date-value",
                    "The sortable date value.")
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
            return $"[ArchiveDate] {Date}";
        }
    }
}
