using System;
using System.Collections.Generic;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Philology.Parts.Layers
{
    /// <summary>
    /// Epigraphical letters ligature: ligature ("", abbreviation: <c>L</c>),
    /// inversion ("inversione", <c>I</c>), overlap ("sovrapposizione", <c>O</c>),
    /// replacement ("sostituzione", <c>R</c>), graft ("innesto", <c>G</c>),
    /// inclusion ("inclusione", <c>N</c>), connection ("nesso", <c>C</c>),
    /// complex (<c>X</c>) (see Manzella 1987 149-151).
    /// Tag: <c>fr.net.fusisoft.ligature</c>.
    /// </summary>
    /// <remarks>This part defines all the essential graphical connection types
    /// occurring among letters.
    /// The letters can belong to the same word or (rarely) to different words.
    /// A single item defines a specific connection type including 2 or more
    /// consecutive letters.
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>Layer.Ligature.Type</term>
    /// <description>a character representing the ligature type.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Tag("fr.net.fusisoft.ligature")]
    public sealed class LigatureLayerFragment : ITextLayerFragment
    {
        private const string ABBREVIATIONS = "LIORGNCX";

        /// <summary>
        /// Gets or sets the location of this fragment.
        /// </summary>
        /// <remarks>
        /// The location can be expressed in different ways according to the
        /// text coordinates system being adopted. For instance, it might be a
        /// simple token-based coordinates system (e.g. 1.2=second token of
        /// first block), or a more complex system like an XPath expression.
        /// </remarks>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the ligature type.
        /// </summary>
        public LigatureType Type { get; set; }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Pins: <c>ligature</c>=ligature type letter (one letter picked
        /// from <c>LIORGNCX</c>, each representing a ligature type;
        /// see <see cref="LigatureType"/>).
        /// </summary>
        /// <returns>Data pins.</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            return new[]
            {
                new DataPin
                {
                    Name = PartBase.FR_PREFIX + "ligature",
                    Value = new string(LigatureTypeToAbbreviation(Type), 1)
                }
            };
        }

        #region Abbreviations
        /// <summary>
        /// Get a 1-letter abbreviation corresponding to the specified ligature
        /// type.
        /// </summary>
        /// <param name="type">ligature type</param>
        /// <returns>letter</returns>
        public static char LigatureTypeToAbbreviation(LigatureType type)
        {
            return ABBREVIATIONS[(int)type];
        }

        /// <summary>
        /// Get the ligature type corresponding to the specified 1-letter
        /// abbreviation.
        /// </summary>
        /// <param name="abbreviation">abbreviation</param>
        /// <returns>ligature type</returns>
        public static LigatureType AbbreviationToLigatureType(char abbreviation)
        {
            int i = ABBREVIATIONS.IndexOf(abbreviation);
            if (i == -1) return LigatureType.None;

            return (LigatureType)i;
        }
        #endregion

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Ligature] {Location} {Enum.GetName(typeof(LigatureType), Type)}";
        }
    }

    #region Constants
    /// <summary>
    /// Letters graphical connection type.
    /// </summary>
    /// <remarks>Cfr. Manzella 1987 149-151.</remarks>
    public enum LigatureType
    {
        /// <summary>No ligature.</summary>
        None = 0,

        /// <summary>standard ligature: a shared trait among several letters.
        /// </summary>
        Ligature,

        /// <summary>standard nexus with inversion of a letter for graphical
        /// commodity: e.g.<c>PR</c> where P is horizontally flipped so that
        /// its vertical trait can be shared with <c>R</c>.</summary>
        Inversion,

        /// <summary>overlap: letters parts overlap like <c>DO</c> where the
        /// right part of <c>D</c> and the left part of <c>O</c> cross each other,
        /// or <c>AV</c> where <c>V</c> is vertically flipped and overlapped
        /// to <c>A</c>.</summary>
        Overlap,

        /// <summary>replacement: a trait is shared with another letter replacing
        /// the other letter's trait which would be graphically unfit: e.g.
        /// <c>OE</c> where the curve of <c>O</c> also hosts the horizontal traits
        /// of <c>E</c>, replacing the <c>E</c>'s vertical trait.</summary>
        Replacement,

        /// <summary>graft: a letter continues the tracing of another letter,
        /// like <c>I</c> which in <c>CI</c> continues the topright terminal
        /// point of <c>C</c> somewhat like a vertically flipped <c>G</c>.
        /// </summary>
        Graft,

        /// <summary>inclusion: improperly considered a nexus: a letter is
        /// smaller and placed inside another letter, like <c>O</c> in <c>C</c>
        /// and <c>I</c> in <c>G</c> of the word <c>CONIUGI</c>.</summary>
        Inclusion,

        /// <summary>connection: letters are connected via additional traits
        /// not belonging to any letter
        /// shape</summary>
        Connection,

        /// <summary>complex ligature: any complex combination of the other
        /// types.</summary>
        Complex
    }
    #endregion
}
