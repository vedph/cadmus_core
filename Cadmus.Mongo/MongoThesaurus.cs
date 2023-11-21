using System;
using System.Collections.Generic;
using Cadmus.Core.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace Cadmus.Mongo;

/// <summary>
/// Mongo thesaurus.
/// </summary>
public class MongoThesaurus
{
    /// <summary>
    /// The collection name.
    /// </summary>
    public const string COLLECTION = "thesauri";

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [BsonId]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the target identifier (for aliases).
    /// </summary>
    public string? TargetId { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public List<MongoThesaurusEntry> Entries { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoThesaurus"/> class.
    /// </summary>
    public MongoThesaurus()
    {
        Id = "";
        Entries = new List<MongoThesaurusEntry>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoThesaurus"/> class.
    /// </summary>
    /// <param name="thesaurus">The set to load data from.</param>
    /// <exception cref="ArgumentNullException">null set</exception>
    public MongoThesaurus(Thesaurus thesaurus)
    {
        ArgumentNullException.ThrowIfNull(thesaurus);

        Id = thesaurus.Id;
        TargetId = thesaurus.TargetId;
        Entries = new List<MongoThesaurusEntry>();
        if (thesaurus.Entries != null)
        {
            foreach (ThesaurusEntry entry in thesaurus.Entries)
                Entries.Add(new MongoThesaurusEntry(entry));
        }
    }

    /// <summary>
    /// Gets a thesaurus from this object.
    /// </summary>
    /// <returns>Thesaurus.</returns>
    public Thesaurus ToThesaurus()
    {
        Thesaurus thesaurus = new(Id!)
        {
            TargetId = TargetId
        };
        foreach (MongoThesaurusEntry entry in Entries)
            thesaurus.AddEntry(entry.ToThesaurusEntry());
        return thesaurus;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{Id} ({Entries?.Count})";
    }
}
