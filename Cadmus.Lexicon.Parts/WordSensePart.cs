using Cadmus.Core;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// Word sense part.
    /// Tag: <c>it.vedph.lexicon.word-sense</c>.
    /// </summary>
    [Tag("it.vedph.lexicon.word-sense")]
    public sealed class WordSensePart : PartBase
    {
        /// <summary>
        /// Gets or sets the identifier of this sense, unique only among the senses
        /// of the same word. This might be a number, or a short mnemonic key.
        /// </summary>
        public string SenseId { get; set; }

        /// <summary>
        /// Gets or sets the rank for this sense.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the part of speech (POS) for this sense.
        /// </summary>
        public string Pos { get; set; }

        /// <summary>
        /// Gets or sets the explanation.
        /// </summary>
        public SenseExplanation Explanation { get; set; }

        /// <summary>
        /// Gets or sets the optional date.
        /// </summary>
        public HistoricalDate Date { get; set; }

        /// <summary>
        /// Gets or sets the usage.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets an optional free textual tip to better specify
        /// the sense meaning.
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// Gets or sets the links to other words.
        /// </summary>
        public List<WordLink> Links { get; set; }

        /// <summary>
        /// Gets or sets the examples for this sense.
        /// </summary>
        public List<WordExample> Examples { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordSensePart"/> class.
        /// </summary>
        public WordSensePart()
        {
            Explanation = new SenseExplanation();
            Links = new List<WordLink>();
            Examples = new List<WordExample>();
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
            StringBuilder sb = new StringBuilder("[WordSense] ");

            sb.Append($"#{SenseId} | {Rank} {Pos}: {Explanation}");

            if (!string.IsNullOrEmpty(Usage))
                sb.Append(" [").Append(Usage).Append("]");

            if (!string.IsNullOrEmpty(Tip))
                sb.Append(" (").Append(Tip).Append(")");

            if (Examples?.Count > 0)
                sb.Append(" | examples: ").Append(Examples.Count);

            if (Links?.Count > 0)
                sb.Append(" | links: ").Append(Links.Count);

            return sb.ToString();
        }
    }
}
