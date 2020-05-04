using Cadmus.Core;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Globalization;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Historical date part. This part just wraps a <see cref="HistoricalDate"/>.
    /// Tag: <c>net.fusisoft.historical-date</c>
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.historical-date")]
    public sealed class HistoricalDatePart : PartBase
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Pins: <c>date-value</c> with the date sort value or 0.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return new[]
            {
                CreateDataPin("date-value",
                    (Date?.GetSortValue() ?? 0).ToString(CultureInfo.InvariantCulture))
            };
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[HistoricalDate] {Date}";
        }
    }
}
