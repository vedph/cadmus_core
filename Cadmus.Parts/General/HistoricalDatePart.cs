using Cadmus.Core.Blocks;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Globalization;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Historical date part. This part just wraps a <see cref="HistoricalDate"/>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("historical-date")]
    public sealed class HistoricalDatePart : PartBase
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            return new[]
            {
                CreateDataPin("date-sort-value",
                    Date?.GetSortValue().ToString(CultureInfo.InvariantCulture))
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
            return $"{nameof(HistoricalDatePart)}: {Date}";
        }
    }
}
