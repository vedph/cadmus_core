using System;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// An entry used in a thesaurus (<see cref="Thesaurus"/>).
    /// </summary>
    public class ThesaurusEntry
    {
        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <exception cref="ArgumentException">invalid ID</exception>
        public string Id { get; set; }

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
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Value = value ?? "";
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>New instance.</returns>
        public virtual ThesaurusEntry Clone()
        {
            return new ThesaurusEntry(Id, Value);
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
