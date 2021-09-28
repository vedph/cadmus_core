using Fusi.Tools.Data;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Filter for <see cref="Triple"/>.
    /// </summary>
    /// <seealso cref="PagingOptions" />
    public class TripleFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the subject node identifier to match.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the predicate node identifier to match.
        /// </summary>
        public int PredicateId { get; set; }

        /// <summary>
        /// Gets or sets the object identifier to match.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object literal regular expression to match.
        /// </summary>
        public string ObjectLiteral { get; set; }

        /// <summary>
        /// Gets or sets the sid.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Sid"/> represents
        /// the initial portion of the SID being searched, rather than the
        /// full SID.
        /// </summary>
        public bool IsSidPrefix { get; set; }

        /// <summary>
        /// Gets or sets the tag filter to match. If null, no tag filtering
        /// is applied; if empty, only triples with a null tag are matched;
        /// otherwise, the triples with the same tag must be matched.
        /// </summary>
        public string Tag { get; set; }
    }
}
