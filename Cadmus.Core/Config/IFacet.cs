using System.Collections.Generic;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// Item's facet.
    /// </summary>
    public interface IFacet
    {
        /// <summary>
        /// Gets or sets the facet's identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the facet's label.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets or sets the facet's description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets the part definitions.
        /// </summary>
        List<PartDefinition> PartDefinitions { get; }
    }
}