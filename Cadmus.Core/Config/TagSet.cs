using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// A set of tags. Tags are generic id/name pairs used by some parts to represent
    /// a set of selectable options. E.g. you might have a list of categories to be
    /// selected for a categories part; each item in the list would be a tag from a
    /// categories tags set, having a unique ID and a human-readable name representing
    /// it.
    /// </summary>
    public sealed class TagSet : ITagSet
    {
        private string _id;
        private Dictionary<string, Tag> _dct;

        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <value>The tag ID must not be null, and should contain only letters
        /// (a-z or A-Z), digits (0-9), undercores, dashes, dots (which can
        /// be used to represent a hierarchy), and end with a language suffix
        /// like in RDF, e.g. <c>@en</c> = English.</value>
        /// <exception cref="ArgumentNullException">null value</exception>
        /// <exception cref="ArgumentException">invalid value</exception>
        public string Id
        {
            get => _id;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_\-\.]+\@[a-z]{2}$"))
                {
                    throw new ArgumentException(LocalizedStrings.Format(
                        Properties.Resources.InvalidTagSetId, value));
                }
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the tags in this set.
        /// </summary>
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagSet"/> class.
        /// </summary>
        public TagSet()
        {
            Tags = new List<Tag>();
        }

        /// <summary>
        /// Gets the language of the tags in this set, as extracted from
        /// the set <see cref="Id"/>. If not set there, it defaults to English
        /// (<c>en</c>).
        /// </summary>
        /// <returns>ISO 639-2 language code</returns>
        public string GetLanguage()
        {
            if (_id != null)
            {
                Match m = Regex.Match(_id, @"\@([a-z]{2})$");
                if (m.Success) return m.Groups[1].Value;
            }

            return "en";
        }

        /// <summary>
        /// Gets the tag with the specified ID from this set.
        /// </summary>
        /// <param name="id">The tag's ID.</param>
        /// <returns>tag, or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public Tag GetTag(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (Tags == null || Tags.Count == 0) return null;

            if (_dct == null)
            {
                _dct = new Dictionary<string, Tag>();
                foreach (Tag tag in Tags) _dct[tag.Id] = tag;
            }

            return _dct.ContainsKey(id) ? _dct[id] : null;
        }

        /// <summary>
        /// Adds the specified tag to this set. If a tag with the same ID already
        /// exists, it will be replaced by the newly added tag.
        /// </summary>
        /// <param name="id">The tag ID.</param>
        /// <param name="name">The tag name.</param>
        /// <exception cref="ArgumentNullException">null tag ID or name</exception>
        public void AddTag(string id, string name)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name));

            Tag tag = new Tag
            {
                Id = id,
                Name = name
            };
            Tag old = Tags.Find(t => t.Id == id);
            if (old != null) Tags.Remove(old);
            Tags.Add(tag);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Tags?.Count}";
        }
    }
}
