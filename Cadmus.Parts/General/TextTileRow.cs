using System.Linq;
using Cadmus.Core;
using System.Collections.Generic;
using System.Globalization;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A row of <see cref="TextTile"/>'s in a <see cref="TiledTextPart"/>.
    /// </summary>
    public sealed class TextTileRow : IHasText
    {
        /// <summary>
        /// The name of the data entry reserved to represent the tile's text
        /// in a row of tiles.
        /// </summary>
        public const string TEXT_DATA_NAME = "text";

        /// <summary>
        /// Gets or sets the number of this tiles row (1-N).
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the tiles in this row.
        /// </summary>
        public List<TextTile> Tiles { get; set; }

        /// <summary>
        /// Gets or sets the generic data attached to this row.
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTileRow"/> class.
        /// </summary>
        public TextTileRow()
        {
            Tiles = new List<TextTile>();
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the text obtained from joining the text of all the text-bearing
        /// tiles in this row, separated by a space.
        /// </summary>
        /// <remarks>Text.</remarks>
        public string GetText()
        {
            return Tiles == null
                ? ""
                : string.Join(" ", from t in Tiles
                                   where t.Data.ContainsKey(TEXT_DATA_NAME)
                                   select t.Data[TEXT_DATA_NAME]);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string s = $"{Y}: "
                + (Tiles?.Count.ToString(CultureInfo.InvariantCulture) ?? "0");
            if (Data?.Count > 0) s += $" [{Data.Count}]";
            return s;
        }
    }
}
