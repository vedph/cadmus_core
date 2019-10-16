using System;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// An entry used in a thesaurus (<see cref="Thesaurus"/>).
    /// </summary>
    public sealed class ThesaurusEntry
    {
        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <value>The tag ID must not be null, and should contain only letters
        /// (a-z or A-Z), digits (0-9), underscores, dashes, and dots (which can
        /// be used to represent a hierarchy, e.g. <c>inscription.funerary</c>).
        /// </value>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the entry's human-readable value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThesaurusEntry"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">id or value</exception>
        /// <exception cref="ArgumentException">invalid ID</exception>
        public ThesaurusEntry(string id, string value)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            if (!Regex.IsMatch(id, @"^[a-zA-Z0-9_\-\.]+$"))
            {
                throw new ArgumentException(LocalizedStrings.Format(
                    Properties.Resources.InvalidTagId, id));
            }
            Id = id;

            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>New instance.</returns>
        public ThesaurusEntry Clone()
        {
            return new ThesaurusEntry(Id, Value);
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{Id}] {Value}";
        }
    }
}
