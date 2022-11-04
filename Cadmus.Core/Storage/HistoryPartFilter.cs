using System;
using System.Collections.Generic;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Filter for history parts.
    /// </summary>
    /// <seealso cref="HistoryFilter" />
    public class HistoryPartFilter : HistoryFilter
    {
        /// <summary>
        /// Gets or sets the item(s) identifier(s).
        /// </summary>
        public IList<string>? ItemIds { get; set; }

        /// <summary>
        /// Gets or sets the part type identifier filter.
        /// </summary>
        public string? TypeId { get; set; }

        /// <summary>
        /// Gets or sets the part's role identifier filter.
        /// </summary>
        public string? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the optional custom sort expressions to be used to
        /// sort the filtered parts.
        /// This can be useful when retrieving parts from several items, e.g.
        /// hierarchy parts which usually are sorted by their Y and X values.
        /// </summary>
        /// <value>Tuples where 1=field name and 2=true for ascending,
        /// false for descending</value>
        public IList<Tuple<string, bool>>? SortExpressions { get; set; }
    }
}
