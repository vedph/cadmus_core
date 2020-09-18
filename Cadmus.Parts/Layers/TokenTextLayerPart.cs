using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.General;
using Fusi.Tools.Config;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Text layer part class, based on token-referenced text.
    /// Tag: <c>it.vedph.token-text-layer</c>.
    /// </summary>
    /// <remarks>This class represents any text layer part using token-based
    /// coordinates. The text layer item part is just a wrapper for a collection
    /// of text layer fragments, and adds no other piece of data to the part itself.
    /// <para>
    /// A text layer part is like any other ordinary part, and derives
    /// from the same base class; its only peculiarity is that it just contains
    /// a collection of <see cref="ITextLayerFragment"/>-derived fragments,
    /// and exposes some utility methods to deal with them (e.g. adding a
    /// fragment, or getting all the fragments at the specified location).
    /// </para>
    /// <para>
    /// As a consequence, the pins exposed by this part is just the collection
    /// of all the pins exposed by its fragments. Also, layer parts always have
    /// their role ID equal to their fragments type ID; this effectively is the
    /// role played by this generic layer part in an item, as determined by the
    /// type of its fragments.
    /// </para>
    /// </remarks>
    [Tag("it.vedph.token-text-layer")]
    public sealed class TokenTextLayerPart<TFragment> : YXLayerPartBase<TFragment>
        where TFragment : ITextLayerFragment, new()
    {
        /// <summary>
        /// Gets the text at the specified location from the specified
        /// base text part.
        /// </summary>
        /// <param name="baseTextPart">The base text part to get text from.</param>
        /// <param name="location">The location.</param>
        /// <returns>The text, or null if location is invalid.</returns>
        /// <exception cref="ArgumentNullException">baseTextPart or location
        /// </exception>
        public override string GetTextAt(IPart baseTextPart, string location)
        {
            if (baseTextPart == null)
                throw new ArgumentNullException(nameof(baseTextPart));
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (!(baseTextPart is TokenTextPart textPart)) return null;

            // parse
            TokenTextLocation loc = TokenTextLocation.Parse(location);
            string[] tokens;
            int aLineIndex = loc.A.Y - 1;
            if (aLineIndex < 0 || aLineIndex >= textPart.Lines.Count)
                return null;

            // range
            if (loc.IsRange)
            {
                // tokens range
                StringBuilder sb = new StringBuilder();

                int bLineIndex = loc.B.Y - 1;
                if (bLineIndex < aLineIndex || bLineIndex >= textPart.Lines.Count)
                    return null;

                int aTokenIndex = loc.A.X - 1;
                int bTokenIndex = loc.B.X - 1;

                // corner case: same line for A-B
                if (aLineIndex == bLineIndex)
                {
                    // defensive
                    if (bTokenIndex < aTokenIndex) return null;

                    tokens = textPart.Lines[aLineIndex].GetTokens();
                    if (aTokenIndex < 0 || aTokenIndex >= tokens.Length)
                        return null;
                    return string.Join(" ",
                        tokens.Skip(aTokenIndex).Take(bTokenIndex + 1 - aTokenIndex));
                }

                // first line
                // A.X
                tokens = textPart.Lines[aLineIndex].GetTokens();
                if (aTokenIndex < 0 || aTokenIndex >= tokens.Length)
                    return null;
                sb.Append(string.Join(" ", tokens.Skip(aTokenIndex)));

                // mid-lines
                for (int i = aLineIndex + 1; i < bLineIndex; i++)
                {
                    sb.AppendLine();
                    sb.Append(textPart.Lines[i].Text);
                }

                // last line
                sb.AppendLine();
                tokens = textPart.Lines[bLineIndex].GetTokens();
                if (bTokenIndex < 0 || bTokenIndex >= tokens.Length)
                    return null;
                sb.Append(string.Join(" ", tokens.Take(bTokenIndex + 1)));

                return sb.ToString();
            }

            // single token
            // X
            int tokenIndex = loc.A.X - 1;
            tokens = textPart.Lines[aLineIndex].GetTokens();
            return tokenIndex < 0 || tokenIndex >= tokens.Length
                ? null : tokens[tokenIndex];
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Pins: all the pins from the part's fragments, sorted first in their
        /// order, and then by the criterion used by the fragment's type.
        /// By convention, fragment-generated pins should all start with
        /// prefix <see cref="PartBase.FR_PREFIX"/>.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>Pins.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            List<DataPin> pins = new List<DataPin>();
            if (Fragments == null) return pins;

            // add pins from fragments
            foreach (TFragment fr in Fragments)
            {
                foreach (DataPin frPin in fr.GetDataPins(item))
                {
                    DataPin pin = CreateDataPin(frPin.Name, frPin.Value);
                    pins.Add(pin);
                }
            }

            return pins;
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            TFragment fr = Activator.CreateInstance<TFragment>();
            return fr.GetDataPinDefinitions();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{TypeId}.{RoleId}: {Fragments?.Count}";
        }
    }
}
