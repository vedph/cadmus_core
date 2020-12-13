using System;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A short example text for the usage of a word.
    /// </summary>
    public sealed class WordExample
    {
        /// <summary>
        /// Gets or sets the example value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the example optional explanation or translation.
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets an optional tip for this example.
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// Gets or sets the optional source of this example
        /// (when it is a citation).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            // value
            StringBuilder sb = new StringBuilder(Value);

            // usage
            if (!string.IsNullOrEmpty(Usage))
                sb.Append(" [").Append(Usage).Append("]");

            // tip
            if (!string.IsNullOrEmpty(Tip))
                sb.Append(" (").Append(Tip).Append(")");

            // source
            if (!string.IsNullOrEmpty(Source))
                sb.Append(" <").Append(Source).Append(">");

            // explanation
            if (!string.IsNullOrEmpty(Explanation))
                sb.Append(" = ").Append(Explanation);

            return sb.ToString();
        }
    }
}
