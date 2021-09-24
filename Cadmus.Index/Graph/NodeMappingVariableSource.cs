using Cadmus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The source data for setting the value of <see cref="NodeMappingVariable"/>'s.
    /// </summary>
    public class NodeMappingVariableSource
    {
        /// <summary>
        /// Gets or sets the item. For a part, this is the container item.
        /// </summary>
        public IItem Item { get; set; }

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
        /// Gets or sets the mapping identifier for the node mapping
        /// corresponding to the current item (=the item being mapped, or the
        /// item including the parts being mapped).
        /// </summary>
        public int ItemMappingId { get; set; }

        /// <summary>
        /// Gets or sets the mapping identifier for the node mapping corresponding
        /// to the current item's facet.
        /// </summary>
        public int FacetMappingId { get; set; }

        /// <summary>
        /// Gets the UIDs corresponding to the node corresponding to each group
        /// ID component (or just to the unique group ID when this is not
        /// composite) of the current item.
        /// </summary>
        public IList<string> GroupUids { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMappingVariableSource" />
        /// class.
        /// </summary>
        /// <param name="mappedUris">The mapped URIs.</param>
        /// <param name="mappingPath">The mapping path.</param>
        /// <exception cref="ArgumentNullException">mappedUris or mappingPath
        /// </exception>
        public NodeMappingVariableSource(Dictionary<int, string> mappedUris,
            IList<int> mappingPath)
        {
            MappedUris = mappedUris
                ?? throw new ArgumentNullException(nameof(mappedUris));
            MappingPath = mappingPath
                ?? throw new ArgumentNullException(nameof(mappingPath));
            GroupUids = new List<string>();
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
            if (Item != null) sb.Append('I');
            if (Part != null) sb.Append('P');
            if (!string.IsNullOrEmpty(PinName)) sb.Append('N');
            if (PinValue != null) sb.Append('V');
            return sb.ToString();
        }
    }
}
