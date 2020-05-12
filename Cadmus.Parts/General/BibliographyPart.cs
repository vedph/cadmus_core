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

        private static void AddAuthors(IList<BibAuthor> authors,
            HashSet<string> target)
        {
            if (authors?.Count > 0)
            {
                foreach (BibAuthor author in authors)
                    target.Add(StandardTextFilter.Apply(author.LastName));
            }
        }

        /// <summary>
        /// Get all the key=value pairs (pins) exposed by the implementor.
        /// </summary>
        /// <remarks>
        /// For each entry, the pins emitted are (omitting duplicates):
        /// <list type="bullet">
        /// <item>
        /// <term>biblio.type</term>
        /// <description>The entry type ID.</description>
        /// </item>
        /// <item>
        /// <term>biblio.author</term>
        /// <description>The filtered last name of the author.</description>
        /// </item>
        /// <item>
        /// <term>biblio.title</term>
        /// <description>The filtered title.</description>
        /// </item>
        /// <item>
        /// <term>biblio.container</term>
        /// <description>The filtered container name.</description>
        /// </item>
        /// <item>
        /// <term>biblio.keyword</term>
        /// <description>The keyword.</description>
        /// </item>
        /// </list>
        /// Filtering implies preserving only letters, digits, whitespaces
        /// (normalized), and apostrophe. Letters are all lowercase and without
        /// any diacritics.
        /// </remarks>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            // collect data from entries
            HashSet<string> typeIds = new HashSet<string>();
            HashSet<string> authors = new HashSet<string>();
            HashSet<string> titles = new HashSet<string>();
            HashSet<string> containers = new HashSet<string>();
            HashSet<string> keywords = new HashSet<string>();

            foreach (BibEntry entry in Entries)
            {
                // type
                if (entry.TypeId != null) typeIds.Add(entry.TypeId);

                // authors (filtered last names)
                AddAuthors(entry.Authors, authors);
                AddAuthors(entry.Contributors, authors);

                // title (filtered)
                string title = StandardTextFilter.Apply(entry.Title);
                if (!string.IsNullOrEmpty(title)) titles.Add(title);

                // container (filtered)
                string container = StandardTextFilter.Apply(entry.Container);
                if (!string.IsNullOrEmpty(container)) containers.Add(container);

                // keywords
                if (entry.Keywords?.Length > 0)
                {
                    foreach (Keyword k in entry.Keywords)
                        keywords.Add(k.Value);
                }
            }

            // add pins
            List<DataPin> pins = new List<DataPin>();
            pins.AddRange(from s in typeIds select CreateDataPin("biblio.type", s));
            pins.AddRange(from s in authors select CreateDataPin("biblio.author", s));
            pins.AddRange(from s in titles select CreateDataPin("biblio.title", s));
            pins.AddRange(from s in containers select CreateDataPin("biblio.container", s));
            pins.AddRange(from s in keywords select CreateDataPin("biblio.keyword", s));

            return pins;
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
