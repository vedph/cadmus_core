﻿using System.Text;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// An index keyword.
    /// </summary>
    /// <seealso cref="Cadmus.Parts.General.Keyword" />
    public sealed class IndexKeyword : Keyword
    {
        /// <summary>
        /// Gets or sets the optional index identifier. This can be used when
        /// you are building more than a single index.
        /// </summary>
        public string IndexId { get; set; }

        /// <summary>
        /// Gets or sets an optional short note for this keyword.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the optional tag for this keyword, representing any
        /// additional general purpose classification tag for the keyword.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(IndexId))
                sb.Append(IndexId).Append(": ");

            if (!string.IsNullOrEmpty(Language))
                sb.Append('[').Append(Language).Append("] ");

            sb.Append(Value);

            if (!string.IsNullOrEmpty(Note))
                sb.Append(" (").Append(Note).Append(')');
            if (!string.IsNullOrEmpty(Tag))
                sb.Append(" <").Append(Tag).Append('>');

            return sb.ToString();
        }
    }
}