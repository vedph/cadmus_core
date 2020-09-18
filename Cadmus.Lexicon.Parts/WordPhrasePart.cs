using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A phrase involving a special usage of a word.
    /// Tag: <c>it.vedph.lexicon.word-phrase</c>.
    /// </summary>
    [Tag("it.vedph.lexicon.word-phrase")]
    public sealed class WordPhrasePart : PartBase
    {
        /// <summary>
        /// Gets or sets the phrase rank.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the usage limit.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets the phrase value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the example optional explanation or translation.
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// Gets or sets an optional tip for this example.
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// Gets or sets the links to all the relevant words in this phrase.
        /// </summary>
        public List<WordLink> Links { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordPhrasePart"/> class.
        /// </summary>
        public WordPhrasePart()
        {
            Links = new List<WordLink>();
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            // TODO pins
            return new DataPin[0];
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            // TODO pins definitions
            return new List<DataPinDefinition>();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[WordPhrase] ");

            // rank
            sb.Append("[").Append(Rank).Append("] ");

            // usage
            if (!String.IsNullOrEmpty(Usage))
                sb.Append(" (").Append(Usage).Append(") ");

            // value
            sb.Append(Value);

            // tip
            if (!String.IsNullOrEmpty(Tip))
                sb.Append(" (").Append(Tip).Append(") ");

            // explanation
            if (!String.IsNullOrEmpty(Explanation))
                sb.Append(" = ").Append(Explanation);

            return sb.ToString();
        }
    }
}
