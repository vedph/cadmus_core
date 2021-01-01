using Cadmus.Core;
using Fusi.Tools.Config;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Bibliography part. This contains any number of <see cref="BibEntry"/>
    /// entries. Tag: <c>it.vedph.bibliography</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("it.vedph.bibliography")]
    public sealed class BibliographyPart : PartBase
    {
        /// <summary>
        /// Gets or sets the bibliographic entries.
        /// </summary>
        public List<BibEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BibliographyPart"/> class.
        /// </summary>
        public BibliographyPart()
        {
            Entries = new List<BibEntry>();
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tot-count</c> and a collection of pins with
        /// keys: <c>type-X-count</c>, <c>key</c>, <c>author</c> (for authors and
        /// contributors; filtered last name only), <c>title</c> (filtered,
        /// with digits), <c>container</c> (filtered, with digits),
        /// <c>keyword.LANG</c> (keyword filtered with digits).</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            DataPinBuilder builder = new DataPinBuilder(
                new StandardDataPinTextFilter());

            // tot-count
            builder.Set("tot", Entries?.Count ?? 0, false);

            if (Entries?.Count > 0)
            {
                foreach (BibEntry entry in Entries)
                {
                    // key
                    if (!string.IsNullOrEmpty(entry.Key))
                        builder.AddValue("key", entry.Key);

                    // type-X-count
                    if (!string.IsNullOrEmpty(entry.TypeId))
                        builder.Increase(entry.TypeId, false, "type-");

                    // author/contributor
                    if (entry.Authors?.Length > 0)
                    {
                        builder.AddValues("author",
                            from a in entry.Authors
                            select a.LastName, prefix: null, filter: true);
                    }
                    if (entry.Contributors?.Length > 0)
                    {
                        builder.AddValues("author",
                            from a in entry.Contributors
                            select a.LastName, prefix: null, filter: true);
                    }

                    // title
                    if (!string.IsNullOrEmpty(entry.Title))
                    {
                        builder.AddValue("title", entry.Title,
                            prefix: null, filter: true, filterOptions: true);
                    }

                    // container
                    if (!string.IsNullOrEmpty(entry.Container))
                    {
                        builder.AddValue("container", entry.Container,
                            prefix: null, filter: true, filterOptions: true);
                    }

                    // keyword
                    if (entry.Keywords?.Length > 0)
                    {
                        var keysByLang = from k in entry.Keywords
                                         group k by k.Language
                                         into g
                                         orderby g.Key
                                         select g;

                        foreach (var g in keysByLang)
                        {
                            var values = from k in g
                                         orderby k.Value
                                         select builder.Filter.Apply(k.Value, true);

                            foreach (var value in values)
                                builder.AddValue($"keyword.{g.Key}", value);
                        }
                    }
                }
            }

            return builder.Build(this);
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
                    "key",
                    "List of bibliographic entries keys.",
                    "M"),
                new DataPinDefinition(DataPinValueType.Integer,
                    "tot-count",
                    "The total count of bibliographic entries."),
                new DataPinDefinition(DataPinValueType.Integer,
                    "type-{TYPE}-count",
                    "The count of type TYPE in the entries."),
                new DataPinDefinition(DataPinValueType.String,
                    "author",
                    "List of distinct author or contributor last names.",
                    "MF"),
                new DataPinDefinition(DataPinValueType.String,
                    "title",
                    "List of distinct titles.",
                    "Mf"),
                new DataPinDefinition(DataPinValueType.String,
                    "container",
                    "List of distinct container titles.",
                    "Mf"),
                new DataPinDefinition(DataPinValueType.String,
                    "keyword.{LANG}",
                    "List of distinct keywords for language LANG.",
                    "Mf")
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

            sb.Append("[Bibliography]");
            if (Entries?.Count > 0)
            {
                sb.Append(' ');
                var groups = from e in Entries
                             group e by e.TypeId into g
                             select g;
                int n = 0;
                foreach (var g in groups)
                {
                    if (++n > 1) sb.Append(", ");
                    sb.Append(g.Key).Append('=').Append(g.Count());
                }
            }

            return sb.ToString();
        }
    }
}
