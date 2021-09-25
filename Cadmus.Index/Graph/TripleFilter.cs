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
    }
}
