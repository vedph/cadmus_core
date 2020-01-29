using System.Collections.Generic;
using System.Globalization;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A single text "tile" in a <see cref="TiledTextPart"/>'s row.
    /// </summary>
    public sealed class TextTile
    {
        /// <summary>
        /// Gets or sets the x (=1 tile number in row, 1-N).
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the generic data attached to this tile.
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTile"/> class.
        /// </summary>
        public TextTile()
        {
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{X}: " +
                Data?.Count.ToString(CultureInfo.InvariantCulture) ?? "0";
        }
    }
}
