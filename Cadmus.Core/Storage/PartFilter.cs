using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Item's part filter.
    /// </summary>
    public class PartFilter : VersionFilter
    {
        /// <summary>
        /// Gets or sets the item(s) identifier(s).
        /// </summary>
        public string[] ItemIds { get; set; }

        /// <summary>
        /// Gets or sets the part type identifier filter.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the part's role identifier filter.
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the part's thesaurus scope.
        /// </summary>
        public string ThesaurusScope { get; set; }

        /// <summary>
        /// Gets or sets the optional custom sort expressions to be used to
        /// sort the filtered parts.
        /// This can be useful when retrieving parts from several items, e.g.
        /// hierarchy parts which usually are sorted by their Y and X values.
        /// </summary>
        /// <value>Tuples where 1=field name and 2=true for ascending,
        /// false for descending</value>
        public Tuple<string,bool>[] SortExpressions { get; set; }
    }
}
