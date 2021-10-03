using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A set of <see cref="GraphPinFilter"/>'s.
    /// </summary>
    public class GraphPinFilterSet
    {
        /// <summary>
        /// Gets or sets a value indicating whether this set represents a
        /// blacklist, i.e. the pins matching the filters are the one to be
        /// excluded.
        /// </summary>
        public bool IsBlack { get; set; }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public IList<GraphPinFilter> Filters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphPinFilterSet"/>
        /// class.
        /// </summary>
        public GraphPinFilterSet()
        {
            Filters = new List<GraphPinFilter>();
        }

        /// <summary>
        /// Determines whether the specified pin name matches this set of
        /// filters.
        /// </summary>
        /// <param name="pinName">The name of the pin.</param>
        /// <param name="partTypeId">The optional part type identifier.</param>
        /// <param name="partRoleId">The optional part role identifier.</param>
        /// <returns>
        /// <c>true</c> if the specified pin name matches; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(string pinName,
            string partTypeId = null, string partRoleId = null)
        {
            if (Filters == null || Filters.Count == 0) return !IsBlack;

            // if whitelist, any matching filter is ok (OR)
            bool match = Filters.Any(f => f.IsMatch(pinName, partTypeId, partRoleId));
            return IsBlack ? !match : match;
        }
    }
}
