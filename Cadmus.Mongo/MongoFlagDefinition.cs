using System;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo;

/// <summary>
/// Flag definition storage model.
/// </summary>
public sealed class MongoFlagDefinition
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public const string COLLECTION = "flags";

    /// <summary>
    /// Gets or sets the bit value, representing the ID of the flag.
    /// </summary>
    [BsonId]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the label.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the description.
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
    /// Initializes a new instance of the <see cref="MongoFlagDefinition"/>
    /// class.
    /// </summary>
    public MongoFlagDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoFlagDefinition"/>
    /// class.
    /// </summary>
    /// <param name="definition">The definition to get data from.</param>
    /// <exception cref="ArgumentNullException">null definition</exception>
    public MongoFlagDefinition(FlagDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        Id = definition.Id;
        Label = definition.Label;
        Description = definition.Description;
        ColorKey = definition.ColorKey;
    }

    /// <summary>
    /// Get a <see cref="FlagDefinition"/> from this object.
    /// </summary>
    /// <returns>The flag definition.</returns>
    public FlagDefinition ToFlagDefinition()
    {
        return new FlagDefinition
        {
            Id = Id,
            Label = Label ?? "",
            Description = Description,
            ColorKey = ColorKey
        };
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{Id:X4} {Label}";
    }
}
