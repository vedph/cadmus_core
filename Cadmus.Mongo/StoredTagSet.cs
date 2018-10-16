using System;
using System.Collections.Generic;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo
{
    /// <summary>
    /// Stored tag set.
    /// </summary>
    public sealed class StoredTagSet : ITagSet
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string COLLECTION = "tags";

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredTagSet"/> class.
        /// </summary>
        /// <param name="set">The set to load data from.</param>
        /// <exception cref="ArgumentNullException">null set</exception>
        public StoredTagSet(TagSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            Id = set.Id;
            Tags = set.Tags;
        }
    }
}
