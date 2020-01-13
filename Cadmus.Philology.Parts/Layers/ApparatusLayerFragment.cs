using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Critical apparatus layer fragment.
    /// Tag: <c>fr.net.fusisoft.apparatus</c>.
    /// </summary>
    /// <remarks>Any apparatus fragment may define a textual variant which
    /// should be replaced to the lemma according to their proposers, or any
    /// text which should be inserted before/after the lemma according to their
    /// proposers, or any other annotation related to the constitution of the
    /// text. According to these types, the entry is either a replacement, an
    /// addition, or just a note respectively:
    /// <list type="bullet">
    /// <item>
    ///	<term>replacement</term>
    ///	<description>: the entry has been proposed as a replacement for its
    ///	lemma by 1 or more authors (<c>Authors</c> property). Eventually
    ///	the variant can be <see cref="IsAccepted"/>, so that it should
    ///	replace the lemma in the output text. Only 1 variant in a set
    ///	can be accepted. The replacement variant is the most common type of
    ///	variant. 
    /// A special case for this variant type is the deletion by an editor:
    /// in this case, the variant text is zero (i.e. a null string: e.g.
    /// <c>in om. Crusius</c>).
    /// </description>
    /// </item>
    /// <item>
    ///	<term>addition</term>
    ///	<description>: the entry text has been proposed as an addition
    ///	before/after the lemma it refers to (e.g. <c>in</c> before
    ///	<c>domo</c>) by 1 or more authors (<see cref="Authors"/> property).
    ///	If this variant is accepted, the output text should insert the
    ///	variant text before or after the lemma with the proper diacritics.
    ///	</description>
    /// </item>
    /// <item>
    ///	<term>note</term>
    ///	<description>: any annotation strictly connected to the text
    ///	constitution (e.g. <c>dubitat Crusius an interpungendum sit</c>).
    ///	In such case, the <see cref="Value"/> and <see cref="IsAccepted"/>
    ///	properties have no meaning.
    ///	</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="T:Cadmus.Core.Layers.ITextLayerFragment" />
    [Tag("fr.net.fusisoft.apparatus")]
    public sealed class ApparatusLayerFragment : ITextLayerFragment
    {
        #region Properties
        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a simple
        /// token-based coordinates system (e.g. 1.2=second token of first block), or
        /// a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Variant type.
        /// </summary>
        public LemmaVariantType Type { get; set; }

        /// <summary>
        /// Lemma variant text value, can be zero (null or empty) for a
        /// deletion. When <see cref="Type"/> is <see cref="LemmaVariantType.Note"/>
        /// this property has no meaning as it's not applicable.
        /// </summary>
        /// <value>Variant text.</value>
        public string Value { get; set; }

        /// <summary>
        /// True if this variant has been accepted.
        /// </summary>
        /// <remarks>Only 1 variant per text word can be accepted.</remarks>
        public bool IsAccepted { get; set; }

        /// <summary>
        /// Authors of this variant.
        /// </summary>
        public HashSet<string> Authors { get; set; }

        /// <summary>
        /// An optional short note.
        /// </summary>
        public string Note { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ApparatusLayerFragment"/>
        /// class.
        /// </summary>
        public ApparatusLayerFragment()
        {
            Authors = new HashSet<string>();
        }

        /// <summary>
        /// Get all the pins exposed by the implementor.
        /// Pins: <c>fr.author</c>=author (0-N, 1 pin for each single author),
        /// sorted by author.
        /// </summary>
        /// <returns>pins</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            if (Authors?.Count > 0)
            {
                return from author in Authors.OrderBy(s => s)
                       select new DataPin
                       {
                           Name = PartBase.FR_PREFIX + "author",
                           Value = author
                       };
            }

            return Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"[Apparatus] {Location} ");

            switch (Type)
            {
                case LemmaVariantType.Replacement:
                    sb.Append(Value?.Length == 0 ? "del." : Value);
                    break;

                case LemmaVariantType.AdditionBefore:
                    sb.Append(Value).Append(" add.");
                    break;
                case LemmaVariantType.AdditionAfter:
                    goto case LemmaVariantType.AdditionBefore;
            }

            if (Authors != null) sb.Append(' ').Append(string.Join(", ", Authors));

            if (!string.IsNullOrEmpty(Note)) sb.Append(' ').Append(Note);
            if (IsAccepted) sb.Append(" (ok)");

            return sb.ToString();
        }
    }

    #region Constants
    /// <summary>
    /// Lemma variant type.
    /// </summary>
    public enum LemmaVariantType
    {
        /// <summary>variant should replace lemma</summary>
        Replacement = 0,

        /// <summary>variant should be added before lemma</summary>
        AdditionBefore,

        /// <summary>variant should be added after lemma</summary>
        AdditionAfter,

        /// <summary>any note to the text</summary>
        Note
    }
    #endregion
}
