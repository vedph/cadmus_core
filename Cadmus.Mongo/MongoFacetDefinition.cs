using System;
using System.Collections.Generic;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo;

/// <summary>
/// Item's facet storage model.
/// </summary>
public sealed class MongoFacetDefinition
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public const string COLLECTION = "facets";

    /// <summary>
    /// Gets or sets the facet's identifier.
    /// </summary>
    [BsonId]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the facet's label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the facet's description.
    /// </summary>
    [BsonIgnoreIfNull]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color key.
    /// </summary>
    /// <value>
    /// The color key, with format RRGGBB.
    /// </value>
    [BsonIgnoreIfNull]
    public string? ColorKey { get; set; }

    /// <summary>
    /// Gets the part definitions.
    /// </summary>
    public List<PartDefinition> PartDefinitions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFacetDefinition"/> class.
    /// </summary>
    public MongoFacetDefinition()
    {
        PartDefinitions = new List<PartDefinition>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFacetDefinition"/> class.
    /// </summary>
    /// <param name="facet">The facet.</param>
    /// <exception cref="ArgumentNullException">null facet</exception>
    public MongoFacetDefinition(FacetDefinition facet)
    {
        if (facet == null) throw new ArgumentNullException(nameof(facet));

        Id = facet.Id;
        Label = facet.Label;
        Description = facet.Description;
        ColorKey = facet.ColorKey;
        PartDefinitions = new List<PartDefinition>(facet.PartDefinitions);
    }

    /// <summary>
    /// Get a <see cref="FacetDefinition"/> from this object.
    /// </summary>
    /// <returns>Facet definition.</returns>
    public FacetDefinition ToFacetDefinition()
    {
        FacetDefinition definition = new()
        {
            Id = Id!,
            Label = Label ?? "",
            Description = Description,
            ColorKey = ColorKey
        };
        definition.PartDefinitions.AddRange(PartDefinitions);
        return definition;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{Id}: {Label}" + (PartDefinitions.Count > 0 ?
            $" ({PartDefinitions.Count})" : "");
    }
}
