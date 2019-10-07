using Fusi.Tools;
using System;
using System.Text;

namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// A point in time in an archive date.
    /// </summary>
    public sealed class ArchiveDatePoint
    {
        private static readonly string[] _approxPrefixes = {
            "",
            "ca. ",
            "inizio ",
            "I metà ",
            "metà ",
            "II metà ",
            "fine "
        };

        #region Properties
        /// <summary>
        /// Gets or sets the value, which corresponds to the year (e.g. 1923),
        /// decade (e.g. 191=1910), or century (e.g. 20=XX sec.). A value of
        /// 0=undefined date.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the type of this date point's
        /// <see cref="Value"/>.
        /// </summary>
        public DateValueType ValueType { get; set; }

        /// <summary>
        /// Gets or sets the month (0=undefined).
        /// </summary>
        public short Month { get; set; }

        /// <summary>
        /// Gets or sets the day (0=undefined).
        /// </summary>
        public short Day { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the year is inferred.
        /// </summary>
        public bool IsYearInferred { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the month is inferred.
        /// </summary>
        public bool IsMonthInferred { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the day is inferred.
        /// </summary>
        public bool IsDayInferred { get; set; }

        /// <summary>
        /// Gets or sets the level of approximation of this date point.
        /// </summary>
        public ApproximationType Approximation { get; set; }

        /// <summary>
        /// Gets the sort value for this point.
        /// </summary>
        public int SortValue => GetSortValue();
        #endregion

        private int GetSortValue()
        {
            int year;

            switch (ValueType)
            {
                case DateValueType.Century:
                    // century year defaults at its middle: e.g. 1950 from XX
                    year = ((Math.Abs(Value) - 1) * 100) + 50;

                    // century approximation
                    switch (Approximation)
                    {
                        case ApproximationType.Beginning:
                            year -= 40;    // e.g. 1910
                            break;
                        case ApproximationType.FirstHalf:
                            year -= 25;    // e.g. 1925
                            break;
                        case ApproximationType.SecondHalf:
                            year += 25;    // e.g. 1975
                            break;
                        case ApproximationType.End:
                            year += 40;    // e.g. 1990
                            break;
                    }
                    // restore sign
                    if (Value < 0) year = -year;
                    break;

                case DateValueType.Decade:
                    year = (Value * 10) + 5; // e.g. 1915
                    break;

                default:
                    year = Value;
                    break;
            }

            return (year * 365) + (Month * 30) + Day;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>new instance</returns>
        public ArchiveDatePoint Clone()
        {
            return new ArchiveDatePoint
            {
                Value = Value,
                ValueType = ValueType,
                Month = Month,
                Day = Day,
                IsYearInferred = IsYearInferred,
                IsMonthInferred = IsMonthInferred,
                IsDayInferred = IsDayInferred,
                Approximation = Approximation
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
            StringBuilder sb = new StringBuilder();

            switch (ValueType)
            {
                case DateValueType.Century:
                    sb.Append(_approxPrefixes[(int)Approximation]);
                    sb.Append("sec. ");
                    int n = Math.Abs(Value);
                    sb.Append(RomanNumber.ToRoman(n));
                    if (Value < 0) sb.Append(" a.C.");
                    break;

                case DateValueType.Decade:
                    sb.Append("anni '").Append(Value * 10);
                    break;

                default:
                    if (Approximation == ApproximationType.About)
                        sb.Append(_approxPrefixes[(int)Approximation]);
                    if (Value != 0)
                        sb.Append(IsYearInferred ? $"[{Value}]" : $"{Value}");
                    if (Month != 0)
                        sb.Append(IsMonthInferred ? $"-[{Month}]" : $"-{Month}");
                    if (Day != 0)
                        sb.Append(IsDayInferred ? $"-[{Day}]" : $"-{Day}");
                    break;
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// The type of <see cref="ArchiveDatePoint.Value"/>.
    /// </summary>
    public enum DateValueType
    {
        /// <summary>Year</summary>
        Year = 0,

        /// <summary>Decade</summary>
        Decade,

        /// <summary>Century</summary>
        Century
    }

    /// <summary>
    /// Date approximation type for <see cref="ArchiveDatePoint"/>.
    /// </summary>
    public enum ApproximationType
    {
        /// <summary>Not approximated. The date is exact.</summary>
        None = 0,

        /// <summary>Approximated in an unspecified way (about).</summary>
        About,

        /// <summary>At the beginning of the specified century.</summary>
        Beginning,

        /// <summary>In the first half of the specified century.</summary>
        FirstHalf,

        /// <summary>At the middle of the specified century.</summary>
        Mid,

        /// <summary>In the second half of the specified century.</summary>
        SecondHalf,

        /// <summary>At the end of the specified century.</summary>
        End
    }
}
