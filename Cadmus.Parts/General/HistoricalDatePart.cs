using Cadmus.Bricks;
using Cadmus.Core;
using Cadmus.Refs.Bricks;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Globalization;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Historical date part. This part just wraps a <see cref="HistoricalDate"/>.
    /// Tag: <c>it.vedph.historical-date</c>
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.historical-date")]
    public sealed class HistoricalDatePart : PartBase
    {
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Gets or sets the short documental references connected to this
        /// datation.
        /// </summary>
        public List<DocReference> References { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoricalDatePart"/>
        /// class.
        /// </summary>
        public HistoricalDatePart()
        {
            References = new List<DocReference>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins: <c>date-value</c> with the date sort value or 0;
        /// <c>hint</c> with the date's hint(s) when present (filtered, with digits).
        /// </returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder(
                DataPinHelper.DefaultFilter);

            builder.AddValue("date-value",
                (Date?.GetSortValue() ?? 0).ToString(CultureInfo.InvariantCulture));

            if (Date != null)
            {
                switch (Date.GetDateType())
                {
                    case HistoricalDateType.Range:
                        if (!string.IsNullOrEmpty(Date.A.Hint))
                        {
                            builder.AddValue("hint", Date.A.Hint,
                                filter: true, filterOptions: true);
                        }
                        if (!string.IsNullOrEmpty(Date.B.Hint))
                        {
                            builder.AddValue("hint", Date.B.Hint,
                                filter: true, filterOptions: true);
                        }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(Date.A.Hint))
                        {
                            builder.AddValue("hint", Date.A.Hint,
                                filter: true, filterOptions: true);
                        }
                        break;
                }
            }

            return builder.Build(this);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Decimal,
                    "date-value",
                    "The sortable date value (0 if undefined)."),
                new DataPinDefinition(DataPinValueType.String,
                    "hint",
                    "The list of date's hints, if any.",
                    "Mf")
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
            return $"[HistoricalDate] {Date}";
        }
    }
}
