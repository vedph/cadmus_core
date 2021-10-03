using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Core
{
    /// <summary>
    /// A set of <see cref="DataPinFilterClause"/>'s.
    /// </summary>
    public class DataPinFilterClauseSet
    {
        /// <summary>
        /// Gets or sets a value indicating whether this set represents a
        /// blacklist, i.e. the pins matching the filters are the one to be
        /// excluded.
        /// </summary>
        public bool IsBlack { get; set; }

        /// <summary>
        /// Gets the clauses.
        /// </summary>
        public IList<DataPinFilterClause> Clauses { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPinFilterClauseSet"/>
        /// class.
        /// </summary>
        public DataPinFilterClauseSet()
        {
            Clauses = new List<DataPinFilterClause>();
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
            if (Clauses == null || Clauses.Count == 0) return !IsBlack;

            // if whitelist, any matching filter is ok (OR)
            bool match = Clauses.Any(f => f.IsMatch(pinName, partTypeId, partRoleId));
            return IsBlack ? !match : match;
        }
    }
}
