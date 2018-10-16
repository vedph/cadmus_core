using System;
using System.Collections.Generic;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo
{
    /// <summary>
    /// Item's facet storage model.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Config.IFacet" />
    public sealed class StoredItemFacet : IFacet
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string COLLECTION = "facets";

        /// <summary>
        /// Gets or sets the facet's identifier.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the facet's label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the facet's description.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Description { get; set; }

        /// <summary>
        /// Gets the part definitions.
        /// </summary>
        public List<PartDefinition> PartDefinitions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredItemFacet"/> class.
        /// </summary>
        public StoredItemFacet()
        {
            PartDefinitions = new List<PartDefinition>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredItemFacet"/> class.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <exception cref="ArgumentNullException">null facet</exception>
        public StoredItemFacet(IFacet facet)
        {
            if (facet == null) throw new ArgumentNullException(nameof(facet));

            Id = facet.Id;
            Label = facet.Label;
            Description = facet.Description;
            PartDefinitions = new List<PartDefinition>(facet.PartDefinitions);
        }
    }
}
