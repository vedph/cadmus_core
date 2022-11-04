using System.Collections.Generic;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// An item's facet definition.
    /// </summary>
    public class FacetDefinition
    {
        /// <summary>
        /// Gets or sets the facet's identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the facet's label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the facet's description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the color key.
        /// </summary>
        /// <value>
        /// The color key, with format RRGGBB.
        /// </value>
        public string? ColorKey { get; set; }

        /// <summary>
        /// Gets the part definitions.
        /// </summary>
        public List<PartDefinition> PartDefinitions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetDefinition"/> class.
        /// </summary>
        public FacetDefinition()
        {
            Id = Label = "";
            PartDefinitions = new List<PartDefinition>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Label}" + (PartDefinitions.Count > 0?
                $" ({PartDefinitions.Count})" : "");
        }
    }
}
