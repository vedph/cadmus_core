using System;
using System.Collections.Generic;
using System.Linq;
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
    /// categories, which are traditionally used to group them according to their
    /// type (e.g. funerary, votive, honorary, etc.). In fact, there are a number of
    /// such thesauri, which vary according to the content being handled:
    /// categories, languages, metres, etc. In such cases, usually we also want our
    /// editing UI to provide these tags as a closed set of lookup values, so that
    /// users can pick them from a list, rather than typing them (which would be more
    /// difficult, and error-prone). Thus, Cadmus provides a generic solution to
    /// these scenarios in the form of thesauri. Entries in these thesauri are
    /// generic id/value pairs used by some parts to represent a set of selectable
    /// options. Each thesaurus refers to a logical set (e.g. categories,
    /// languages, etc.), and has a specific language.
    /// </remarks>
    public sealed class Thesaurus
    {
        private readonly Dictionary<string, ThesaurusEntry> _entries;

        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <value>The tag ID must not be null, and should contain only letters
        /// (a-z or A-Z), digits (0-9), underscores, dashes, dots (which can
        /// be used to represent a hierarchy), and end with a language suffix
        /// like in RDF, e.g. <c>@en</c> = English.</value>
        /// <exception cref="ArgumentNullException">null value</exception>
        /// <exception cref="ArgumentException">invalid value</exception>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Thesaurus"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">id</exception>
        public Thesaurus(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!Regex.IsMatch(id, @"^[a-zA-Z0-9_\-\.]+\@[a-z]{2}$"))
            {
                throw new ArgumentException(LocalizedStrings.Format(
                    Properties.Resources.InvalidTagSetId, id));
            }
            Id = id;

            _entries = new Dictionary<string, ThesaurusEntry>();
        }

        /// <summary>
        /// Gets the language of the tags in this set, as extracted from
        /// the set <see cref="Id"/>. If not set there, it defaults to English
        /// (<c>en</c>).
        /// </summary>
        /// <returns>ISO 639-2 language code</returns>
        public string GetLanguage()
        {
            Match m = Regex.Match(Id, @"\@([a-z]{2})$");
            return m.Success? m.Groups[1].Value : "en";
        }

        /// <summary>
        /// Gets the entries, sorted by their key.
        /// </summary>
        public IList<ThesaurusEntry> GetEntries()
        {
            List<ThesaurusEntry> entries = new List<ThesaurusEntry>();
            foreach (string key in _entries.Keys.OrderBy(s => s))
                entries.Add(_entries[key].Clone());
            return entries;
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

            _entries[entry.Id] = entry.Clone();
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

            return _entries.ContainsKey(id) ? _entries[id].Value : null;
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} ({_entries.Count})";
        }
    }
}
