namespace Cadmus.Parts.General
{
    /// <summary>
    /// A physical dimension used in <see cref="PhysicalSize"/>.
    /// </summary>
    public class PhysicalDimension
    {
        /// <summary>
        /// Gets or sets an optional tag used to categorize or group several
        /// dimensions.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the measurement unit.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{Tag}] {Value} {Unit}";
        }
    }
}
