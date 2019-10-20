using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A phrase involving a special usage of a word.
    /// Tag: <c>net.fusisoft.lexicon.word-phrase</c>.
    /// </summary>
    [Tag("net.fusisoft.lexicon.word-phrase")]
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
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            // TODO pins
            return new DataPin[0];
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
