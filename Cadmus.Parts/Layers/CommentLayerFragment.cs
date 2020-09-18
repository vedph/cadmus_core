﻿using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Generic comment fragment. This item contains a generic comment referred
    /// to a specific text portion. The text format depends on the implementor.
    /// Tag: <c>fr.it.vedph.comment</c>.
    /// </summary>
    /// <seealso cref="ITextLayerFragment" />
    /// <seealso cref="IHasText" />
    [Tag("fr.it.vedph.comment")]
    public sealed class CommentLayerFragment : ITextLayerFragment, IHasText
    {
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
        /// Gets or sets the optional tag linked to this comment. You might want
        /// to use this value to categorize or group comments according to some
        /// criteria.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the text. The format of the text is chosen by the
        /// implementor (it might be plain text, Markdown, RTF, HTML, XML, etc).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>full text</returns>
        public string GetText()
        {
            return Text;
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>fr.tag</c>=tag if any.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return Tag != null
                ? new[]
                {
                    new DataPin
                    {
                        Name = PartBase.FR_PREFIX + "tag",
                        Value = Tag
                    }
                }
                : Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.String,
                    PartBase.FR_PREFIX + "tag",
                    "The tag if any.")
            });
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Comment [{Tag}]: {(Text?.Length > 80 ? Text.Substring(0, 80) + "..." : Text)}";
        }
    }
}
