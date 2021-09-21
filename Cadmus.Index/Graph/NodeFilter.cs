using Fusi.Tools.Data;
using System.Collections.Generic;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A filter for <see cref="Node"/>.
    /// </summary>
    public class NodeFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets any portion of the node's UID to match.
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets any portion of the label to match.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        public NodeSourceType? SourceType { get; set; }

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
        /// Gets or sets the identifier of a node which is directly linked
        /// to the nodes being searched.
        /// </summary>
        public int LinkedNodeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the role of the node identified by
        /// <see cref="LinkedNodeId"/>: <c>S</c>=subject, <c>O</c>=object,
        /// else no role filtering.
        /// </summary>
        public char LinkedNodeRole { get; set; }

        /// <summary>
        /// Gets or sets the classes identifiers to match only those nodes
        /// which are inside any of the listed classes.
        /// </summary>
        public List<string> ClassIds { get; set; }
    }
}
