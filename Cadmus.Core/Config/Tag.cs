using System;
using System.Text.RegularExpressions;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// A tag used in a <see cref="TagSet"/>.
    /// </summary>
    public sealed class Tag
    {
        private string _id;

        /// <summary>
        /// Gets or sets the tag's unique identifier.
        /// </summary>
        /// <value>The tag ID must not be null, and should contain only letters
        /// (a-z or A-Z), digits (0-9), undercores, dashes, and dots (which can
        /// be used to represent a hierarchy, e.g. <c>inscription.funerary</c>).
        /// </value>
        /// <exception cref="ArgumentNullException">null value</exception>
        /// <exception cref="ArgumentException">invalid value</exception>
        public string Id
        {
            get => _id;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_\-\.]+$"))
                    throw new ArgumentException(LocalizedStrings.Format(
                        Properties.Resources.InvalidTagId, value));
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the tag's human-readable name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}
