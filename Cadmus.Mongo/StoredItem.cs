using System;
using Cadmus.Core.Blocks;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo
{
    /// <summary>
    /// An item as stored in the Mongo database.
    /// </summary>
    public class StoredItem
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string COLLECTION = "items";

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item's facet ID.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        public string FacetId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items in a list.
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.</remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredItem"/> class.
        /// </summary>
        public StoredItem()
        {
            TimeModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredItem"/> class.
        /// </summary>
        /// <param name="item">The source item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public StoredItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            FacetId = item.FacetId;
            SortKey = item.SortKey;
            Flags = item.Flags;
            UserId = item.UserId;
            TimeModified = item.TimeModified;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Title}";
        }
    }
}
