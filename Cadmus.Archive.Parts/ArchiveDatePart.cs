using System.Collections.Generic;
using System.Globalization;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// Date assigned to any archive's content.
    /// </summary>
    /// <remarks>
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// 	<item>
    /// 		<term>date-sort-value</term>
    /// 		<description>sort value</description>
    /// 	</item>
    /// </list>
    /// </remarks>
    [Tag("archive-date")]
    public sealed class ArchiveDatePart : PartBase
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public ArchiveDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
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
            return $"{nameof(ArchiveDatePart)}: {Date}";
        }
    }
}
