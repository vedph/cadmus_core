namespace Cadmus.Parts.General
{
    /// <summary>
    /// An entry for the <see cref="ExtBibliographyPart"/>.
    /// </summary>
    /// <remarks>This is a generic model which does not imply any specific
    /// data storage format. Each entry just has a string ID for the work,
    /// a human-readable label for it, and a general purpose payload
    /// string to carry additional information which might be useful in the
    /// context of the external database.
    /// </remarks>
    public class ExtBibEntry
    {
        /// <summary>
        /// Gets or sets the identifier for this entry.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a human-friendly label for this entry.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the optional payload.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets an optional tag assigned to this entry.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets an optional note.
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
            return $"{Id}: {Label}";
        }
    }
}
