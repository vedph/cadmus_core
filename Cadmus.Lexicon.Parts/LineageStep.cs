using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A single step in an etymologic lineage.
    /// </summary>
    public sealed class LineageStep
    {
        /// <summary>
        /// Gets or sets the language ID (ISO 639-3).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the optional usage limit.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets the form value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the optional target identifier
        /// when the form refers to an entity existing in some
        /// database. For instance, the Italian word "abate"
        /// might reference the Latin word "abbas" in a Latin
        /// database.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets or sets the confidence rank.
        /// </summary>
        public int ConfidenceRank { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this step indirectly
        /// refers to its form, which typically happens when quoting
        /// a paradigmatic word form standing from a trivally derived one,
        /// which is the true ancestor of the discussed word.
        /// </summary>
        public bool IsIndirect { get; set; }

        /// <summary>
        /// Gets or sets the sense of the form.
        /// </summary>
        public string Sense { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // indirect
            if (!string.IsNullOrEmpty(Usage)) sb.Append("from ");

            // language
            if (!string.IsNullOrEmpty(Language))
                sb.Append(Language).Append(" ");

            // value
            sb.Append(Value).Append(" ");

            // usage
            if (!string.IsNullOrEmpty(Usage))
                sb.Append(" (").Append(Usage).Append(") ");

            // confidence
            if (ConfidenceRank != 0)
                sb.Append(" [").Append(Usage).Append("] ");

            // sense
            if (!string.IsNullOrEmpty(Sense))
                sb.Append(": ").Append(Sense);

            return sb.ToString();
        }
    }
}
