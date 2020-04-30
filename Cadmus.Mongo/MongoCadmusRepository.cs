using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Layers;
using Cadmus.Core.Storage;
using DiffMatchPatch;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thesaurus = Cadmus.Core.Config.Thesaurus;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB based repository for Cadmus data.
    /// Tag: <c>cadmus-repository.mongo</c>.
    /// </summary>
    [Tag("cadmus-repository.mongo")]
    public sealed class MongoCadmusRepository : MongoConsumerBase,
        ICadmusRepository,
        IConfigurable<MongoCadmusRepositoryOptions>
    {
        private readonly IPartTypeProvider _partTypeProvider;
        private readonly IItemSortKeyBuilder _itemSortKeyBuilder;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly JsonWriterSettings _jsonSettings;
        private IEditOperationDiffAdapter<YXEditOperation> _opDiffAdapter;
        private MongoCadmusRepositoryOptions _options;
        private string _databaseName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCadmusRepository"/>
        /// class.
        /// </summary>
        /// <param name="partTypeProvider">The part type provider.</param>
        /// <param name="itemSortKeyBuilder">The item sort key builder.</param>
        /// <exception cref="ArgumentNullException">null options or part type
        /// provider</exception>
        public MongoCadmusRepository(
            IPartTypeProvider partTypeProvider,
            IItemSortKeyBuilder itemSortKeyBuilder)
        {
            _partTypeProvider = partTypeProvider ??
                throw new ArgumentNullException(nameof(partTypeProvider));
            _itemSortKeyBuilder = itemSortKeyBuilder ??
                throw new ArgumentNullException(nameof(itemSortKeyBuilder));

            _jsonSettings = new JsonWriterSettings
            {
                Indent = false
            };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(MongoCadmusRepositoryOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _databaseName = GetDatabaseName(_options.ConnectionString);
        }

        #region Flags
        /// <summary>
        /// Gets the flag definitions.
        /// </summary>
        /// <returns>definitions</returns>
        public IList<FlagDefinition> GetFlagDefinitions()
        {
            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var flags = db.GetCollection<MongoFlagDefinition>(
                MongoFlagDefinition.COLLECTION);

            return (from d in flags.Find(_ => true)
                    .SortBy(f => f.Id).ToList()
                    select d.ToFlagDefinition())
                    .ToList();
        }

        /// <summary>
        /// Gets the specified flag definition.
        /// </summary>
        /// <param name="id">The flag identifier.</param>
        /// <returns>definition or null if not found</returns>
        public FlagDefinition GetFlagDefinition(int id)
        {
            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var flags = db.GetCollection<MongoFlagDefinition>(
                MongoFlagDefinition.COLLECTION);

            return flags.Find(f => f.Id.Equals(id))
                .FirstOrDefault()?.ToFlagDefinition();
        }

        /// <summary>
        /// Adds or updates the specified flag definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <exception cref="ArgumentNullException">null definition</exception>
        public void AddFlagDefinition(FlagDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var flags = db.GetCollection<MongoFlagDefinition>(
                MongoFlagDefinition.COLLECTION);

            flags.ReplaceOne(f => f.Id.Equals(definition.Id),
                new MongoFlagDefinition(definition),
                new ReplaceOptions {IsUpsert = true});
        }

        /// <summary>
        /// Deletes the specified flag definition.
        /// </summary>
        /// <param name="id">The flag identifier.</param>
        public void DeleteFlagDefinition(int id)
        {
            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var flags = db.GetCollection<MongoFlagDefinition>(
                MongoFlagDefinition.COLLECTION);

            flags.DeleteOne(f => f.Id.Equals(id));
        }
        #endregion

        #region Facets
        /// <summary>
        /// Gets the item's facets.
        /// </summary>
        /// <returns>facets</returns>
        public IList<FacetDefinition> GetFacetDefinitions()
        {
            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var facets = db.GetCollection<MongoFacetDefinition>(
                MongoFacetDefinition.COLLECTION);

            return (from f in facets.Find(_ => true)
                    .SortBy(f => f.Label).ToList()
                select f.ToFacetDefinition()).ToList();
        }

        /// <summary>
        /// Gets the specified item's facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        /// <returns>facet or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public FacetDefinition GetFacetDefinition(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var facets = db.GetCollection<MongoFacetDefinition>(
                MongoFacetDefinition.COLLECTION);

            return facets.Find(f => f.Id.Equals(id)).FirstOrDefault()?
                .ToFacetDefinition();
        }

        /// <summary>
        /// Adds or updates the specified facet.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <exception cref="ArgumentNullException">null facet</exception>
        public void AddFacetDefinition(FacetDefinition facet)
        {
            if (facet == null) throw new ArgumentNullException(nameof(facet));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var facets = db.GetCollection<MongoFacetDefinition>(
                    MongoFacetDefinition.COLLECTION);

            facets.ReplaceOne(f => f.Id.Equals(facet.Id),
                new MongoFacetDefinition(facet),
                new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Deletes the specified facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteFacetDefinition(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var facets = db.GetCollection<MongoFacetDefinition>(
                MongoFacetDefinition.COLLECTION);

            facets.DeleteOne(f => f.Id.Equals(id));
        }
        #endregion

        #region Thesauri
        /// <summary>
        /// Gets the IDs of all the thesauri.
        /// </summary>
        /// <returns>IDs</returns>
        public IList<string> GetThesaurusIds()
        {
            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var thesauri = db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION);

            return (from set in thesauri.AsQueryable()
                orderby set.Id
                select set.Id).ToList();
        }

        /// <summary>
        /// Gets the specified page of thesauri.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<Thesaurus> GetThesauri(ThesaurusFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var thesauri = db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION)
                .AsQueryable();

            // apply filters
            if (!string.IsNullOrEmpty(filter.Id))
                thesauri = thesauri.Where(t => t.Id.Contains(filter.Id));

            if (filter.IsAlias != null)
            {
                thesauri = filter.IsAlias.Value
                    ? thesauri.Where(t => t.TargetId != null)
                    : thesauri.Where(t => t.TargetId == null);
            }

            if (!string.IsNullOrEmpty(filter.Language))
            {
                string suffix = "@" + filter.Language;
                thesauri = thesauri.Where(t => t.Id.EndsWith(suffix));
            }

            // get total
            int total = thesauri.Count();
            if (total == 0)
            {
                return new DataPage<Thesaurus>(
                    filter.PageNumber,
                    filter.PageSize,
                    0,
                    Array.Empty<Thesaurus>());
            }

            // get page
            var pagedThesauri = thesauri.OrderBy(t => t.Id)
                .Skip(filter.GetSkipCount())
                .Take(filter.PageSize)
                .ToList();

            return new DataPage<Thesaurus>(
                filter.PageNumber,
                filter.PageSize,
                total,
                new List<Thesaurus>(from t in pagedThesauri select t.ToThesaurus()));
        }

        /// <summary>
        /// Gets the thesaurus with the specified ID.
        /// </summary>
        /// <param name="id">The thesaurus ID. This usually should include the
        /// language code suffix. When this language is not found, the repository
        /// tries to fall back to these languages (in this order): <c>eng</c>,
        /// <c>en</c>, or no language at all. If there is still no match, null
        /// will be returned.</param>
        /// <returns>Thesaurus, or null if not found. Notice that when the
        /// requested thesaurus is an alias, it is its target thesaurus that
        /// will be returned, if any.</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public Thesaurus GetThesaurus(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var thesauri = db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION)
                .AsQueryable();

            MongoThesaurus mongo;
            do
            {
                mongo = thesauri.FirstOrDefault(set => set.Id == id);

                // if not found and not @en, try with @eng, @en
                if (mongo == null
                    && !id.EndsWith("@en", StringComparison.OrdinalIgnoreCase))
                {
                    string[] langs = new[] { "@eng", "@en", "" };
                    string bareId = Regex.Replace(id, @"\@[a-z]{2,3}$", "");

                    for (int i = 0; i < langs.Length; i++)
                    {
                        string fallbackId = bareId + langs[i];
                        mongo = thesauri.FirstOrDefault(set => set.Id == fallbackId);
                        if (mongo != null) break;
                    }
                }

                // if this is an alias, repeat the search
                if (mongo?.TargetId != null) id = mongo.TargetId;
            } while (mongo?.TargetId != null);

            return mongo?.ToThesaurus();
        }

        /// <summary>
        /// Adds or updates the specified thesaurus.
        /// </summary>
        /// <param name="thesaurus">The thesaurus.</param>
        /// <exception cref="ArgumentNullException">null thesaurus</exception>
        public void AddThesaurus(Thesaurus thesaurus)
        {
            if (thesaurus == null) throw new ArgumentNullException(nameof(thesaurus));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var thesauri = db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION);

            thesauri.ReplaceOne(old => old.Id.Equals(thesaurus.Id),
                new MongoThesaurus(thesaurus),
                new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Deletes the specified thesaurus.
        /// </summary>
        /// <param name="id">The thesaurus ID.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteThesaurus(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var thesauri = db.GetCollection<MongoThesaurus>(MongoThesaurus.COLLECTION);

            thesauri.DeleteOne(t => t.Id.Equals(id));
        }
        #endregion

        #region Items
        /// <summary>
        /// Gets a page of items.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>items page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public DataPage<ItemInfo> GetItems(ItemFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            var builder = new FilterDefinitionBuilder<MongoItem>();
            FilterDefinition<MongoItem> f = builder.Empty;

            if (!string.IsNullOrEmpty(filter.Title))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.Title.Contains(filter.Title)));
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.Description.Contains(filter.Description)));
            }

            if (!string.IsNullOrEmpty(filter.FacetId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.Description.Contains(filter.FacetId)));
            }

            if (!string.IsNullOrEmpty(filter.GroupId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.GroupId.Equals(filter.GroupId)));
            }

            if (filter.Flags.HasValue)
            {
                f = builder.And(builder.BitsAllSet(i => i.Flags, filter.Flags.Value));
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.UserId.Equals(filter.UserId)));
            }

            if (filter.MinModified.HasValue)
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.TimeModified >= filter.MinModified.Value));
            }

            if (filter.MaxModified.HasValue)
            {
                f = builder.And(new ExpressionFilterDefinition<MongoItem>(
                    i => i.TimeModified <= filter.MaxModified.Value));
            }

            int total = (int)items.CountDocuments(f);
            if (total == 0)
            {
                return new DataPage<ItemInfo>(
                    filter.PageNumber, filter.PageSize, 0, null);
            }

            var mongoItems = items.Find(f)
                .SortBy(i => i.SortKey)
                .ThenBy(i => i.Id)
                .Skip(filter.GetSkipCount())
                .Limit(filter.PageSize)
                .ToList();

            IList<ItemInfo> results =
                mongoItems.Select(i => i.ToItemInfo()).ToList();
            return new DataPage<ItemInfo>(
                filter.PageNumber, filter.PageSize, total, results);
        }

        /// <summary>
        /// Gets the specified item.
        /// </summary>
        /// <param name="id">The item's identifier.</param>
        /// <param name="includeParts">if set to <c>true</c>, include all the
        /// item's parts.</param>
        /// <returns>item or null if not found</returns>
        /// <exception cref="ArgumentNullException">null item ID</exception>
        public IItem GetItem(string id, bool includeParts = true)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            MongoItem mongoItem = items.Find(i => i.Id.Equals(id)).FirstOrDefault();
            if (mongoItem == null) return null;

            Item item = mongoItem.ToItem();

            // add parts if required
            if (includeParts)
            {
                var parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
                item.Parts.AddRange(InstantiateParts(
                    parts.AsQueryable().Where(p => p.ItemId.Equals(id))));
            }

            return item;
        }

        /// <summary>
        /// Adds or updates the specified item.
        /// </summary>
        /// <remarks>Note that the item's parts are ignored. If you want to add
        /// parts, use the <see cref="AddPart"/> method.</remarks>
        /// <param name="item">The item.</param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">null item or user ID</exception>
        public void AddItem(IItem item, bool history = true)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            EnsureClientCreated(_options.ConnectionString);

            // update sort key
            item.SortKey = _itemSortKeyBuilder.BuildKey(item, this);
            // set time modified
            item.TimeModified = DateTime.UtcNow;

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            if (history)
            {
                // add the new item to the history, as newly created or updated
                bool exists = items.AsQueryable().Any(i => i.Id.Equals(item.Id));

                MongoHistoryItem historyItem = new MongoHistoryItem(item)
                {
                    Status = exists ? EditStatus.Updated : EditStatus.Created
                };

                db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION)
                    .InsertOne(historyItem);
            }

            items.ReplaceOne(i => i.Id.Equals(item.Id),
                new MongoItem(item),
                new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <remarks>Note that deleting an item does triggers the deletion of
        /// all its parts.</remarks>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">null ID or user ID</exception>
        public void DeleteItem(string id, string userId, bool history = true)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);

            // find the item to delete
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);
            MongoItem mongoItem = items.Find(i => i.Id.Equals(id)).FirstOrDefault();
            if (mongoItem == null) return;

            // delete the item's parts (one at a time, as we need to store them 
            // into history, too)
            var partsCollection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            var parts = (from p in partsCollection.AsQueryable()
                where p.ItemId.Equals(id)
                select p).ToList();
            foreach (var p in parts) DeletePart(p.Id, userId, history);

            // store the item being deleted into history, as deleted now by 
            // the specified user
            if (history)
            {
                MongoHistoryItem historyItem = new MongoHistoryItem(mongoItem)
                {
                    // user deleted it now
                    UserId = userId,
                    Status = EditStatus.Deleted,
                    TimeModified = DateTime.UtcNow
                };

                db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION)
                    .InsertOne(historyItem);
            }

            // delete the item
            items.DeleteOne(i => i.Id == id);
        }

        /// <summary>
        /// Set the flags of the item(s) with the specified ID(s).
        /// Note that this operation never affects the item's history.
        /// </summary>
        /// <param name="ids">The item identifier(s).</param>
        /// <param name="flags">The flags value.</param>
        /// <exception cref="ArgumentNullException">null ID(s)</exception>
        public void SetItemFlags(IList<string> ids, int flags)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            items.UpdateMany(
                Builders<MongoItem>.Filter.In(i => i.Id, ids),
                Builders<MongoItem>.Update.Set(i => i.Flags, flags));
        }

        /// <summary>
        /// Set the group ID of the item(s) with the specified ID(s).
        /// Note that this operation never affects the item's history.
        /// </summary>
        /// <param name="ids">The items identifiers.</param>
        /// <param name="groupId">The group ID (can be null).</param>
        public void SetItemGroupId(IList<string> ids, string groupId)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            items.UpdateMany(
                Builders<MongoItem>.Filter.In(i => i.Id, ids),
                Builders<MongoItem>.Update.Set(i => i.GroupId, groupId));
        }

        private async Task<int> GetDistinctGroupIdsCountAsync(
            IMongoCollection<BsonDocument> items, AggregateOptions options)
        {
            #region Mongo query
            // use cadmus;
            // db.getCollection("items").aggregate(
            //     [
            //         { 
            //             "$project" : { 
            //                 "groupId" : true
            //             }
            //         }, 
            //         { 
            //             "$match" : { 
            //                 "$and" : [
            //                     { 
            //                         "groupId" : { 
            //                             "$exists" : true
            //                         }
            //                     }, 
            //                     { 
            //                         "groupId" : { 
            //                             "$ne" : null
            //                         }
            //                     }
            //                 ]
            //             }
            //         }, 
            //         { 
            //             "$group" : { 
            //                 "_id" : "groupId"
            //             }
            //         }, 
            //         { 
            //             "$count" : "count"
            //         }
            //     ], 
            //     { 
            //         "allowDiskUse" : false
            //     }
            // );
            #endregion

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
            {
                new BsonDocument("$project", new BsonDocument()
                        .Add("groupId", new BsonBoolean(true))),
                new BsonDocument("$match", new BsonDocument()
                        .Add("$and", new BsonArray()
                                .Add(new BsonDocument()
                                        .Add("groupId", new BsonDocument()
                                                .Add("$exists", new BsonBoolean(true))
                                        )
                                )
                                .Add(new BsonDocument()
                                        .Add("groupId", new BsonDocument()
                                                .Add("$ne", BsonNull.Value)
                                        )
                                )
                        )),
                new BsonDocument("$group", new BsonDocument()
                        .Add("_id", "$groupId")),
                new BsonDocument("$count", "count")
            };

            using (var cursor = await items.AggregateAsync(pipeline, options))
            {
                await cursor.MoveNextAsync();
                BsonDocument first = cursor.Current.FirstOrDefault();
                return first?["count"].AsInt32 ?? 0;
            }
        }

        /// <summary>
        /// Gets the requested page from a list of all the distinct, non-null
        /// group IDs found in the items.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">options</exception>
        public async Task<DataPage<string>> GetDistinctGroupIdsAsync(
            PagingOptions options)
        {
            #region Mongo query
            // use cadmus;
            // db.getCollection("items").aggregate(
            //     [
            //         { 
            //             "$project" : { 
            //                 "groupId" : true
            //             }
            //         }, 
            //         { 
            //             "$match" : { 
            //                 "$and" : [
            //                     { 
            //                         "groupId" : { 
            //                             "$exists" : true
            //                         }
            //                     }, 
            //                     { 
            //                         "groupId" : { 
            //                             "$ne" : null
            //                         }
            //                     }
            //                 ]
            //             }
            //         }, 
            //         { 
            //             "$group" : { 
            //                 "_id" : "groupId"
            //             }
            //         }, 
            //         { 
            //             "$sort" : { 
            //                 "_id" : 1.0
            //             }
            //         }, 
            //         { 
            //             "$skip" : 0.0
            //         }, 
            //         { 
            //             "$limit" : 20.0
            //         }
            //     ], 
            //     { 
            //         "allowDiskUse" : false
            //     }
            // );
            #endregion

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<BsonDocument>(MongoItem.COLLECTION);

            var aggOptions = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            int total = await GetDistinctGroupIdsCountAsync(items, aggOptions);
            List<string> ids = new List<string>();

            if (total > 0)
            {
                PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
                {
                new BsonDocument("$project", new BsonDocument()
                        .Add("groupId", new BsonBoolean(true))),
                new BsonDocument("$match", new BsonDocument()
                        .Add("$and", new BsonArray()
                                .Add(new BsonDocument()
                                        .Add("groupId", new BsonDocument()
                                                .Add("$exists", new BsonBoolean(true))
                                        )
                                )
                                .Add(new BsonDocument()
                                        .Add("groupId", new BsonDocument()
                                                .Add("$ne", BsonNull.Value)
                                        )
                                )
                        )),
                new BsonDocument("$group", new BsonDocument()
                        .Add("_id", "$groupId")),
                new BsonDocument("$sort", new BsonDocument()
                        .Add("_id", 1)),
                new BsonDocument("$skip", options.GetSkipCount()),
                new BsonDocument("$limit", options.PageSize)
                };

                using (var cursor = await items.AggregateAsync(pipeline, aggOptions))
                {
                    while (await cursor.MoveNextAsync())
                    {
                        foreach (BsonDocument document in cursor.Current)
                            ids.Add(document["_id"].AsString);
                    }
                }
            }

            return new DataPage<string>(options.PageNumber, options.PageSize,
                total, ids);
        }

        /// <summary>
        /// Get the count of all the non-empty layers in the specified items
        /// group.
        /// </summary>
        /// <param name="groupId">The group ID.</param>
        /// <returns>The count.</returns>
        public async Task<int> GetGroupLayersCountAsync(string groupId)
        {
            #region MongoDB query
            // db.getCollection("parts").aggregate(
            //     [
            //      { 
            //       "$match" : { 
            //        "content.fragments" : { 
            //         "$exists" : true, 
            //         "$ne" : [
            // 
            //         ]
            //        }
            //       }
            //      }, 
            //      { 
            //       "$lookup" : { 
            //        "from" : "items", 
            //        "localField" : "itemId", 
            //        "foreignField" : "_id", 
            //        "as" : "items"
            //       }
            //      }, 
            //      { 
            //       "$project" : { 
            //        "_id" : 1.0, 
            //        "item" : { 
            //         "$arrayElemAt" : [
            //             "$items", 
            //             0.0
            //         ]
            //        }
            //       }
            //      }, 
            //      { 
            //       "$match" : { 
            //        "item.groupId" : "g00"
            //       }
            //      }, 
            //      { 
            //       "$count" : "count"
            //      }
            //     ], 
            //     { 
            //      "allowDiskUse" : false
            //     }
            // );
            #endregion

            if (groupId is null)
                throw new ArgumentNullException(nameof(groupId));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var parts = db.GetCollection<BsonDocument>(MongoPart.COLLECTION);

            var aggOptions = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument()
                        .Add("content.fragments", new BsonDocument()
                                .Add("$exists", new BsonBoolean(true))
                                .Add("$ne", new BsonArray())
                        )),
                new BsonDocument("$lookup", new BsonDocument()
                        .Add("from", "items")
                        .Add("localField", "itemId")
                        .Add("foreignField", "_id")
                        .Add("as", "items")),
                new BsonDocument("$project", new BsonDocument()
                        .Add("_id", 1)
                        .Add("item", new BsonDocument()
                                .Add("$arrayElemAt", new BsonArray()
                                        .Add("$items")
                                        .Add(0)
                                )
                        )),
                new BsonDocument("$match", new BsonDocument()
                        .Add("item.groupId", groupId)),
                new BsonDocument("$count", "count")
            };

            using (var cursor = await parts.AggregateAsync(pipeline, aggOptions))
            {
                await cursor.MoveNextAsync();
                BsonDocument first = cursor.Current.FirstOrDefault();
                return first?["count"].AsInt32 ?? 0;
            }
        }

        /// <summary>
        /// Gets a page of history for the specified item.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history items page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public DataPage<HistoryItemInfo> GetHistoryItems(HistoryItemFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION);

            var builder = new FilterDefinitionBuilder<MongoHistoryItem>();
            FilterDefinition<MongoHistoryItem> f = builder.Empty;

            if (!string.IsNullOrEmpty(filter.Title))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.Title.Contains(filter.Title)));
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.Description.Contains(filter.Title)));
            }

            if (!string.IsNullOrEmpty(filter.FacetId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.Description.Contains(filter.FacetId)));
            }

            if (!string.IsNullOrEmpty(filter.GroupId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.GroupId.Equals(filter.GroupId)));
            }

            if (filter.Flags.HasValue)
            {
                f = builder.And(builder.BitsAllSet(i => i.Flags, filter.Flags.Value));
            }

            if (!string.IsNullOrEmpty(filter.ReferenceId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.ReferenceId.Equals(filter.ReferenceId)));
            }

            if (filter.Status.HasValue)
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.Status.Equals(filter.Status.Value)));
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.UserId.Equals(filter.UserId)));
            }

            if (filter.MinModified.HasValue)
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.TimeModified >= filter.MinModified.Value));
            }

            if (filter.MaxModified.HasValue)
            {
                f = builder.And(new ExpressionFilterDefinition<MongoHistoryItem>(
                    i => i.TimeModified <= filter.MaxModified.Value));
            }

            int total = (int)items.CountDocuments(f);
            if (total == 0)
            {
                return new DataPage<HistoryItemInfo>(
                    filter.PageNumber, filter.PageSize, 0,
                    null);
            }

            var mongoItems = items.Find(f)
                .SortByDescending(i => i.TimeModified)
                .ThenBy(i => i.ReferenceId)
                .ThenBy(i => i.Id)
                .Skip(filter.GetSkipCount())
                .Limit(filter.PageSize)
                .ToList();

            var results = mongoItems.Select(i => i.ToHistoryItemInfo()).ToList();
            return new DataPage<HistoryItemInfo>(
                filter.PageNumber, filter.PageSize, total, results);
        }

        /// <summary>
        /// Gets the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        /// <returns>item or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public HistoryItem GetHistoryItem(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION);

            MongoHistoryItem item = items.Find(
                i => i.Id.Equals(id)).FirstOrDefault();
            return item?.ToHistoryItem();
        }

        /// <summary>
        /// Adds the specified history item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public void AddHistoryItem(HistoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION);

            items.ReplaceOne(h => h.Id.Equals(item.Id),
                new MongoHistoryItem(item));
        }

        /// <summary>
        /// Deletes the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteHistoryItem(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var items = db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION);

            items.DeleteOne(i => i.Id.Equals(id));
        }
        #endregion

        #region Parts
        private IPart InstantiatePart(string typeId, string roleId, string content)
        {
            string reqTypeId = PartBase.BuildProviderId(typeId, roleId);
            Type type = _partTypeProvider.Get(reqTypeId);
            if (type == null) return null;

            return (IPart)JsonSerializer.Deserialize(content, type, _jsonOptions);
        }

        private IList<IPart> InstantiateParts(IEnumerable<MongoPart> parts,
            bool throwOnNull = true)
        {
            List<IPart> results = new List<IPart>();

            foreach (MongoPart mongoPart in parts)
            {
                IPart part = InstantiatePart(mongoPart.TypeId, mongoPart.RoleId,
                    mongoPart.Content.ToJson(_jsonSettings));

                if (part == null)
                {
                    string error = "Unable to instantiate part for type.role=" +
                        $"{mongoPart.TypeId}.{mongoPart.RoleId}";
                    Debug.WriteLine(error);
                    if (throwOnNull) throw new ApplicationException(error);
                }
                else
                {
                    results.Add(part);
                }
            }
            return results;
        }

        private static IMongoQueryable<MongoPart> ApplyPartFilters(
            IMongoQueryable<MongoPart> parts, string[] itemIds,
            string typeId, string roleId, string thesaurusScope)
        {
            if (itemIds?.Length > 0)
            {
                if (itemIds.Length == 1)
                {
                    parts = parts.Where(p => p.ItemId.Equals(itemIds[0]));
                }
                else
                {
                    parts = parts.Where(p => itemIds.Contains(p.ItemId));
                }
            }

            if (!string.IsNullOrEmpty(typeId))
                parts = parts.Where(p => p.TypeId.Equals(typeId));

            if (!string.IsNullOrEmpty(roleId))
                parts = parts.Where(p => p.RoleId.Equals(roleId));

            if (!string.IsNullOrEmpty(thesaurusScope))
                parts = parts.Where(p => p.ThesaurusScope.Equals(thesaurusScope));

            return parts;
        }

        /// <summary>
        /// Gets the specified page of parts.
        /// </summary>
        /// <param name="filter">The parts filter.</param>
        /// <returns>parts page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public DataPage<PartInfo> GetParts(PartFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var partCollection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            IMongoQueryable<MongoPart> parts = partCollection.AsQueryable();

            parts = ApplyPartFilters(parts, filter.ItemIds, filter.TypeId,
                filter.RoleId, filter.ThesaurusScope);

            if (!string.IsNullOrEmpty(filter.UserId))
                parts = parts.Where(p => p.UserId.Equals(filter.UserId));

            if (filter.MinModified.HasValue)
            {
                parts = parts.Where(p => p.TimeModified >= filter.MinModified.Value);
            }

            if (filter.MaxModified.HasValue)
            {
                parts = parts.Where(p => p.TimeModified <= filter.MaxModified.Value);
            }

            int total = parts.Count();
            if (total == 0)
            {
                return new DataPage<PartInfo>(
                    filter.PageNumber, filter.PageSize, 0, null);
            }

            List<MongoPart> results;

            // if no sort order specified, sort by TypeId,RoleId
            if (filter.SortExpressions == null)
            {
                parts = parts.OrderBy(p => p.TypeId)
                    .ThenBy(p => p.RoleId);
            }
            else
            {
                foreach (Tuple<string, bool> t in filter.SortExpressions)
                {
                    parts = (IMongoQueryable<MongoPart>)(t.Item2
                        ? parts.OrderBy(t.Item1)
                        : parts.OrderByDescending(t.Item1));
                }
            }
            results = parts.Skip(filter.GetSkipCount())
                .Take(filter.PageSize)
                .ToList();

            return new DataPage<PartInfo>(
                filter.PageNumber, filter.PageSize, total,
                results.Select(p => p.ToPartInfo()).ToList());
        }

        /// <summary>
        /// Gets the parts belonging to the specified item(s), eventually
        /// filtered by their type and/or role.
        /// </summary>
        /// <param name="itemIds">The item ID(s).</param>
        /// <param name="typeId">The optional type identifier.</param>
        /// <param name="roleId">The optional role identifier.</param>
        /// <returns>parts</returns>
        /// <exception cref="ArgumentNullException">null item ID(s)</exception>
        /// <exception cref="ArgumentOutOfRangeException">empty item IDs array
        /// </exception>
        public IList<IPart> GetItemParts(string[] itemIds, string typeId = null,
            string roleId = null)
        {
            if (itemIds == null)
                throw new ArgumentNullException(nameof(itemIds));
            if (itemIds.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(itemIds));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            IMongoQueryable<MongoPart> parts = collection.AsQueryable();

            parts = ApplyPartFilters(parts, itemIds, typeId, roleId, null);

            List<IPart> itemParts = new List<IPart>();
            itemParts.AddRange(InstantiateParts(
                parts.OrderBy(p => p.TypeId).ThenBy(p => p.RoleId)));

            return itemParts;
        }

        /// <summary>
        /// Gets layer parts information about the item with the specified ID.
        /// </summary>
        /// <param name="itemId">The item's identifier.</param>
        /// <param name="absent">if set to <c>true</c>, include also information
        /// about absent parts, i.e. those parts which are not present in the
        /// repository, but are defined in the item's facet.</param>
        /// <returns>layers parts information.</returns>
        /// <exception cref="ArgumentNullException">itemId</exception>
        public IList<LayerPartInfo> GetItemLayerInfo(string itemId, bool absent)
        {
            if (itemId == null)
                throw new ArgumentNullException(nameof(itemId));

            // get all the layer parts for the specified item
            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);

            List<MongoPart> parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION)
                .Find(Builders<MongoPart>.Filter
                    .And(
                        Builders<MongoPart>.Filter.Eq(p => p.ItemId, itemId),
                        Builders<MongoPart>.Filter.Where(
                            p => p.RoleId.StartsWith(PartBase.FR_PREFIX))))
                .SortBy(p => p.RoleId)
                .ToList();

            // generate the corresponding layer part infos
            List<LayerPartInfo> results = new List<LayerPartInfo>(
                from p in parts select p.ToLayerPartInfo());

            // append to these results the absent layer parts if requested
            if (absent)
            {
                // get the facet from the item's facet ID
                var items = db.GetCollection<MongoItem>(MongoItem.COLLECTION);
                MongoItem item = items.Find(
                    i => i.Id.Equals(itemId)).FirstOrDefault();

                if (item != null)
                {
                    MongoFacetDefinition facet =
                        db.GetCollection<MongoFacetDefinition>(
                            MongoFacetDefinition.COLLECTION)
                        .Find(f => f.Id.Equals(item.FacetId)).FirstOrDefault();
                    if (facet != null)
                    {
                        // append each layer part definition not already present
                        foreach (PartDefinition def in facet.PartDefinitions
                            // must be a layer part
                            .Where(pd => pd.RoleId?.StartsWith(
                                PartBase.FR_PREFIX, StringComparison.Ordinal) == true
                            // must not be already present in results
                            && results.Find(i => i.RoleId == pd.RoleId
                                            && i.TypeId == pd.TypeId) == null)
                            .OrderBy(pd => pd.RoleId))
                        {
                            results.Add(new LayerPartInfo
                            {
                                Id = null,
                                ItemId = itemId,
                                TypeId = def.TypeId,
                                RoleId = def.RoleId,
                                TimeCreated = DateTime.UtcNow,
                                CreatorId = null,
                                TimeModified = DateTime.UtcNow,
                                UserId = null,
                                IsAbsent = true
                            });
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the specified part.
        /// </summary>
        /// <typeparam name="T">The type of the part to cast the result to.
        /// </typeparam>
        /// <param name="id">The part identifier.</param>
        /// <returns>part or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public T GetPart<T>(string id) where T : class, IPart
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            MongoPart part = db.GetCollection<MongoPart>(MongoPart.COLLECTION)
                .Find(p => p.Id.Equals(id))
                .FirstOrDefault();

            if (part == null) return null;

            string providerId = PartBase.BuildProviderId(part.TypeId, part.RoleId);

            return (T)JsonSerializer.Deserialize(
                part.Content.ToJson(_jsonSettings),
                _partTypeProvider.Get(providerId),
                _jsonOptions);
        }

        /// <summary>
        /// Gets the identifier of the item including the part with the
        /// specified part identifier.
        /// </summary>
        /// <param name="id">The part identifier.</param>
        /// <returns>
        /// The item identifier, or null if part not found
        /// </returns>
        /// <exception cref="ArgumentNullException">id</exception>
        public string GetPartItemId(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            MongoPart part = db.GetCollection<MongoPart>(MongoPart.COLLECTION)
                .Find(p => p.Id.Equals(id))
                .FirstOrDefault();

            return part?.ItemId;
        }

        /// <summary>
        /// Gets the JSON code representing the part with the specified ID.
        /// </summary>
        /// <param name="id">The part identifier.</param>
        /// <returns>JSON code or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public string GetPartContent(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            MongoPart part = db.GetCollection<MongoPart>(MongoPart.COLLECTION)
                .Find(p => p.Id.Equals(id))
                .FirstOrDefault();

            return part?.Content?.ToJson(_jsonSettings);
        }

        /// <summary>
        /// Adds or updates the specified part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">null part or user ID
        /// </exception>
        public void AddPart(IPart part, bool history = true)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            string json = JsonSerializer.Serialize(part, part.GetType(),
                _jsonOptions);

            // set time modified
            part.TimeModified = DateTime.UtcNow;

            MongoPart mongoPart = new MongoPart(part)
            {
                Content = BsonDocument.Parse(json)
            };

            if (history)
            {
                bool exists = parts.AsQueryable().Any(p => p.Id.Equals(part.Id));
                MongoHistoryPart historyPart = new MongoHistoryPart(part)
                {
                    Content = mongoPart.Content,
                    Status = exists ? EditStatus.Updated : EditStatus.Created
                };

                db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                    .InsertOne(historyPart);
            }

            parts.ReplaceOne(p => p.Id.Equals(part.Id),
                mongoPart,
                new ReplaceOptions { IsUpsert = true });
        }

        /// <summary>
        /// Adds or updates the specified part from its encoded content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">content</exception>
        public void AddPartFromContent(string content, bool history = true)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            JsonDocument doc = JsonDocument.Parse(content);
            string typeId = doc.RootElement.GetProperty("typeId").GetString();

            string roleId = null;
            if (doc.RootElement.TryGetProperty("roleId", out JsonElement rid))
                roleId = rid.GetString();

            IPart part = InstantiatePart(typeId, roleId, content);
            AddPart(part, history);
        }

        /// <summary>
        /// Deletes the specified part.
        /// </summary>
        /// <param name="id">The part's identifier.</param>
        /// <param name="userId">The identifier of the user deleting the part.
        /// </param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">null part ID or user ID
        /// </exception>
        public void DeletePart(string id, string userId, bool history = true)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION);

            MongoPart part = parts.Find(p => p.Id.Equals(id)).FirstOrDefault();
            if (part == null) return;

            if (history)
            {
                MongoHistoryPart historyPart = new MongoHistoryPart(part)
                {
                    // user deleted it now
                    UserId = userId,
                    Status = EditStatus.Deleted,
                    TimeModified = DateTime.UtcNow
                };

                db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                    .InsertOne(historyPart);
            }

            parts.DeleteOne(p => p.Id.Equals(id));
        }

        private DataPage<HistoryPartInfo> GetHistoryParts(HistoryPartFilter filter,
            IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoHistoryPart>(
                MongoHistoryPart.COLLECTION);
            IQueryable<MongoHistoryPart> parts = collection.AsQueryable();

            if (filter.ItemIds?.Length > 0)
            {
                if (filter.ItemIds.Length == 1)
                {
                    parts = parts.Where(p => p.ItemId.Equals(filter.ItemIds[0]));
                }
                else
                {
                    parts = parts.Where(p => filter.ItemIds.Contains(p.ItemId));
                }
            }

            if (!string.IsNullOrEmpty(filter.TypeId))
                parts = parts.Where(p => p.TypeId.Equals(filter.TypeId));

            if (!string.IsNullOrEmpty(filter.RoleId))
                parts = parts.Where(p => p.RoleId.Equals(filter.RoleId));

            if (!string.IsNullOrEmpty(filter.ReferenceId))
            {
                parts = parts.Where(p => p.ReferenceId.Equals(filter.ReferenceId));
            }

            if (filter.Status.HasValue)
            {
                parts = parts.Where(p => p.Status.Equals(filter.Status.Value));
            }

            if (!string.IsNullOrEmpty(filter.UserId))
                parts = parts.Where(p => p.UserId.Equals(filter.UserId));

            if (filter.MinModified.HasValue)
            {
                parts = parts.Where(p => p.TimeModified >= filter.MinModified.Value);
            }

            if (filter.MaxModified.HasValue)
            {
                parts = parts.Where(p => p.TimeModified <= filter.MaxModified.Value);
            }

            int total = parts.Count();
            if (total == 0)
            {
                return new DataPage<HistoryPartInfo>(
                    filter.PageNumber, filter.PageSize, 0, null);
            }

            var page = parts.OrderByDescending(p => p.TimeModified)
                .ThenBy(p => p.Id)
                .Skip(filter.GetSkipCount())
                .Take(filter.PageSize)
                .ToList();

            return new DataPage<HistoryPartInfo>(
                filter.PageNumber, filter.PageSize, total,
                page.Select(p => p.ToHistoryPartInfo()).ToList());
        }

        /// <summary>
        /// Gets a page of history parts.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history items page</returns>
        public DataPage<HistoryPartInfo> GetHistoryParts(HistoryPartFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);

            return GetHistoryParts(filter, db);
        }

        /// <summary>
        /// Gets the specified history part.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>part or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public HistoryPart<T> GetHistoryPart<T>(string id)
            where T : class, IPart
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoHistoryPart>(
                MongoHistoryPart.COLLECTION);

            MongoHistoryPart part = collection.Find(p => p.Id.Equals(id))
                .FirstOrDefault();
            if (part == null) return null;

            T wrapped = JsonSerializer.Deserialize<T>(
                part.Content.ToJson(_jsonSettings),
                _jsonOptions);
            return new HistoryPart<T>(part.Id, wrapped)
            {
                UserId = part.UserId,
                Status = part.Status
            };
        }

        /// <summary>
        /// Deletes the specified history part.
        /// </summary>
        /// <param name="id">The history part's identifier.</param>
        /// <exception cref="ArgumentNullException">null part ID</exception>
        public void DeleteHistoryPart(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                .DeleteOne(p => p.Id.Equals(id));
        }

        /// <summary>
        /// Gets the ID of the creator of the part with the specified ID.
        /// </summary>
        /// <param name="id">The part ID.</param>
        /// <returns>Creator ID, or null.</returns>
        public string GetPartCreatorId(string id)
        {
            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var collection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            var filter = Builders<MongoPart>.Filter.Eq(p => p.Id, id);
            var fields = Builders<MongoPart>.Projection.Include(p => p.CreatorId);
            return collection
                .Find(filter)
                .Project<MongoPart>(fields)
                .FirstOrDefault()
                ?.CreatorId;
        }

        private Tuple<string, DateTime> GetPartItemIdAndLastModified(string id,
            IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            var filter = Builders<MongoPart>.Filter.Eq(p => p.Id, id);
            var fields = Builders<MongoPart>.Projection
                .Include(p => p.ItemId)
                .Include(p => p.TimeModified);
            MongoPart part = collection
                .Find(filter)
                .Project<MongoPart>(fields)
                .FirstOrDefault();
            return part != null
                ? Tuple.Create(part.ItemId, part.TimeModified)
                : null;
        }

        private Tuple<MongoHistoryPart, int> FindLastBaseTextChange(IMongoDatabase db,
            string baseTextPartId)
        {
            int total =
                db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                .AsQueryable()
                .Count(p => p.ReferenceId == baseTextPartId);

            for (int i = 0; i < total; i++)
            {
                var historyParts =
                    db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                    .AsQueryable()
                    .Skip(i)
                    .Where(p => p.ReferenceId == baseTextPartId)
                    .OrderByDescending(p => p.TimeModified)
                    .Take(2)
                    .ToList();

                if (historyParts.Count == 1)
                    return Tuple.Create(historyParts[0], total);

                MongoHistoryPart hp1 = historyParts[0];
                MongoHistoryPart hp2 = historyParts[1];
                IHasText part1 = InstantiatePart(hp1.TypeId, hp1.RoleId,
                    hp1.Content.ToJson(_jsonSettings))
                    as IHasText;
                IHasText part2 = InstantiatePart(hp2.TypeId, hp2.RoleId,
                    hp2.Content.ToJson(_jsonSettings))
                    as IHasText;
                if (part1 == null || part2 == null) return null;

                if (part1.GetText() != part2.GetText())
                    return Tuple.Create(hp1, total);
            }
            return null;
        }

        /// <summary>
        /// Determines whether the layer part with the specified ID might
        /// potentially have been broken because of changes in its base text.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="toleranceSeconds">The count of seconds representing
        /// the tolerance time interval between a base text save time and that
        /// of its layer part. Once this interval has elapsed, the layer part
        /// is not considered as potentially broken. If set to 0 or less, no
        /// tolerance interval is allowed.</param>
        /// <returns>
        /// <c>0</c> if the layer part is not potentially broken; <c>1</c>
        /// if it's potentially broken; <c>2</c> if it's surely broken.
        /// </returns>
        /// <remarks>A layer part is potentially broken when the corresponding
        /// text part has been saved (with a different text) either after it,
        /// or a few time before it.
        /// In both cases, this implies that the part fragments might have
        /// broken links, as the underlying text was in some way changed.
        /// To detect a potential break we can just check for last modified date
        /// and time; if the above conditions for save date and time are not met,
        /// the method can return false. If instead they are met, we must ensure
        /// that text has changed. To this end, we must go back in the text part
        /// history to find the latest save which changed the text, and refer
        /// to its date and time. If the history of the text part includes a
        /// single record, this means that the text part is in its original
        /// state; in this case, we never consider the layer part as broken,
        /// because this means that we just have imported or seeded both the
        /// text part and its layer(s) at the same time, or within a few
        /// fractions of seconds.
        /// </remarks>
        /// <exception cref="ArgumentNullException">id</exception>
        public int GetLayerPartBreakChance(string id, int toleranceSeconds)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);

            // get the layer part item ID and its last modified time
            // (if there is no such part, there's nothing which can be broken!)
            var layerPartItemIdAndTime = GetPartItemIdAndLastModified(id, db);
            if (layerPartItemIdAndTime == null) return 0;

            // get the layer part's container item
            // (if not found, it's broken for sure: the whole part is orphan!
            // anyway, this should never happen, as deleting items implying
            // deleting all their parts)
            MongoItem item = db.GetCollection<MongoItem>(MongoItem.COLLECTION)
                .Find(i => i.Id.Equals(layerPartItemIdAndTime.Item1))
                .FirstOrDefault();
            if (item == null) return 2;

            // get the item's facet
            MongoFacetDefinition facet = db.GetCollection<MongoFacetDefinition>
                (MongoFacetDefinition.COLLECTION)
                .Find(f => f.Id.Equals(item.FacetId))
                .FirstOrDefault();
            if (facet == null) return 1; // defensive

            // get the base text part from the role defined in the facet
            // (if not found, it's broken for sure, as any other layer in this item;
            // in this case, all the layers are orphaned)
            MongoPart baseTextPart = db.GetCollection<MongoPart>(
                MongoPart.COLLECTION)
                .Find(f => f.ItemId == item.Id && f.RoleId == PartBase.BASE_TEXT_ROLE_ID)
                .FirstOrDefault();
            if (baseTextPart == null) return 2;

            // determine the reference text part save time:
            // - if we have history, look for the latest saved base text
            // version where the text changed from its preceding version,
            // and use it as a reference.
            // - else, just use the current base text part as the reference.
            var lastHistoryPartAndTotal = FindLastBaseTextChange(db, baseTextPart.Id);
            DateTime lastTextChange = lastHistoryPartAndTotal != null
                ? lastHistoryPartAndTotal.Item1.TimeModified
                : baseTextPart.TimeModified;

            // the total count of text part versions in history, used to determine
            // if the current text part state is original (which happens when
            // the count is 1); consider it as 2 if there is no history at all,
            // because in such cases we cannot know if the current (unique)
            // state is original or not.
            int historyTotal = lastHistoryPartAndTotal != null
                ? lastHistoryPartAndTotal.Item2 : 2;

            // with/without tolerance: potentially broken if
            // last text change happened after/when layer was saved.
            if (lastTextChange >= layerPartItemIdAndTime.Item2
                && historyTotal > 1)
            {
                return 1;
            }

            // last text change happened before layer was saved:
            // potentially broken if no tolerance, or within the tolerance
            // interval.
            return toleranceSeconds > 0 && historyTotal > 1
                ? (layerPartItemIdAndTime.Item2 - lastTextChange).TotalSeconds
                   <= toleranceSeconds ? 1 : 0
                : 0;
        }

        /// <summary>
        /// Gets the layer part reconciliation hints.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The hints, one for each fragment in the layer part.
        /// If any of the required resources is not found, an empty list
        /// will be returned.</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public IList<LayerHint> GetLayerPartHints(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);

            List<LayerHint> hints = new List<LayerHint>();

            // get the layer part
            MongoPart mongoLayerPart = db.GetCollection<MongoPart>
                (MongoPart.COLLECTION)
                .Find(p => p.Id.Equals(id))
                .FirstOrDefault();
            if (mongoLayerPart == null) return hints;

            // get the layer part's container item
            MongoItem item = db.GetCollection<MongoItem>(MongoItem.COLLECTION)
                .Find(i => i.Id.Equals(mongoLayerPart.ItemId))
                .FirstOrDefault();
            if (item == null) return hints;

            // get the item's facet
            MongoFacetDefinition facet = db.GetCollection<MongoFacetDefinition>
                (MongoFacetDefinition.COLLECTION)
                .Find(f => f.Id.Equals(item.FacetId))
                .FirstOrDefault();
            if (facet == null) return hints;

            // get the base text part from the role defined in the facet
            MongoPart mongoTextPart = db.GetCollection<MongoPart>(
                MongoPart.COLLECTION)
                .Find(f => f.ItemId == item.Id && f.RoleId == PartBase.BASE_TEXT_ROLE_ID)
                .FirstOrDefault();
            if (mongoTextPart == null) return hints;

            // get the current text
            IPart textPart = InstantiatePart(mongoTextPart.TypeId,
                mongoTextPart.RoleId,
                mongoTextPart.Content.ToJson(_jsonSettings));
            string currentText = (textPart as IHasText)?.GetText();
            if (currentText == null) return hints;

            // locate the text which was current when the layer part
            // was last saved, i.e. its version in history whose save time
            // is less than the layer part save time
            MongoHistoryPart hp = db.GetCollection<MongoHistoryPart>(
                MongoHistoryPart.COLLECTION)
                .Find(p => p.ReferenceId.Equals(textPart.Id)
                      && p.TimeModified < mongoLayerPart.TimeModified)
                .FirstOrDefault();
            if (hp == null) return hints;

            // get the old text
            IPart oldTextPart = InstantiatePart(hp.TypeId, hp.RoleId,
                hp.Content.ToJson(_jsonSettings));
            string oldText = (oldTextPart as IHasText)?.GetText();
            if (oldText == null) return hints;

            // calculate diff's between old and new text
            List<Diff> diffs = new diff_match_patch()
                .diff_main(oldText, currentText);

            // adapt them into YX-based edit operations (the adapter is
            // created lazily here if required)
            if (_opDiffAdapter == null)
                _opDiffAdapter = new YXEditOperationDiffAdapter();
            IList<YXEditOperation> operations = _opDiffAdapter.Adapt(diffs);

            // to create hints, just load the layer part as a stripped down
            // part whose fragments just include their locations. This is all
            // what is needed for our purposes here, and is required because
            // we do not know at compile time the type of the fragment
            AnonLayerPart anonLayerPart = (AnonLayerPart)
                JsonSerializer.Deserialize(
                    mongoLayerPart.Content.ToJson(_jsonSettings),
                    typeof(AnonLayerPart),
                    _jsonOptions);

            return anonLayerPart.GetFragmentHints(operations);
        }

        /// <summary>
        /// Applies the specified patches instructions to the layer part
        /// with ID equal to <paramref name="id" />.
        /// </summary>
        /// <param name="id">The layer part identifier.</param>
        /// <param name="userId">The user identifier. This will be set as
        /// the author of the changes in the part.</param>
        /// <param name="patches">The patch instructions.</param>
        /// <returns>The patched layer part content.</returns>
        /// <exception cref="ArgumentNullException">id or userId or patches
        /// </exception>
        public string ApplyLayerPartPatches(string id, string userId,
            IList<string> patches)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (patches == null)
                throw new ArgumentNullException(nameof(patches));

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);

            // get the layer part
            var parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION);
            MongoPart mongoLayerPart = parts.Find(p => p.Id.Equals(id))
                .FirstOrDefault();
            if (mongoLayerPart == null) return null;

            // instantiate a generic layer part
            string json = mongoLayerPart.Content.ToJson(_jsonSettings);
            AnonLayerPart anonLayerPart = (AnonLayerPart)
                JsonSerializer.Deserialize(
                    json,
                    typeof(AnonLayerPart),
                    _jsonOptions);

            // apply patches to it
            mongoLayerPart.Content =
                BsonDocument.Parse(anonLayerPart.ApplyPatches(json, patches));
            anonLayerPart.UserId = userId;
            anonLayerPart.TimeModified = DateTime.UtcNow;

            // save it back
            parts.ReplaceOne(f => f.Id.Equals(mongoLayerPart.Id),
                mongoLayerPart,
                new ReplaceOptions { IsUpsert = true });

            // return patched part content
            return mongoLayerPart.Content.ToJson(_jsonSettings);
        }

        private static string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length == 1) return s.ToLowerInvariant();
            return $"{char.ToLowerInvariant(s[0])}{s.Substring(1)}";
        }

        /// <summary>
        /// Set the <see cref="IPart.ThesaurusScope"/> property of all the
        /// parts with the specified ID.
        /// Note that this operation never affects the item's history.
        /// </summary>
        /// <param name="ids">The item identifier(s).</param>
        /// <param name="scope">The new scope (may be null).</param>
        /// <exception cref="ArgumentNullException">null ID(s)</exception>
        public void SetPartThesaurusScope(IList<string> ids, string scope)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            EnsureClientCreated(_options.ConnectionString);

            IMongoDatabase db = Client.GetDatabase(_databaseName);
            var parts = db.GetCollection<MongoPart>(MongoPart.COLLECTION);

            foreach (MongoPart part in parts.AsQueryable()
                .Where(p => ids.Contains(p.Id)))
            {
                // update property
                part.ThesaurusScope = scope;

                // patch property in serialized content, too
                string propName = ToCamelCase(nameof(IPart.ThesaurusScope));
                if (part.Content.Contains(propName))
                    part.Content[propName] = scope;
                else
                    part.Content.Add(propName, scope);

                // save
                parts.ReplaceOne(p => p.Id.Equals(part.Id),
                    part,
                    new ReplaceOptions { IsUpsert = true });
            }
        }
        #endregion
    }

    /// <summary>
    /// Options for <see cref="MongoCadmusRepository"/>.
    /// </summary>
    public sealed class MongoCadmusRepositoryOptions
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string, like e.g.
        /// <c>mongodb://localhost:27017/cadmus</c>.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
