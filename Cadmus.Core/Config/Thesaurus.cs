using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// A thesaurus.
    /// </summary>
    /// <remarks>
    /// Often, a common requirement for data is having some shared
    /// terminology to be used for the whole content. For instance, think of a
    /// collection of inscriptions: a typical requirement would be a set of
    /// categories, which are traditionally used to group them according to
    /// their type (e.g. funerary, votive, honorary, etc.). In fact, there are
    /// a number of such thesauri, which vary according to the content being
    /// handled: categories, languages, metres, etc. In such cases, usually we
    /// also want our editing UI to provide these tags as a closed set of
    /// lookup values, so that users can pick them from a list, rather than
    /// typing them (which would be more difficult, and error-prone). Thus,
    /// Cadmus provides a generic solution to these scenarios in the form of
    /// thesauri. Entries in these thesauri are generic id/value pairs used by
    /// some parts to represent a set of selectable options. Each thesaurus
    /// refers to a logical set (e.g. categories, languages, etc.), and has a
    /// specific language.
    /// </remarks>
    public sealed class Thesaurus
    {
        private static readonly Regex _idRegex = new Regex(@".+\@[a-z]{2,3}$");
        private string _id;

        /// <summary>
        /// Gets or sets the thesaurus unique identifier.
        /// </summary>
        /// <value>The ID must end with a language suffix like in RDF, using
        /// ISO639-2 or ISO639-3, e.g. <c>@en</c> (or <c>eng</c>) = English.
        /// The choice between ISO639-2 and -3 is up to the user, but once set
        /// it should be coherent.</value>
        /// <exception cref="ArgumentNullException">null value</exception>
        /// <exception cref="ArgumentException">invalid value</exception>
        public string Id
        {
            get { return _id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (!_idRegex.IsMatch(value))
                    throw new ArgumentException(nameof(value));
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the target thesaurus identifier. This is used only
        /// for those thesauri which are just aliases to another thesaurus.
        /// When this property is not null, this is an alias and should have
        /// no entries. Otherwise, it is a regular thesaurus with entries.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public IList<ThesaurusEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thesaurus"/> class.
        /// </summary>
        public Thesaurus()
        {
            Entries = new List<ThesaurusEntry>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thesaurus"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentException">invalid ID</exception>
        /// <exception cref="ArgumentNullException">id</exception>
        public Thesaurus(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            if (!_idRegex.IsMatch(id))
                throw new ArgumentException(nameof(id));

            Entries = new List<ThesaurusEntry>();
        }

        /// <summary>
        /// Gets the language of the tags in this set, as extracted from
        /// the set <see cref="Id"/>. If not set there, it defaults to English
        /// (ISO639-2: <c>en</c>).
        /// </summary>
        /// <returns>ISO 639-2/-3 language code.</returns>
        public string GetLanguage()
        {
            Match m = Regex.Match(Id, @"\@([a-z]{2,3})$");
            return m.Success? m.Groups[1].Value : "en";
        }

        /// <summary>
        /// Adds the specified entry to this thesaurus.
        /// If an entry with the same ID already exists, it will be replaced.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <exception cref="ArgumentNullException">entry</exception>
        public void AddEntry(ThesaurusEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            if (Entries == null)
            {
                Entries = new List<ThesaurusEntry>
                {
                    entry
                };
                return;
            }
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Id == entry.Id)
                {
                    Entries[i] = entry;
                    return;
                }
            }
            Entries.Add(entry);
        }

        /// <summary>
        /// Gets the tag with the specified ID from this set.
        /// </summary>
        /// <param name="id">The tag's ID.</param>
        /// <returns>tag, or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public string GetEntryValue(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            ThesaurusEntry entry = Entries?.FirstOrDefault(e => e.Id == id);
            return entry?.Value;
        }

        private static int CountDots(string text)
        {
            int n = 0;
            foreach (char c in text)
            {
                if (c == '.') n++;
            }
            return n;
        }

        /// <summary>
        /// Visits this thesaurus entries as a tree, by levels: first the top
        /// siblings, then their children, etc.
        /// </summary>
        /// <param name="visitor">The visitor function to invoke for each
        /// node. This returns <c>true</c> to continue, or <c>false</c> to
        /// stop visiting.</param>
        /// <exception cref="ArgumentNullException">visitor</exception>
        public void VisitByLevel(Func<ThesaurusTreeEntry, bool> visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));
            if (Entries == null || Entries.Count == 0) return;

            var groups = from e in Entries
                          group e by CountDots(e.Id) into g
                          select new
                          {
                              Level = g.Key + 1,
                              Entries = g.ToList()
                          };
            Dictionary<string, ThesaurusTreeEntry> parents =
                new Dictionary<string, ThesaurusTreeEntry>();

            foreach (var group in groups)
            {
                foreach (ThesaurusEntry entry in group.Entries)
                {
                    int i = entry.Id.LastIndexOf('.');
                    string parentId = i == -1? entry.Id : entry.Id.Substring(0, i);

                    ThesaurusTreeEntry treeEntry = new ThesaurusTreeEntry(entry)
                    {
                        Parent = parents.ContainsKey(parentId)
                            ? parents[parentId] : null
                    };
                    if (!visitor(treeEntry)) return;

                    if (!parents.ContainsKey(entry.Id))
                        parents[entry.Id] = treeEntry;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} ({Entries?.Count ?? 0})";
        }
    }
}
