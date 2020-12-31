using Cadmus.Core;
using Fusi.Tools.Config;
using System.Collections.Generic;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A generic free text comment, with optional tag, document references,
    /// external IDs, keywords, and categories.
    /// Tag: <c>it.vedph.comment</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.comment")]
    public sealed class CommentPart : PartBase, IHasText
    {
        private readonly Comment _comment;

        /// <summary>
        /// Gets or sets the optional tag linked to this comment. You might want
        /// to use this value to categorize or group comments according to some
        /// criteria.
        /// </summary>
        public string Tag
        {
            get { return _comment.Tag; }
            set { _comment.Tag = value; }
        }

        /// <summary>
        /// Gets or sets the text. The format of the text is chosen by the
        /// implementor (it might be plain text, Markdown, RTF, HTML, XML, etc).
        /// </summary>
        public string Text
        {
            get { return _comment.Text; }
            set { _comment.Text = value; }
        }

        /// <summary>
        /// Gets or sets the optional references related to this comment.
        /// </summary>
        public List<DocReference> References
        {
            get { return _comment.References; }
            set { _comment.References = value; }
        }

        /// <summary>
        /// Gets or sets the optional external IDs related to this comment.
        /// </summary>
        public List<string> ExternalIds
        {
            get { return _comment.ExternalIds; }
            set { _comment.ExternalIds = value; }
        }

        /// <summary>
        /// Gets or sets the optional categories.
        /// </summary>
        public List<string> Categories
        {
            get { return _comment.Categories; }
            set { _comment.Categories = value; }
        }

        /// <summary>
        /// Gets or sets the optional keywords.
        /// </summary>
        public List<IndexKeyword> Keywords
        {
            get { return _comment.Keywords; }
            set { _comment.Keywords = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentPart"/> class.
        /// </summary>
        public CommentPart()
        {
            _comment = new Comment();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string GetText() => _comment.GetText();

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tag</c>=tag if any, plus these list of
        /// pins: <c>ref</c>=references (built with author and work,
        /// both filtered), <c>id</c>=external IDs, <c>cat</c>=categories,
        /// <c>key.{INDEXID}.{LANG}</c>=keywords.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            return _comment.GetDataPins(item, this, "");
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return _comment.GetDataPinDefinitions("");
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "[Comment] " + _comment;
        }
    }
}
