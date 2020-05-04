using Cadmus.Core;
using Cadmus.Core.Layers;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Text layer part class, based on tiles-referenced text. Note that
    /// currently tiles-referenced text uses the same coordinates system as
    /// token-referenced text: for tiles, y=row number and x=tile number.
    /// Tag: <c>net.fusisoft.tiled-text-layer</c>.
    /// </summary>
    /// <remarks>This class represents any text layer part using tiles-based
    /// coordinates. The tiles layer item part is just a wrapper for a collection
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
    /// <typeparam name="TFragment">The type of the fragment.</typeparam>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.tiled-text-layer")]
    public sealed class TiledTextLayerPart<TFragment> : YXLayerPartBase<TFragment>
        where TFragment : ITextLayerFragment, new()
    {
        private static string GetTileText(TextTile tile) =>
            tile.Data.ContainsKey(TextTileRow.TEXT_DATA_NAME)
                ? tile.Data[TextTileRow.TEXT_DATA_NAME]
                : "";

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

            if (!(baseTextPart is TiledTextPart textPart)) return null;

            // parse
            TokenTextLocation loc = TokenTextLocation.Parse(location);
            int aRowIndex = loc.A.Y - 1;
            if (aRowIndex < 0 || aRowIndex >= textPart.Rows.Count)
                return null;
            List<TextTile> tiles;

            // range
            if (loc.IsRange)
            {
                // tokens range
                StringBuilder sb = new StringBuilder();

                int bRowIndex = loc.B.Y - 1;
                if (bRowIndex < aRowIndex || bRowIndex >= textPart.Rows.Count)
                    return null;

                int aTileIndex = loc.A.X - 1;
                int bTileIndex = loc.B.X - 1;

                // corner case: same row for A-B
                if (aRowIndex == bRowIndex)
                {
                    // defensive
                    if (bTileIndex < aTileIndex) return null;
                    tiles = textPart.Rows[aRowIndex].Tiles;
                    if (aTileIndex < 0 || aTileIndex >= tiles.Count)
                        return null;

                    return string.Join(" ",
                        from t in tiles
                            .Skip(aTileIndex)
                            .Take(bTileIndex + 1 - aTileIndex)
                        select GetTileText(t));
                }

                // first row
                // A.X
                tiles = textPart.Rows[aRowIndex].Tiles;
                if (aTileIndex < 0 || aTileIndex >= tiles.Count)
                    return null;
                sb.Append(string.Join(" ",
                    from t in tiles.Skip(aTileIndex)
                    select GetTileText(t)));

                // mid-rows
                for (int i = aRowIndex + 1; i < bRowIndex; i++)
                {
                    sb.AppendLine();
                    sb.Append(string.Join(" ",
                        from t in textPart.Rows[i].Tiles
                        select GetTileText(t)));
                }

                // last row
                sb.AppendLine();
                tiles = textPart.Rows[bRowIndex].Tiles;
                if (bTileIndex < 0 || bTileIndex >= tiles.Count)
                    return null;
                sb.Append(string.Join(" ",
                    from t in tiles.Take(bTileIndex + 1)
                    select GetTileText(t)));

                return sb.ToString();
            }

            // single tile
            // X
            int tokenIndex = loc.A.X - 1;
            tiles = textPart.Rows[aRowIndex].Tiles;
            return tokenIndex < 0 || tokenIndex >= tiles.Count
                ? null : GetTileText(tiles[tokenIndex]);
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
