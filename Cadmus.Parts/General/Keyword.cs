namespace Cadmus.Parts.General
{
    /// <summary>
    /// Keyword.
    /// </summary>
    /// <remarks>A keyword has an ISO 639-3 language ID and a value, and
    /// represents any relevant keyword linked to an item.</remarks>
    public class Keyword
    {
        /// <summary>
        /// Language (usually an ISO 639 3-letters code).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Keyword text value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Textual representation of this keyword.
        /// </summary>
        /// <returns>string in format <c>language: value</c></returns>
        public override string ToString()
        {
            return $"[{Language}] {Value}";
        }
    }
}
