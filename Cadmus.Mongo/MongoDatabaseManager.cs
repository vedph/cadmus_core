using System;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core.Storage;
using Cadmus.Core.Config;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB-based database manager.
    /// </summary>
    public sealed class MongoDatabaseManager : MongoConsumerBase, IDatabaseManager
    {
        private void CreateIndexes(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            // items
            var itemCollection = database.GetCollection<MongoItem>
                (MongoItem.COLLECTION);
            itemCollection.Indexes.CreateOne(new CreateIndexModel<MongoItem>(
                Builders<MongoItem>.IndexKeys.Ascending(i => i.Title)));
            itemCollection.Indexes.CreateOne(new CreateIndexModel<MongoItem>(
                Builders<MongoItem>.IndexKeys.Ascending(i => i.Description)));
            itemCollection.Indexes.CreateOne(new CreateIndexModel<MongoItem>(
                Builders<MongoItem>.IndexKeys.Ascending(i => i.FacetId)));
            itemCollection.Indexes.CreateOne(new CreateIndexModel<MongoItem>(
                Builders<MongoItem>.IndexKeys.Ascending(i => i.SortKey)));
            itemCollection.Indexes.CreateOne(new CreateIndexModel<MongoItem>(
                Builders<MongoItem>.IndexKeys.Ascending(i => i.Flags)));

            // parts
            var partCollection = database.GetCollection<MongoPart>
                (MongoPart.COLLECTION);
            partCollection.Indexes.CreateOne(new CreateIndexModel<MongoPart>(
                Builders<MongoPart>.IndexKeys.Ascending(p => p.ItemId)));
            partCollection.Indexes.CreateOne(new CreateIndexModel<MongoPart>(
                Builders<MongoPart>.IndexKeys.Ascending(p => p.TypeId)));
            partCollection.Indexes.CreateOne(new CreateIndexModel<MongoPart>(
                Builders<MongoPart>.IndexKeys.Ascending(p => p.RoleId)));

            // history-items
            var historyItemCollection = database.GetCollection<MongoHistoryItem>
                (MongoHistoryItem.COLLECTION);
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.Title)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.Description)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.FacetId)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.SortKey)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.Flags)));

            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.TimeModified)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.UserId)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.ReferenceId)));
            historyItemCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryItem>(
                Builders<MongoHistoryItem>.IndexKeys.Ascending(i => i.Status)));

            // history-parts
            var historyPartCollection = database.GetCollection<MongoHistoryPart>
                (MongoHistoryPart.COLLECTION);
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(p => p.ItemId)));
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(p => p.TypeId)));
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(p => p.RoleId)));

            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(i => i.TimeModified)));
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(i => i.UserId)));
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(i => i.ReferenceId)));
            historyPartCollection.Indexes.CreateOne(new CreateIndexModel<MongoHistoryPart>(
                Builders<MongoHistoryPart>.IndexKeys.Ascending(i => i.Status)));
        }

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <param name="profile">The database profile.</param>
        /// <exception cref="ArgumentNullException">null source or profile
        /// </exception>
        public void CreateDatabase(string source, DataProfile profile)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            EnsureClientCreated(source);

            // store facets, flags, and tag sets
            IMongoDatabase db = Client.GetDatabase(GetDatabaseName(source));

            if (profile.Facets?.Length > 0)
            {
                var collFacets = db.GetCollection<MongoFacetDefinition>(
                    MongoFacetDefinition.COLLECTION);
                collFacets.InsertMany(profile.Facets.Select(
                    f => new MongoFacetDefinition(f)));
            }

            if (profile.Flags?.Length > 0)
            {
                var collFlags = db.GetCollection<MongoFlagDefinition>(
                    MongoFlagDefinition.COLLECTION);
                collFlags.InsertMany(profile.Flags
                    .Select(f => new MongoFlagDefinition(f)));
            }

            if (profile.Thesauri?.Length > 0)
            {
                var collSets = db.GetCollection<MongoThesaurus>(
                    MongoThesaurus.COLLECTION);
                collSets.InsertMany(profile.Thesauri
                    .Select(s => new MongoThesaurus(s)));
            }

            // add indexes
            CreateIndexes(db);
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <exception cref="ArgumentNullException">null source</exception>
        public void DeleteDatabase(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            EnsureClientCreated(source);
            Client.DropDatabase(GetDatabaseName(source));
        }

        /// <summary>
        /// Clears the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        public void ClearDatabase(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            EnsureClientCreated(source);
            IMongoDatabase db = Client.GetDatabase(GetDatabaseName(source));

            // parts
            db.GetCollection<MongoPart>(MongoPart.COLLECTION)
                .DeleteMany(_ => true);
            // items
            db.GetCollection<MongoItem>(MongoItem.COLLECTION)
                .DeleteMany(_ => true);

            // history-parts
            db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                .DeleteMany(_ => true);
            // history-items
            db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION)
                .DeleteMany(_ => true);

            // facets
            db.GetCollection<MongoFacetDefinition>(MongoFacetDefinition.COLLECTION)
                .DeleteMany(_ => true);
            // flags
            db.GetCollection<MongoFlagDefinition>(MongoFlagDefinition.COLLECTION)
                .DeleteMany(_ => true);
            // thesauri
            db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION)
                .DeleteMany(_ => true);
        }

        private static IEnumerable<BsonDocument> Enumerate(
            IAsyncCursor<BsonDocument> docs)
        {
            while (docs.MoveNext())
            {
                foreach (BsonDocument item in docs.Current)
                    yield return item;
            }
        }

        /// <summary>
        /// Databases the exists.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <returns>true if the database exists</returns>
        public bool DatabaseExists(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            EnsureClientCreated(source);
            string databaseName = GetDatabaseName(source);

            // http://stackoverflow.com/questions/7049722/check-if-mongodb-database-exists
            var dbList = Enumerate(Client.ListDatabases())
                .Select(db => db.GetValue("name").AsString);
            return dbList.Contains(databaseName);
        }
    }
}
