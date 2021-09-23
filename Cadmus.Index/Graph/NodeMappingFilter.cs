using Fusi.Tools.Data;
using System.Collections.Generic;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A filter for <see cref="NodeMapping"/>.
    /// </summary>
    public class NodeMappingFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the parent mapping's identifier.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the source types. Any of the specified types must
        /// be matched.
        /// </summary>
        public List<NodeSourceType> SourceTypes { get; set; }

        /// <summary>
        /// Gets or sets any portion of the name to match.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the facet ID to match.
        /// </summary>
        public string Facet { get; set; }

        /// <summary>
        /// Gets or sets the regular expression to match for the group ID.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the flags; all of them must be matched.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Gets or sets the regular expression to match for the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type ID of the source part to match.
        /// </summary>
        public string PartType { get; set; }

        /// <summary>
        /// Gets or sets the role ID of the source part to match.
        /// </summary>
        public string PartRole { get; set; }

        /// <summary>
        /// Gets or sets the name of the pin to match.
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMappingFilter"/>
        /// class.
        /// </summary>
        public NodeMappingFilter()
        {
            SourceTypes = new List<NodeSourceType>();
        }
    }
}
