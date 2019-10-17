using System;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Cadmus.Core.Layers;
using Fusi.Tools.Config;

namespace Cadmus.Parts.Layers
{
    /// <summary>
    /// Generic comment fragment. This item contains a generic comment referred
    /// to a specific text portion. The text format depends on the implementor.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Layers.ITextLayerFragment" />
    /// <seealso cref="Cadmus.Core.IHasText" />
    [Tag("fr.comment")]
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
        /// <returns>Data pins.</returns>
        public IEnumerable<DataPin> GetDataPins()
        {
            return Tag != null
                ? new[]
                {
                    new DataPin
                    {
                        Name = "tag",
                        Value = Tag
                    }
                }
                : Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Comment [{Tag}]: {(Text?.Length > 80 ? Text.Substring(0, 80) + "..." : Text)}";
        }
    }
}
