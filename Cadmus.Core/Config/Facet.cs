using System.Collections.Generic;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// An item's facet.
    /// </summary>
    public class Facet : IFacet
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
        public string Description { get; set; }

        /// <summary>
        /// Gets the part definitions.
        /// </summary>
        public List<PartDefinition> PartDefinitions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Facet"/> class.
        /// </summary>
        public Facet()
        {
            PartDefinitions = new List<PartDefinition>();
        }

        /// <summary>Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Id}: {Label}";
        }
    }
}
