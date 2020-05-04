using Cadmus.Core;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// Word phonetic and graphic form part.
    /// Tag: <c>net.fusisoft.lexicon.word-form</c>.
    /// </summary>
    [Tag("net.fusisoft.lexicon.word-form")]
    public sealed class WordFormPart : PartBase
    {
        /// <summary>
        /// Gets or sets the optional prelemma.
        /// </summary>
        public string Prelemma { get; set; }

        /// <summary>
        /// Gets or sets the lemma.
        /// </summary>
        public string Lemma { get; set; }

        /// <summary>
        /// Gets or sets the optional postlemma.
        /// </summary>
        public string Postlemma { get; set; }

        /// <summary>
        /// Gets or sets the optional homograph number (1-N).
        /// </summary>
        public int Homograph { get; set; }

        /// <summary>
        /// Gets or sets the pronunciation(s).
        /// </summary>
        public List<UsedWord> Pronunciations { get; set; }

        /// <summary>
        /// Gets or sets the hyphenation(s).
        /// </summary>
        public List<UsedWord> Hyphenations { get; set; }

        /// <summary>
        /// Gets or sets the variant(s).
        /// </summary>
        public List<VariantWord> Variants { get; set; }

        /// <summary>
        /// Gets or sets an optional orthographic note (e.g. "written with 2 L").
        /// </summary>
        public string OrthographicNote { get; set; }

        /// <summary>
        /// Gets or sets the link to the optional parent word.
        /// This can be used for derived words like e.g. it. "abatino" from "abate",
        /// for phrasal verbs like e.g. en. "get off" from "get".
        /// </summary>
        public WordLink Parent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordFormPart"/> class.
        /// </summary>
        public WordFormPart()
        {
            Pronunciations = new List<UsedWord>();
            Hyphenations = new List<UsedWord>();
            Variants = new List<VariantWord>();
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
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[WordForm]");

            if (!string.IsNullOrEmpty(Prelemma)) sb.Append(Prelemma).Append(' ');
            sb.Append(Lemma);
            if (!string.IsNullOrEmpty(Postlemma)) sb.Append(Postlemma).Append(' ');

            if (Homograph > 0) sb.AppendFormat(" ({0})", Homograph);

            return sb.ToString();
        }
    }
}
