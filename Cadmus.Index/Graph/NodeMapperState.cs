using Cadmus.Core;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The state of a <see cref="NodeMapper"/>. This also serves as a data
    /// source for setting the value of <see cref="NodeMappingVariable"/>'s.
    /// </summary>
    public class NodeMapperState
    {
        /// <summary>
        /// Gets or sets the current source ID.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets the item. For a part, this is the container item.
        /// </summary>
        public IItem Item { get; set; }

        /// <summary>
        /// Gets or sets the ordinal number of the item's group ID being mapped,
        /// used when mapping an item's group with a composite ID.
        /// </summary>
        public int GroupOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the part.
        /// </summary>
        public IPart Part { get; set; }

        /// <summary>
        /// Gets or sets the name of the source pin.
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// Gets or sets the value of the source pin.
        /// </summary>
        public string PinValue { get; set; }

        /// <summary>
        /// Gets or sets the "mappings path", i.e. the list of all the mappings
        /// IDs starting from the root mapping and walking down along its
        /// descendants, up to the current mapping. This is maintained by the
        /// caller when applying mappings, and used to walk up the path in
        /// setting variable values.
        /// </summary>
        public IList<int> MappingPath { get; }

        /// <summary>
        /// Gets the mapping between node mapping IDs (keys) and the generated
        /// node URIs (UIDs, values). This is maintained by the caller when
        /// applying mappings, and used to get the target node URI of each
        /// mapping rule applied so far.
        /// </summary>
        public Dictionary<int, string> MappedUris { get; }

        /// <summary>
        /// Gets the mapping identifier for the node mapping corresponding to
        /// the current item (=the item being mapped, or the item including
        /// the parts being mapped).
        /// </summary>
        public int ItemMappingId { get; }

        /// <summary>
        /// Gets the mapping identifier for the node mapping corresponding
        /// to the current item's facet.
        /// </summary>
        public int FacetMappingId { get; }

        /// <summary>
        /// Gets the UIDs corresponding to the node corresponding to each group
        /// ID component (or just to the unique group ID when this is not
        /// composite) of the current item.
        /// </summary>
        public IList<string> GroupUids { get; }

        /// <summary>
        /// Gets the nodes generated in the current mapping session.
        /// </summary>
        public IList<Node> Nodes { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMapperState" />
        /// class.
        /// </summary>
        public NodeMapperState()
        {
            MappedUris = new Dictionary<int, string>();
            MappingPath = new List<int>();
            GroupUids = new List<string>();
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Sid);

            sb.Append(": ");
            if (Item != null) sb.Append('I');
            if (Part != null) sb.Append('P');
            if (!string.IsNullOrEmpty(PinName)) sb.Append('N');
            if (PinValue != null) sb.Append('V');

            sb.Append(" - ").Append(Nodes.Count);

            return sb.ToString();
        }
    }
}
