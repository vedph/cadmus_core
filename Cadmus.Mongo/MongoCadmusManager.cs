using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Cadmus.Core.Storage;
using Cadmus.Core.Config;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB-based database manager.
    /// </summary>
    public sealed class MongoCadmusManager : ICadmusManager
    {
        private readonly Regex _dbAndParamsRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCadmusManager"/>
        /// class.
        /// </summary>
        public MongoCadmusManager()
        {
            _dbAndParamsRegex = new Regex(@"^(mongodb://[^/]+)([^?]*)(\?.+)?$");

            // camel case everything:
            // https://stackoverflow.com/questions/19521626/mongodb-convention-packs/19521784#19521784
            ConventionPack pack = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("camel case", pack, _ => true);
        }

        private string GetDatabaseName(string connectionString)
        {
            Match m = _dbAndParamsRegex.Match(connectionString);
            return m.Success ? m.Groups[2].Value.Trim('/') : null;
        }

        //private static void CreateIndexes(IMongoDatabase db)
        //{
        //    // items
        //    var itemsCollection = db.GetCollection<StoredItem>(StoredItem.COLLECTION);
        //    IndexKeysDefinitionBuilder<StoredItem> itemKdb = new IndexKeysDefinitionBuilder<StoredItem>();
        //    itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(itemKdb.Ascending(i => i.Title)));
        //    itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(itemKdb.Ascending(i => i.FacetId)));
        //    itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(itemKdb.Ascending(i => i.SortKey)));
        //    itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(itemKdb.Ascending(i => i.Flags)));
        //    itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(itemKdb.Ascending(i => i.UserId)));

        //    //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.Title));
        //    //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.FacetId));
        //    //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.SortKey));
        //    //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.Flags));
        //    //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.UserId));

        //    // history-items
        //    var historyCollection = db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);
        //    IndexKeysDefinitionBuilder<StoredHistoryItem> histKdb = new IndexKeysDefinitionBuilder<StoredHistoryItem>();
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.ReferenceId)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.Status)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.Title)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.FacetId)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.SortKey)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.Flags)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.UserId)));
        //    historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(histKdb.Ascending(i => i.TimeModified)));

        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.ReferenceId));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Status));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Title));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.FacetId));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.SortKey));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Flags));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.UserId));
        //    //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.TimeModified));
        //}

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <param name="profile">The database profile.</param>
        /// <exception cref="ArgumentNullException">null source or profile</exception>
        public void CreateDatabase(string source, string profile)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            // load profile
            XElement profileElement = XElement.Parse(profile);
            DataProfile p = new DataProfile(profileElement);

            // store facets, flags, and tag sets
            MongoClient client = new MongoClient(source);
            IMongoDatabase db = client.GetDatabase(GetDatabaseName(source));

            if (p.Facets.Length > 0)
            {
                var collFacets = db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION);
                collFacets.InsertMany(p.Facets.Select(f => new StoredItemFacet(f)));
            }

            if (p.Flags.Length > 0)
            {
                var collFlags = db.GetCollection<StoredFlagDefinition>(StoredFlagDefinition.COLLECTION);
                collFlags.InsertMany(p.Flags.Select(f => new StoredFlagDefinition(f)));
            }

            if (p.TagSets.Length > 0)
            {
                var collSets = db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION);
                collSets.InsertMany(p.TagSets.Select(s => new StoredTagSet(s)));
            }

            // add indexes
            MongoCadmusRepository.CreateIndexes(db);
            // CreateIndexes(db);
        }

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <exception cref="ArgumentNullException">null source</exception>
        public void DeleteDatabase(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            MongoClient client = new MongoClient(source);
            client.DropDatabase(GetDatabaseName(source));
        }

        /// <summary>
        /// Clears the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        public void ClearDatabase(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            MongoClient client = new MongoClient(source);
            IMongoDatabase db = client.GetDatabase(GetDatabaseName(source));

            // items
            db.GetCollection<StoredItem>(StoredItem.COLLECTION).DeleteMany(_ => true);
            // history-items
            db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION).DeleteMany(_ => true);
            // facets
            db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION).DeleteMany(_ => true);
            // flags
            db.GetCollection<StoredFlagDefinition>(StoredFlagDefinition.COLLECTION).DeleteMany(_ => true);
            // sets
            db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION).DeleteMany(_ => true);
        }

        private static IEnumerable<BsonDocument> Enumerate(IAsyncCursor<BsonDocument> docs)
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

            MongoClient client = new MongoClient(source);
            string databaseName = GetDatabaseName(source);

            // http://stackoverflow.com/questions/7049722/check-if-mongodb-database-exists
            var dbList = Enumerate(client.ListDatabases()).Select(db => db.GetValue("name").AsString);
            return dbList.Contains(databaseName);
        }
    }
}
