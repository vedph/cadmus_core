using Cadmus.Core;
using Fusi.Tools.Config;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Bibliography part. This contains any number of <see cref="BibEntry"/>
    /// entries. Tag: <c>net.fusisoft.bibliography</c>.
    /// </summary>
    /// <seealso cref="PartBase" />
    [Tag("net.fusisoft.bibliography")]
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
        /// keys: <c>type-X-count</c>, <c>author</c> (for authors and
        /// contributors; filtered last name only), <c>title</c> (filtered,
        /// with digits), <c>container</c> (filtered, with digits),
        /// <c>keyword</c> (prefixed by language between <c>[]</c>, filtered
        /// with digits).</returns>
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
                        builder.AddValues("keyword",
                            from k in entry.Keywords
                            select builder.ApplyFilter(
                                $"[{k.Language}]", true, k.Value));
                    }
                }
            }

            return builder.Build(this);
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
