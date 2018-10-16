namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// An archive date, consisting of one or two <see cref="ArchiveDatePoint"/>'s.
    /// </summary>
    /// <remarks>A date can have both points defined, when it's a point 
    /// (<see cref="Min"/> = <see cref="Max"/>) or a defined range; one of the
    /// two points may be null when it represents a terminus post (<see cref="Max"/>
    /// is null) or ante (<see cref="Min"/> is null).</remarks>
    public sealed class ArchiveDate
    {
        private const int TERMINUS_DELTA = 10 * 365;

        #region Properties
        /// <summary>
        /// Gets the min point.
        /// </summary>
        public ArchiveDatePoint Min { get; set; }

        /// <summary>
        /// Gets the max point.
        /// </summary>
        public ArchiveDatePoint Max { get; set; }

        /// <summary>
        /// Optional free text annotation about this date.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets a value indicating whether this date is null (both points are null).
        /// </summary>
        public bool IsNull => Min == null && Max == null;

        /// <summary>
        /// Gets a value indicating whether this instance is a range.
        /// </summary>
        public bool IsRange
        {
            get
            {
                if (Min == null && Max == null) return false;
                if (Min == null || Max == null) return true;
                return Min.SortValue != Max.SortValue;
            }
        }

        /// <summary>
        /// Gets the sort value for this date.
        /// </summary>
        public int SortValue
        {
            get
            {
                // null
                if (Min == null && Max == null) return 0;

                // point
                if (!IsRange && Min != null) return Min.SortValue;

                // defined range
                if (Min != null && Max != null)
                    return (Min.SortValue + Max.SortValue) >> 1;

                // undefined range
                return Min?.SortValue + TERMINUS_DELTA ?? Max.SortValue - TERMINUS_DELTA;
            }
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Min == null) return "";

            if (IsRange) return $"{Min} - {Max}";

            if (Min != null && Max != null) return Min.ToString();
            return Max == null ? $"post {Min}" : $"ante {Max}";
        }
    }
}
