using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Fusi.Tools.Data;

namespace Cadmus.Mongo.Test;

/// <summary>
/// MongoDB helper methods.
/// </summary>
public static class MongoHelper
{
    /// <summary>
    /// Gets the specified virtual page of documents.
    /// </summary>
    /// <typeparam name="T">The strong type of document, or just 
    /// <see cref="BsonDocument"/></typeparam>
    /// <param name="collection">The collection to load data from.</param>
    /// <param name="query">The query JSON string.</param>
    /// <param name="sort">The sort JSON string.</param>
    /// <param name="number">The page number (1-N).</param>
    /// <param name="size">The page size.</param>
    /// <returns>page</returns>
    /// <exception cref="ArgumentNullException">null collection, query or
    /// sort</exception>
    /// <exception cref="ArgumentOutOfRangeException">number or size less
    /// than 1</exception>
    public static DataPage<T> GetDocumentsPage<T>(IMongoCollection<T> collection,
        string query, string sort, int number, int size) where T : class
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(sort);
        if (number < 1) throw new ArgumentOutOfRangeException(nameof(number));
        if (size < 1) throw new ArgumentOutOfRangeException(nameof(size));

        BsonDocument docQuery = BsonDocument.Parse(query);
        BsonDocument docSort = BsonDocument.Parse(sort);

        int total = (int)collection.CountDocuments(docQuery);

        return new DataPage<T>(number, size, total,
            collection.Find(docQuery)
            .Sort(docSort).Skip((number - 1) * size).Limit(size)
            .ToList());
    }
}
