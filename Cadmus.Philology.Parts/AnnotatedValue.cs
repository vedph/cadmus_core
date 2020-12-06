namespace Cadmus.Philology.Parts
{
    /// <summary>
    /// A general-purpose string value with an optional annotation.
    /// </summary>
    public class AnnotatedValue
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the optional note.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Note) ? Value : $"{Value} ({Note})";
        }
    }
}
