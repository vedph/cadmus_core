namespace Cadmus.Archive.Parts
{
    /// <summary>
    /// An archive date, consisting of one or two <see cref="ArchiveDatePoint"/>'s.
    /// </summary>
    /// <remarks>A date can have both points defined, when it's a point 
    /// (<see cref="A"/> = <see cref="B"/>) or a defined range; one of the
    /// two points may be null when it represents a terminus post (<see cref="B"/>
    /// is null) or ante (<see cref="A"/> is null).</remarks>
    public sealed class ArchiveDate
    {
        private const int TERMINUS_DELTA = 10 * 365;

        #region Properties
        /// <summary>
        /// Gets the A point.
        /// </summary>
        public ArchiveDatePoint A { get; set; }

        /// <summary>
        /// Gets the B point.
        /// </summary>
        public ArchiveDatePoint B { get; set; }

        /// <summary>
        /// Optional free text annotation about this date.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets a value indicating whether this date is null (both points are
        /// null).
        /// </summary>
        public bool IsNull => A == null && B == null;

        /// <summary>
        /// Gets a value indicating whether this instance is a range.
        /// </summary>
        public bool IsRange
        {
            get
            {
                if (A == null && B == null) return false;
                if (A == null || B == null) return true;
                return A.SortValue != B.SortValue;
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
                if (A == null && B == null) return 0;

                // point
                if (!IsRange && A != null) return A.SortValue;

                // defined range
                if (A != null && B != null)
                    return (A.SortValue + B.SortValue) >> 1;

                // undefined range
                return A?.SortValue + TERMINUS_DELTA ?? B.SortValue - TERMINUS_DELTA;
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
            if (A == null) return "";

            if (IsRange) return $"{A} - {B}";

            if (A != null && B != null) return A.ToString();
            return B == null ? $"post {A}" : $"ante {B}";
        }
    }
}
