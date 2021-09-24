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
        /// Gets the mapped uris.
        /// </summary>
        public Dictionary<int, string> MappedUris { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMappingVariableSource"/>
        /// class.
        /// </summary>
        /// <param name="mappedUris">The mapping between node mapping IDs (keys)
        /// and the generated node URIs (UIDs, values). This is maintained
        /// by the caller when applying mappings.</param>
        /// <exception cref="ArgumentNullException">mappedUris</exception>
        public NodeMappingVariableSource(Dictionary<int, string> mappedUris)
        {
            MappedUris = mappedUris
                ?? throw new ArgumentNullException(nameof(mappedUris));
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
