using Cadmus.Core;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text;

namespace Cadmus.Mongo
{
    /// <summary>
    /// Mongo part.
    /// </summary>
    /// <seealso cref="Cadmus.Core.IHasVersion" />
    public class MongoPart : IHasVersion
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string COLLECTION = "parts";

        /// <summary>
        /// Gets or sets the history record identifier.
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the item the parts belongs to.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the part's type identifier.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <remarks>Parts of the same type in the same item can have different
        /// roles. For instance, two date parts may refer to the date of the
        /// original text and to that of its later copy. In this case, a role
        /// ID helps selecting the desired part from an item.</remarks>
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the encoded content representing this part.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.
        /// </remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPart"/> class.
        /// </summary>
        public MongoPart()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPart"/> class.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException">part</exception>
        public MongoPart(IPart part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            Id = part.Id;
            ItemId = part.ItemId;
            TypeId = part.TypeId;
            RoleId = part.RoleId;

            TimeModified = part.TimeModified;
            UserId = part.UserId;
        }

        /// <summary>
        /// Gets a <see cref="PartInfo"/> from this object.
        /// </summary>
        /// <returns>The part info.</returns>
        public PartInfo ToPartInfo()
        {
            return new PartInfo
            {
                Id = Id,
                ItemId = ItemId,
                TypeId = TypeId,
                RoleId = RoleId,
                UserId = UserId,
                TimeModified = TimeModified
            };
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id).Append(' ').Append(TypeId);
            if (RoleId != null) sb.Append(" [").Append(RoleId).Append(']');
            return sb.ToString();
        }
    }
}
