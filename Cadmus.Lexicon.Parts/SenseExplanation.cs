using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A word sense explanation, built of words and/or a definition
    /// or translation.
    /// </summary>
    public sealed class SenseExplanation
    {
        /// <summary>
        /// Gets or sets the words.
        /// </summary>
        public List<string> Words { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SenseExplanation"/> class.
        /// </summary>
        public SenseExplanation()
        {
            Words = new List<string>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Words?.Count > 0) sb.Append(String.Join(", ", Words));

            if (!String.IsNullOrEmpty(Value))
            {
                if (sb.Length > 0) sb.Append(": ");
                sb.Append(Value);
            }

            return sb.ToString();
        }
    }
}
