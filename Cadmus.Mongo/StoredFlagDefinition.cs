using System;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo
{
    /// <summary>
    /// Flag definition storage model.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Config.IFlagDefinition" />
    public sealed class StoredFlagDefinition : IFlagDefinition
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
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [BsonIgnoreIfNull]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the color key.
        /// </summary>
        /// <value>
        /// The color key, with format RRGGBB.
        /// </value>
        [BsonIgnoreIfNull]
        public string ColorKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredFlagDefinition"/>
        /// class.
        /// </summary>
        public StoredFlagDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredFlagDefinition"/>
        /// class.
        /// </summary>
        /// <param name="definition">The definition to get data from.</param>
        /// <exception cref="ArgumentNullException">null definition</exception>
        public StoredFlagDefinition(IFlagDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            Id = definition.Id;
            Label = definition.Label;
            Description = definition.Description;
            ColorKey = definition.ColorKey;
        }
    }
}
