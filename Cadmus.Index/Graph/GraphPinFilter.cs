using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A filter used to select which pins must be included in or excluded
    /// from the set of pins to be passed to node mappers. All the properties
    /// in this filter are optional. All the properties set must be matched
    /// for the filter to match.
    /// </summary>
    public class GraphPinFilter
    {
        private string _pattern;
        private Regex _regex;

        /// <summary>
        /// Gets or sets the part type identifier.
        /// </summary>
        public string PartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the part role identifier.
        /// </summary>
        public string PartRoleId { get; set; }

        /// <summary>
        /// Gets or sets the pin name prefix.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the pin name suffix.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the pin name pattern.
        /// </summary>
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                if (_pattern == value) return;
                _pattern = value;
                _regex = value != null ? new Regex(value) : null;
            }
        }

        /// <summary>
        /// Determines whether the specified pin name matches this filter.
        /// </summary>
        /// <param name="pinName">The name of the pin.</param>
        /// <param name="partTypeId">The optional part type identifier.</param>
        /// <param name="partRoleId">The optional part role identifier.</param>
        /// <returns>
        /// <c>true</c> if the specified pin name matches; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">pinName</exception>
        public bool IsMatch(string pinName,
            string partTypeId = null, string partRoleId = null)
        {
            if (pinName == null) throw new ArgumentNullException(nameof(pinName));

            if (!string.IsNullOrEmpty(PartTypeId)
                && !string.IsNullOrEmpty(partTypeId)
                && PartTypeId != partTypeId)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(PartRoleId)
                && !string.IsNullOrEmpty(partRoleId)
                && PartRoleId != partRoleId)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Prefix) &&
                !pinName.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Suffix) &&
                !pinName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                return false;
            }

            if (_regex?.IsMatch(pinName) == false) return false;

            return true;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(PartTypeId)) sb.Append(PartTypeId);
            if (!string.IsNullOrEmpty(PartRoleId)) sb.Append(':').Append(PartRoleId);
            if (!string.IsNullOrEmpty(Prefix)) sb.Append(" - p=").Append(Prefix);
            if (!string.IsNullOrEmpty(Suffix)) sb.Append(" - s=").Append(Suffix);
            if (!string.IsNullOrEmpty(Pattern)) sb.Append(" - r=").Append(Pattern);

            return sb.ToString();
        }
    }
}
