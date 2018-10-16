namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A link to another word.
    /// </summary>
    public sealed class WordLink
    {
        /// <summary>
        /// Gets or sets the link type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the target identifier.
        /// The value of this identifier varies according to the linking schema
        /// used. For instance, it might refer to a word by its lemma, homograph
        /// number, and optionally sense ID; or just refer to a word by its ID.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets or sets the target word label.
        /// </summary>
        public string TargetLabel { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{Type}] => {TargetLabel} ({TargetId})";
        }
    }
}
