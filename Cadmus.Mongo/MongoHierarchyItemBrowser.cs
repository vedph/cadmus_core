﻿using Cadmus.Core;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB hierarchy-based items browser. This browser assumes that
    /// each item to be browsed has at least 1 <see cref="HierarchyPart"/>,
    /// representing its relationships in a single-parent hierarchy: this
    /// parts tells which is the parent item, and which are the children items,
    /// and the item's depth level (Y) and sibling ordinal number (X).
    /// It collects all such parts for the specified Y level, with paging,
    /// sorted by X and then by item's sort key.
    /// <para>Tag: <c>net.fusisoft.item-browser.mongo.hierarchy</c>.</para>
    /// </summary>
    /// <seealso cref="IItemBrowser" />
    [Tag("net.fusisoft.item-browser.mongo.hierarchy")]
    public sealed class MongoHierarchyItemBrowser : MongoConsumerBase,
        IItemBrowser,
        IConfigurable<ItemBrowserOptions>
    {
        private string _connection;
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
        public void Configure(ItemBrowserOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _connection = options.ConnectionString;
        }

        private BsonDocument BuildMatchStage(int y, string tag)
        {
            BsonDocument tagFilter = tag != null
                ? new BsonDocument().Add("content.tag", tag)
                : new BsonDocument().Add("content.tag", BsonNull.Value);

            // "$match" : {
            return new BsonDocument("$match", new BsonDocument()
                // "typeId" : "net.fusisoft.hierarchy",
                .Add("typeId", _partTypeId)
                // "$and" : [
                .Add("$and", new BsonArray()
                    // { "content.y": Y },
                    .Add(new BsonDocument().Add("content.y", y))
                    // { "$and": [
                    .Add(new BsonDocument().Add("$and", new BsonArray()
                        // { ... } ] } ] }
                        .Add(tagFilter)))));
        }

        private async Task<int> GetTotalAsync(IMongoCollection<BsonDocument> parts,
            int y, string tag)
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
                BuildMatchStage(y, tag),
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
        /// <param name="database">The database name.</param>
        /// <param name="options">The paging and filtering options.
        /// You can set the page size to 0 when you want to retrieve all
        /// the items at the specified level.</param>
        /// <param name="filters">The filters dictionary: it should include
        /// <c>y</c>=requested Y level and optionally <c>tag</c>=requested
        /// tag (defaults to null).</param>
        /// <returns>
        /// Page of items.
        /// </returns>
        /// <exception cref="ArgumentNullException">database or options or
        /// filters</exception>
        /// <exception cref="InvalidOperationException">not configured</exception>
        public async Task<DataPage<ItemInfo>> BrowseAsync(
            string database,
            IPagingOptions options,
            IDictionary<string, string> filters)
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

            if (database == null) throw new ArgumentNullException(nameof(database));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            if (_connection == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(MongoHierarchyItemBrowser)} not configured");
            }

            int y = 0;
            if (filters.ContainsKey("y"))
            {
                int.TryParse(filters["y"], out y);
            }
            string tag = filters.ContainsKey("tag") ? filters["tag"] : null;

            EnsureClientCreated(string.Format(CultureInfo.InvariantCulture,
                _connection, database));
            IMongoDatabase db = Client.GetDatabase(database);
            IMongoCollection<BsonDocument> parts =
                db.GetCollection<BsonDocument>(MongoPart.COLLECTION);

            // the parts count is equal to the items count
            int total = await GetTotalAsync(parts, y, tag);
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
                BuildMatchStage(y, tag),
                new BsonDocument("$lookup", new BsonDocument()
                        .Add("from", "items")
                        .Add("localField", "itemId")
                        .Add("foreignField", "_id")
                        .Add("as", "items")),
                new BsonDocument("$sort", new BsonDocument()
                        .Add("content.x", 1)
                        .Add("items[0].sortKey", 1)),
                new BsonDocument("$skip", (options.PageNumber - 1) * options.PageSize)
            });

            if (options.PageSize > 0)
                stages.Add(new BsonDocument("$limit", options.PageSize));

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
                            Y = y,
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
