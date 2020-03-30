using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB hierarchy-based items browser.
    /// <para>Tag: <c>item-browser.mongo.hierarchy</c>.</para>
    /// </summary>
    /// <seealso cref="IItemBrowser" />
    [Tag("item-browser.mongo.hierarchy")]
    public sealed class MongoHierarchyItemBrowser : MongoConsumerBase,
        IItemBrowser,
        IConfigurable<MongoHierarchyItemBrowserOptions>
    {
        private MongoHierarchyItemBrowserOptions _options;
        private string _databaseName;
        private readonly string _partTypeId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoHierarchyItemBrowser"/>
        /// class.
        /// </summary>
        public MongoHierarchyItemBrowser()
        {
            _partTypeId = new HierarchyPart().TypeId;
        }

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(MongoHierarchyItemBrowserOptions options)
        {
            _options = options ??
                throw new ArgumentNullException(nameof(options));
            _databaseName = GetDatabaseName(options.ConnectionString);
        }

        private BsonDocument BuildMatchStage(MongoHierarchyItemBrowserFilter filter)
        {
            BsonDocument tagFilter = filter.Tag != null
                ? new BsonDocument().Add("content.tag", filter.Tag)
                : new BsonDocument().Add("content.tag", BsonNull.Value);

            // "$match" : {
            return new BsonDocument("$match", new BsonDocument()
                // "typeId" : "net.fusisoft.hierarchy",
                .Add("typeId", _partTypeId)
                // "$and" : [
                .Add("$and", new BsonArray()
                    // { "content.y": Y },
                    .Add(new BsonDocument().Add("content.y", filter.Y))
                    // { "$and": [
                    .Add(new BsonDocument().Add("$and", new BsonArray()
                        // { ... } ] } ] }
                        .Add(tagFilter)))));
        }

        private async Task<int> GetTotalAsync(IMongoCollection<BsonDocument> parts,
            MongoHierarchyItemBrowserFilter filter)
        {
            #region MongoDB query
            // db.getCollection("parts").aggregate(
            //   [
            //     { 
            //       "$match" : { 
            //         "typeId" : "net.fusisoft.hierarchy", 
            //         "$and" : [
            //           { 
            //             "content.y" : 3.0
            //           }, 
            //           { 
            //             "$and" : [
            //               { 
            //                 "content.tag" : null
            //               }
            //             ]
            //           }
            //         ]
            //       }
            //     }, 
            //     { 
            //       "$count" : "count"
            //     }
            //   ], 
            //   { 
            //     "allowDiskUse" : false
            //   }
            // );
            #endregion

            AggregateOptions aggOptions = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
            {
                BuildMatchStage(filter),
                new BsonDocument("$count", "count")
            };

            using (var cursor = await parts.AggregateAsync(pipeline, aggOptions))
            {
                await cursor.MoveNextAsync();
                BsonDocument first = cursor.Current.FirstOrDefault();
                return first?["count"].AsInt32 ?? 0;
            }
        }

        private static string GetBsonString(BsonValue value) =>
            value.BsonType != BsonType.Null ? value.AsString : null;

        private ItemInfo BsonDocumentToItemInfo(BsonDocument doc)
        {
            return new ItemInfo
            {
                Id = doc["_id"].AsString,
                Title = GetBsonString(doc["title"]),
                Description = GetBsonString(doc["description"]),
                FacetId = GetBsonString(doc["facetId"]),
                GroupId = GetBsonString(doc["groupId"]),
                SortKey = GetBsonString(doc["sortKey"]),
                Flags = doc["flags"].AsInt32,
                TimeCreated = doc["timeCreated"].ToUniversalTime(),
                CreatorId = GetBsonString(doc["creatorId"]),
                TimeModified = doc["timeModified"].ToUniversalTime(),
                UserId = GetBsonString(doc["userId"])
            };
        }

        /// <summary>
        /// Browses the items using the specified options.
        /// </summary>
        /// <param name="options">The paging and filtering options.
        /// You can set the page size to 0 when you want to retrieve all
        /// the items at the specified level.</param>
        /// <returns>
        /// Page of items.
        /// </returns>
        /// <exception cref="ArgumentNullException">options</exception>
        /// <exception cref="InvalidOperationException">not configured</exception>
        public async Task<DataPage<ItemInfo>> BrowseAsync(IPagingOptions options)
        {
            #region MongoDB query
            // db.getCollection("parts").aggregate(
            //   [
            //     { 
            //       "$match" : { 
            //         "typeId" : "net.fusisoft.hierarchy", 
            //         "$and" : [
            //           { 
            //             "content.y" : 3
            //           }, 
            //           { 
            //             "$and" : [
            //               { 
            //                 "content.tag" : null
            //               }
            //             ]
            //           }
            //         ]
            //       }
            //     }, 
            //     { 
            //       "$lookup" : { 
            //         "from" : "items", 
            //         "localField" : "itemId", 
            //         "foreignField" : "_id", 
            //         "as" : "items"
            //       }
            //     }, 
            //     { 
            //       "$sort" : { 
            //         "x" : 1, 
            //         "items[0].sortKey" : 1
            //       }
            //     }, 
            //     { 
            //       "$skip" : 0
            //     }, 
            //     { 
            //       "$limit" : 20
            //     }
            //   ], 
            //   { 
            //     "allowDiskUse" : false
            //   }
            // );
            #endregion

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (_options == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(MongoHierarchyItemBrowser)} not configured");
            }

            MongoHierarchyItemBrowserFilter filter =
                (MongoHierarchyItemBrowserFilter)options;

            EnsureClientCreated(_options.ConnectionString);
            IMongoDatabase db = Client.GetDatabase(_databaseName);
            IMongoCollection<BsonDocument> parts =
                db.GetCollection<BsonDocument>(MongoPart.COLLECTION);

            // the parts count is equal to the items count
            int total = await GetTotalAsync(parts, filter);
            if (total == 0)
            {
                return new DataPage<ItemInfo>(
                    options.PageNumber,
                    options.PageSize, 0, new List<ItemInfo>());
            }

            // get all the hierarchy parts with the specified Y level,
            // and eventually with the specified tag; get the corresponding
            // item; sort, page, and return
            AggregateOptions aggOptions = new AggregateOptions()
            {
                AllowDiskUse = false
            };

            List<BsonDocument> stages = new List<BsonDocument>(new BsonDocument[]
            {
                BuildMatchStage(filter),
                new BsonDocument("$lookup", new BsonDocument()
                        .Add("from", "items")
                        .Add("localField", "itemId")
                        .Add("foreignField", "_id")
                        .Add("as", "items")),
                new BsonDocument("$sort", new BsonDocument()
                        .Add("content.x", 1)
                        .Add("items[0].sortKey", 1)),
                new BsonDocument("$skip", filter.GetSkipCount())
            });

            if (options.PageSize > 0)
                stages.Add(new BsonDocument("$limit", filter.PageSize));

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = stages.ToArray();

            // collect all their items, in the paging range
            List<ItemInfo> items = new List<ItemInfo>();
            using (var cursor = await parts.AggregateAsync(pipeline, aggOptions))
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (BsonDocument document in cursor.Current)
                    {
                        ItemInfo info = BsonDocumentToItemInfo(document["items"]
                            .AsBsonArray[0]
                            .AsBsonDocument);

                        // add payload
                        BsonDocument content = document["content"].AsBsonDocument;
                        info.Payload = new MongoHierarchyItemBrowserPayload
                        {
                            Y = filter.Y,
                            X = content["x"].AsInt32,
                            ChildCount = content["childrenIds"].AsBsonArray.Count
                        };

                        items.Add(info);
                    }
                }
            }
            return new DataPage<ItemInfo>(options.PageNumber, options.PageSize,
                total, items);
        }
    }

    /// <summary>
    /// Filter for <see cref="MongoHierarchyItemBrowser"/>.
    /// </summary>
    /// <seealso cref="PagingOptions" />
    public sealed class MongoHierarchyItemBrowserFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets the Y-level filter to be matched on the
        /// <see cref="HierarchyPart"/>.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the optional tag filter to be matched on the
        /// <see cref="HierarchyPart"/>.
        /// </summary>
        public string Tag { get; set; }
    }

    /// <summary>
    /// Options for <see cref="MongoHierarchyItemBrowser"/>.
    /// </summary>
    public sealed class MongoHierarchyItemBrowserOptions
    {
        /// <summary>
        /// Gets or sets the connection string. This is supplied by the
        /// <see cref="ItemBrowserFactory"/>, unless overridden by this
        /// object's property.
        /// </summary>
        public string ConnectionString { get; set; }
    }

    /// <summary>
    /// Payload object added to <see cref="ItemInfo"/> by
    /// <see cref="MongoHierarchyItemBrowser"/>.
    /// </summary>
    public sealed class MongoHierarchyItemBrowserPayload
    {
        /// <summary>
        /// Gets or sets the Y-level.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the X-position in Y.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the children count.
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Y}.{X}: {ChildCount}";
        }
    }
}
