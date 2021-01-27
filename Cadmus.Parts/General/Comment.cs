using Cadmus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A generic free text comment, with optional tag, document references,
    /// external IDs, keywords, and categories.
    /// </summary>
    public class Comment : IHasText
    {
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
        /// Gets or sets the optional references related to this comment.
        /// </summary>
        public List<DocReference> References { get; set; }

        /// <summary>
        /// Gets or sets the optional external IDs related to this comment.
        /// </summary>
        public List<string> ExternalIds { get; set; }

        /// <summary>
        /// Gets or sets the optional categories.
        /// </summary>
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the optional keywords.
        /// </summary>
        public List<IndexKeyword> Keywords { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/>
        /// class.
        /// </summary>
        public Comment()
        {
            References = new List<DocReference>();
            ExternalIds = new List<string>();
            Categories = new List<string>();
            Keywords = new List<IndexKeyword>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Tag)) sb.Append('[').Append(Tag).Append(']');

            if (!string.IsNullOrEmpty(Text))
            {
                sb.Append(' ');
                for (int i = 0; i < Math.Min(Text.Length, 60); i++)
                {
                    if (Text[i] == '\r' || Text[i] == '\n') sb.Append(' ');
                    else sb.Append(Text[i]);
                }
                if (Text.Length > 60) sb.Append("...");
            }

            if (References?.Count > 0)
                sb.Append(" [R=").Append(References.Count).Append(']');
            if (ExternalIds?.Count > 0)
                sb.Append(" [X=").Append(ExternalIds.Count).Append(']');
            if (Categories?.Count > 0)
                sb.Append(" [C=").Append(Categories.Count).Append(']');
            if (Keywords?.Count > 0)
                sb.Append(" [K=").Append(Keywords.Count).Append(']');

            return sb.ToString();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string GetText() => Text;

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <param name="part">The optional part. This is used when building
        /// pins for a data part, rather than for a fragment.</param>
        /// <param name="prefix">The prefix to be added to pins.</param>
        /// <returns>The pins: <c>fr.tag</c>=tag if any, plus these list of
        /// pins: <c>fr.ref</c>=references (built with author and work,
        /// both filtered), <c>fr.id</c>=external IDs, <c>fr.cat</c>=categories,
        /// <c>fr.key.{INDEXID}.{LANG}</c>=keywords.</returns>
        public IEnumerable<DataPin> GetDataPins(IItem item, IPart part, string prefix)
        {
            DataPinBuilder builder = new DataPinBuilder(
                DataPinHelper.DefaultFilter);

            // fr.tag
            builder.AddValue(prefix + "tag", Tag);

            // fr.ref
            if (References?.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                builder.AddValues(prefix + "ref",
                    References.Select(r =>
                    {
                        sb.Clear();
                        if (!string.IsNullOrEmpty(r.Author)) sb.Append(r.Author);
                        if (!string.IsNullOrEmpty(r.Work))
                        {
                            if (sb.Length > 0) sb.Append(", ");
                            sb.Append(r.Work);
                        }
                        return sb.ToString();
                    }), filter: true, filterOptions: true);
            }

            // fr.id
            if (ExternalIds?.Count > 0)
                builder.AddValues(prefix + "id", ExternalIds);

            // fr.cat
            if (Categories?.Count > 0)
                builder.AddValues(prefix + "cat", Categories);

            // fr.key
            if (Keywords?.Count > 0)
            {
                foreach (string indexId in Keywords.Select(k => k.IndexId ?? "")
                    .OrderBy(s => s).Distinct())
                {
                    var keysByLang = from k in Keywords
                                     where (k.IndexId ?? "") == indexId
                                     group k by k.Language
                                         into g
                                     orderby g.Key
                                     select g;

                    foreach (var g in keysByLang)
                    {
                        var values = from k in g
                                     orderby k.Value
                                     select DataPinHelper.DefaultFilter
                                        .Apply(k.Value, true);
                        foreach (string value in values)
                        {
                            builder.AddValue(
                                prefix + $"key.{indexId}.{g.Key}",
                                value);
                        }
                    }
                }
            }

            return builder.Build(part);
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <param name="prefix">The prefix to be added to pins.</param>
        /// <returns>Data pins definitions.</returns>
        public IList<DataPinDefinition> GetDataPinDefinitions(string prefix)
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.String,
                    prefix + "tag",
                    "The tag if any."),
                new DataPinDefinition(DataPinValueType.String,
                    prefix + "ref",
                    "The list of references, if any.",
                    "MF"),
                new DataPinDefinition(DataPinValueType.String,
                    prefix + "id",
                    "The list of external IDs, if any.",
                    "M"),
                new DataPinDefinition(DataPinValueType.String,
                    prefix + "cat",
                    "The list of external categories, if any.",
                    "M"),
                new DataPinDefinition(DataPinValueType.String,
                    prefix + "key.{INDEXID}.{LANG}",
                    "The list of keywords grouped by index ID and language.",
                    "Mf"),
            });
        }
    }
}
