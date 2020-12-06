using System;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// An entry used in a thesaurus (<see cref="Thesaurus"/>).
    /// </summary>
    public sealed class ThesaurusEntry
    {
        private string _id;

        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <value>The tag ID must not be null, and should contain only letters
        /// (a-z or A-Z), digits (0-9), underscores, dashes, and dots (which can
        /// be used to represent a hierarchy, e.g. <c>inscription.funerary</c>).
        /// </value>
        /// <exception cref="ArgumentException">invalid ID</exception>
        public string Id
        {
            get { return _id; }
            set
            {
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_\-\.]+$"))
                {
                    throw new ArgumentException(LocalizedStrings.Format(
                        Properties.Resources.InvalidTagId, value));
                }
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the entry's human-readable value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThesaurusEntry"/> class.
        /// </summary>
        public ThesaurusEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThesaurusEntry"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">ID</exception>
        /// <exception cref="ArgumentException">invalid ID</exception>
        public ThesaurusEntry(string id, string value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!Regex.IsMatch(id, @"^[a-zA-Z0-9_\-\.]+$"))
            {
                throw new ArgumentException(LocalizedStrings.Format(
                    Properties.Resources.InvalidTagId, id));
            }
            _id = id;

            Value = value;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>New instance.</returns>
        public ThesaurusEntry Clone()
        {
            return new ThesaurusEntry(_id, Value);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{Id}] {Value}";
        }
    }
}
