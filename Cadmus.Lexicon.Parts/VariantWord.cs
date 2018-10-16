using System;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A word representing the variant of another one.
    /// </summary>
    /// <seealso cref="Cadmus.Lexicon.Parts.UsedWord" />
    public class VariantWord : UsedWord
    {
        /// <summary>
        /// Gets or sets the optional prelemma.
        /// </summary>
        public string Prelemma { get; set; }

        /// <summary>
        /// Gets or sets the optional postlemma.
        /// </summary>
        public string Postlemma { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(Prelemma)) sb.Append(Prelemma).Append(" ");
            sb.Append(Value);
            if (!String.IsNullOrEmpty(Postlemma)) sb.Append(" ").Append(Postlemma);
            if (!String.IsNullOrEmpty(Usage)) sb.Append(" (").Append(Usage).Append(")");

            return sb.ToString();
        }
    }
}
