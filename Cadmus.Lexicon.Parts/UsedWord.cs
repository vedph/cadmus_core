using System;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A word with an usage limit.
    /// </summary>
    public class UsedWord
    {
        /// <summary>
        /// Gets or sets the usage limit.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets the word value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.IsNullOrEmpty(Usage)?
                Value :
                $"{Value} ({Usage})";
        }
    }
}
