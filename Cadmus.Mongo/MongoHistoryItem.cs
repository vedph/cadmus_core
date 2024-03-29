﻿using System;
using Cadmus.Core;
using Cadmus.Core.Storage;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo;

/// <summary>
/// A history item as stored in Mongo database.
/// </summary>
public sealed class MongoHistoryItem : IHasHistory, IHasFlags
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public const string COLLECTION = "history-items";

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
    [BsonIgnoreIfNull]
    public string Description { get; set; }

    /// <summary>
    /// Item's facet ID.
    /// </summary>
    /// <value>The facet defines which parts can be stored in the item,
    /// and their order and other presentational attributes. It is a unique
    /// string defined in the corpus configuration.</value>
    public string FacetId { get; set; }

    /// <summary>
    /// Gets or sets the group identifier. This is an arbitrary string
    /// which can be used to group items into a set. For instance, you
    /// might have a set of items belonging to the same literary work,
    /// a set of lemmata belonging to the same dictionary letter, etc.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// The sort key for the item. This is a value used to sort items in
    /// a list.
    /// </summary>
    public string SortKey { get; set; }

    /// <summary>
    /// Gets or sets generic flags for the item.
    /// </summary>
    public int Flags { get; set; }

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
    /// Gets or sets the identifier of the data record this history record
    /// refers to.
    /// </summary>
    public string ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the record status.
    /// </summary>
    public EditStatus Status { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHistoryItem"/> class.
    /// </summary>
    public MongoHistoryItem()
    {
        Id = Guid.NewGuid().ToString();
        Title = Description = "";
        FacetId = "default";
        SortKey = "";
        CreatorId = UserId = "";
        ReferenceId = "";
        TimeCreated = TimeModified = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHistoryItem"/>
    /// class.
    /// </summary>
    /// <param name="item">The source item.</param>
    /// <exception cref="ArgumentNullException">null item</exception>
    public MongoHistoryItem(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = Guid.NewGuid().ToString();
        ReferenceId = item.Id;
        Title = item.Title;
        Description = item.Description;
        FacetId = item.FacetId;
        GroupId = item.GroupId;
        SortKey = item.SortKey;
        Flags = item.Flags;

        TimeCreated = item.TimeCreated;
        CreatorId = item.CreatorId;
        TimeModified = item.TimeModified;
        UserId = item.UserId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHistoryItem"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">null item</exception>
    public MongoHistoryItem(MongoItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = Guid.NewGuid().ToString();
        ReferenceId = item.Id;
        Title = item.Title;
        Description = item.Description;
        FacetId = item.FacetId;
        GroupId = item.GroupId;
        SortKey = item.SortKey;
        Flags = item.Flags;
        TimeCreated = item.TimeCreated;
        CreatorId = item.CreatorId;
        TimeModified = item.TimeModified;
        UserId = item.UserId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoHistoryItem"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">null item</exception>
    public MongoHistoryItem(HistoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = Guid.NewGuid().ToString();
        Title = item.Title;
        Description = item.Description;
        FacetId = item.FacetId;
        GroupId = item.GroupId;
        SortKey = item.SortKey;
        Flags = item.Flags;

        TimeCreated = item.TimeCreated;
        CreatorId = item.CreatorId;
        TimeModified = item.TimeModified;
        UserId = item.UserId;

        ReferenceId = item.Id;
        Status = item.Status;
    }

    /// <summary>
    /// Get a <see cref="HistoryItem"/> from this object.
    /// </summary>
    /// <returns>History item.</returns>
    public HistoryItem ToHistoryItem()
    {
        return new HistoryItem(Id!, ReferenceId)
        {
            Title = Title!,
            Description = Description!,
            FacetId = FacetId!,
            GroupId = GroupId,
            SortKey = SortKey!,
            Flags = Flags,

            TimeCreated = TimeCreated,
            CreatorId = CreatorId,
            TimeModified = TimeModified,
            UserId = UserId,

            Status = Status
        };
    }

    /// <summary>
    /// Get a <see cref="HistoryItemInfo"/> from this object.
    /// </summary>
    /// <returns>History item info.</returns>
    public HistoryItemInfo ToHistoryItemInfo()
    {
        return new HistoryItemInfo(Id!, ReferenceId)
        {
            Title = Title,
            Description = Description,
            FacetId = FacetId,
            GroupId = GroupId,
            SortKey = SortKey,
            Flags = Flags,

            TimeCreated = TimeCreated,
            CreatorId = CreatorId,
            TimeModified = TimeModified,
            UserId = UserId,

            Status = Status
        };
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        return $"{Id}: {Title}" + (FacetId != null ? $" [{FacetId}]" : "");
    }
}
