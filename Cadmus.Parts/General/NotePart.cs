using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Generic text note. A note just contains some text in any format chosen by
    /// the implementor, plus an optional tag to categorize notes where required.
    /// Tag: <c>it.vedph.note</c>.
    /// </summary>
    [Tag("it.vedph.note")]
    public sealed class NotePart : PartBase, IHasText
    {
        /// <summary>
        /// Gets or sets the optional tag linked to this note. You might want to use
        /// this value to categorize or group notes according to some criteria.
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
        public string GetText() => Text;

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// Pins: <c>tag</c>=tag if defined, else none.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return Tag != null
                ? new[]
                {
                    CreateDataPin("tag", Tag)
                }
                : Enumerable.Empty<DataPin>();
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.String,
                    "tag",
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
            StringBuilder sb = new StringBuilder();

            sb.Append("[Note] ");
            if (Tag != null) sb.Append(" (").Append(Tag).Append(')');
            if (Text != null)
            {
                sb.Append(Text.Length > 100
                    ? Text.Substring(0, 100) + "..."
                    : Text);
            }
            return sb.ToString();
        }
    }
}
