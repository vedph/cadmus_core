﻿using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Tiled text part.
    /// <para>Tag: <c>it.vedph.tiled-text</c>.</para>
    /// </summary>
    /// <remarks>A tiled text part is a general-purpose text laid out in 2
    /// dimensions: it has any number of <see cref="Rows"/>, each having
    /// any number of <see cref="TextTile"/>'s. Every <see cref="TextTile"/>
    /// represents a portion of the text, arbitrarily delimited, and has
    /// any number of keyed data (including the text value itself). Rows are
    /// numbered from 1 to N, sequentially, and the same goes for tiles
    /// inside each row. Thus, you can use the same token-based coordinates
    /// system used for <see cref="TokenTextPart"/>, with the difference that
    /// here each Y,X coordinates pair refers to row and tile, while there
    /// it refers to line and token. Usually, coordinates do not refer to
    /// a subset of the text inside a tile, but the architecture does not rule
    /// this out.</remarks>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.tiled-text")]
    public sealed class TiledTextPart : PartBase, IHasText
    {
        /// <summary>
        /// Gets or sets the citation. This is an optional arbitrary string,
        /// used to virtually connect several text parts belonging to a bigger
        /// unit.
        /// </summary>
        public string Citation { get; set; }

        /// <summary>
        /// Gets or sets the rows in this text.
        /// </summary>
        public List<TextTileRow> Rows { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledTextPart"/> class.
        /// Note that this part automatically gets a role ID equal to
        /// <see cref="PartBase.BASE_TEXT_ROLE_ID"/>.
        /// </summary>
        public TiledTextPart()
        {
            Rows = new List<TextTileRow>();
            RoleId = BASE_TEXT_ROLE_ID;
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>row-count</c>, <c>citation</c>=citation if any.
        /// </returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            List<DataPin> pins = new List<DataPin>
            {
                CreateDataPin("row-count",
                Rows?.Count.ToString(CultureInfo.InvariantCulture) ?? "0")
            };

            if (!string.IsNullOrEmpty(Citation))
                pins.Add(CreateDataPin("citation", Citation));

            return pins;
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "row-count",
                    "The rows count."),
                new DataPinDefinition(DataPinValueType.String,
                    "citation",
                    "The citation if any.")
            });
        }

        /// <summary>
        /// Get a single string representing the whole text, line by line.
        /// </summary>
        /// <returns>Text.</returns>
        public string GetText()
        {
            return Rows == null ?
                "" :
                string.Join(Environment.NewLine,
                    from row in Rows
                    select row.GetText());
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[TiledText] {Rows?.Count}";
        }
    }
}
