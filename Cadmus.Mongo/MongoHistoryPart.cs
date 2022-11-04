using Cadmus.Core;
using Cadmus.Core.Storage;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Cadmus.Mongo
{
    /// <summary>
    /// Mongo history part.
    /// </summary>
    /// <seealso cref="Cadmus.Core.Storage.IHasHistory" />
    public class MongoHistoryPart : IHasHistory
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public const string COLLECTION = "history-parts";

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
        public string? RoleId { get; set; }

        /// <summary>
        /// Gets or sets the thesaurus scope. This is an arbitrary string
        /// representing the suffix to be appended to a thesaurus ID (just
        /// before the <c>@</c> introducing its language ID) when loading
        /// thesauri. The suffix is appended with a leading dot, and should
        /// include only letters A-Z or a-z, digits, dash, and underscore.
        /// </summary>
        /// <remarks>This scope is used for those parts which require to
        /// load different thesauri according to runtime data. For instance,
        /// an apparatus part might require to load a different set of witnesses
        /// according to the work it refers to. Thus, an editor could append
        /// the part's thesaurus scope to its required thesaurus ID before
        /// loading it. For instance, say the part wants to load a thesaurus
        /// with ID <c>witnesses@en</c>: if the part has its
        /// <see cref="ThesaurusScope"/> equal to <c>lucr</c>, the editor will
        /// rather load <c>witnesses.lucr@en</c>.
        /// </remarks>
        public string? ThesaurusScope { get; set; }

        /// <summary>
        /// Gets or sets the encoded content representing this part.
        /// </summary>
        public BsonDocument? Content { get; set; }

        /// <summary>
        /// Creation date and time (UTC).
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// ID of the user who created the resource.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// ID of the user who last saved the resource.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data record (part) this history
        /// record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoHistoryPart"/> class.
        /// </summary>
        public MongoHistoryPart()
        {
            Id = Guid.NewGuid().ToString();
            ItemId = TypeId = "";
            CreatorId = UserId = "";
            ReferenceId = "";
            TimeCreated = TimeModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoHistoryPart"/> class.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException">part</exception>
        public MongoHistoryPart(IPart part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            Id = Guid.NewGuid().ToString();
            ItemId = part.ItemId;
            TypeId = part.TypeId;
            RoleId = part.RoleId;
            ThesaurusScope = part.ThesaurusScope;

            TimeCreated = part.TimeCreated;
            CreatorId = part.CreatorId;
            TimeModified = part.TimeModified;
            UserId = part.UserId;

            ReferenceId = part.Id;
            Status = EditStatus.Created;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoHistoryPart"/> class.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException">part</exception>
        public MongoHistoryPart(MongoPart part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            Id = Guid.NewGuid().ToString();
            ItemId = part.ItemId;
            TypeId = part.TypeId;
            RoleId = part.RoleId;
            ThesaurusScope = part.ThesaurusScope;
            Content = part.Content;

            TimeCreated = part.TimeCreated;
            CreatorId = part.CreatorId;
            TimeModified = part.TimeModified;
            UserId = part.UserId;

            ReferenceId = part.Id;
            Status = EditStatus.Created;
        }

        /// <summary>
        /// Gets a <see cref="HistoryPartInfo"/> from this object.
        /// </summary>
        /// <returns>Info.</returns>
        public HistoryPartInfo ToHistoryPartInfo()
        {
            return new HistoryPartInfo
            {
                Id = Id,
                ItemId = ItemId,
                TypeId = TypeId,
                RoleId = RoleId,

                TimeCreated = TimeCreated,
                CreatorId = CreatorId,
                TimeModified = TimeModified,
                UserId = UserId,

                ReferenceId = ReferenceId,
                Status = Status
            };
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} -> {ReferenceId}: {Status}";
        }
    }
}
