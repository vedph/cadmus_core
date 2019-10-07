using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Cadmus.Core.Blocks;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using TagSet = Cadmus.Core.Config.TagSet;

namespace Cadmus.Mongo
{
    /// <summary>
    /// MongoDB based repository for Cadmus data.
    /// Tag: <c>cadmus-repository.mongo</c>.
    /// </summary>
    /// <remarks>The Mongo database has the following collections:
    /// <list type="bullet">
    /// <item>
    /// <term>flags</term>
    /// <description>collection of <see cref="StoredFlagDefinition"/>'s.
    /// </description>
    /// </item>
    /// <item>
    /// <term>facets</term>
    /// <description>collection of <see cref="StoredItemFacet"/>'s.</description>
    /// </item>
    /// <item>
    /// <term>items</term>
    /// <description>collection of <see cref="StoredItem"/>'s.</description>
    /// </item>
    /// <item>
    /// <term>history-items</term>
    /// <description>collection of <see cref="StoredHistoryItem"/>'s.</description>
    /// </item>
    /// <item>
    /// <term>parts</term>
    /// <description>collection of <see cref="BsonDocument"/>'s representing
    /// objects implementing <see cref="IPart"/>, whose standard properties are
    /// <c>Id</c>, <c>ItemId</c>, <c>TypeId</c>, <c>RoleId</c>,
    /// <c>TimeModified</c>, <c>UserId</c>. Each part type then adds its own
    /// properties.</description>
    /// </item>
    /// <item>
    /// <term>history-parts</term>
    /// <description>collection of <see cref="HistoryWrapper{BsonDocument}"/>'s:
    /// its properties are <c>Id</c>, <c>TimeModified</c>, <c>UserId</c>,
    /// <c>Status</c>, <c>Content</c>, which represents the objects implementing
    /// <see cref="IPart"/>.
    /// </description>
    /// </item>
    /// </list>
    /// <para>As for parts, their models vary according to the part type, and
    /// part types may not be known before runtime, as they may come from plugins.
    /// When reading an item with its parts (<see cref="GetItem"/>), an instance
    /// of <see cref="IPartTypeProvider"/> is used to get the C# type for each
    /// part, and then this type information is used to instantiate the
    /// corresponding object during BSON deserialization. When adding an item
    /// (<see cref="AddItem"/>), no parts are added; each part is added
    /// separately using <see cref="AddPart"/>, which just stores the part
    /// object as a BSON document.</para>
    /// </remarks>
    [Tag("cadmus-repository.mongo")]
    public sealed class MongoCadmusRepository : ICadmusRepository,
        IConfigurable<MongoCadmusRepositoryOptions>
    {
        /// <summary>The name of the parts collection.</summary>
        public const string COLL_PARTS = "parts";

        /// <summary>The name of the history parts collection.</summary>
        public const string COLL_HISTORYPARTS = "history-parts";

        private readonly IPartTypeProvider _partTypeProvider;
        private MongoCadmusRepositoryOptions _options;
        private MongoClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoCadmusRepository"/>
        /// class.
        /// </summary>
        /// <param name="partTypeProvider">The part type provider.</param>
        /// <exception cref="ArgumentNullException">null options or part type
        /// provider</exception>
        public MongoCadmusRepository(IPartTypeProvider partTypeProvider)
        {
            _partTypeProvider = partTypeProvider ??
                throw new ArgumentNullException(nameof(partTypeProvider));

            // camel case everything:
            // https://stackoverflow.com/questions/19521626/mongodb-convention-packs/19521784#19521784
            ConventionPack pack = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("camel case", pack, _ => true);
        }

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(MongoCadmusRepositoryOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _client = new MongoClient(string.Format(CultureInfo.InvariantCulture,
                _options.ConnectionStringTemplate, _options.DatabaseName));
        }

        #region Flags
        /// <summary>
        /// Gets the flag definitions.
        /// </summary>
        /// <returns>definitions</returns>
        public IList<IFlagDefinition> GetFlagDefinitions()
        {
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredFlagDefinition> collection =
                db.GetCollection<StoredFlagDefinition>(
                    StoredFlagDefinition.COLLECTION);

            return (from d in collection.Find(_ => true).SortBy(f => f.Id).ToList()
                select (IFlagDefinition) new FlagDefinition
                {
                    Id = d.Id,
                    Label = d.Label,
                    Description = d.Description,
                    ColorKey = d.ColorKey
                }).ToList();
        }

        /// <summary>
        /// Gets the specified flag definition.
        /// </summary>
        /// <param name="id">The flag identifier.</param>
        /// <returns>definition or null if not found</returns>
        public IFlagDefinition GetFlagDefinition(int id)
        {
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredFlagDefinition> collection =
                db.GetCollection<StoredFlagDefinition>(
                    StoredFlagDefinition.COLLECTION);

            return collection.Find(f => f.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Adds or updates the specified flag definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <exception cref="ArgumentNullException">null definition</exception>
        public void AddFlagDefinition(IFlagDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredFlagDefinition> collection =
                db.GetCollection<StoredFlagDefinition>(
                    StoredFlagDefinition.COLLECTION);

            collection.ReplaceOne(f => f.Id.Equals(definition.Id),
                new StoredFlagDefinition(definition),
                new UpdateOptions {IsUpsert = true});
        }

        /// <summary>
        /// Deletes the specified flag definition.
        /// </summary>
        /// <param name="id">The flag identifier.</param>
        public void DeleteFlagDefinition(int id)
        {
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredFlagDefinition> collection =
                db.GetCollection<StoredFlagDefinition>(
                    StoredFlagDefinition.COLLECTION);

            collection.DeleteOne(f => f.Id.Equals(id));
        }
        #endregion

        #region Facets
        /// <summary>
        /// Gets the item's facets.
        /// </summary>
        /// <returns>facets</returns>
        public IList<IFacet> GetFacets()
        {
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItemFacet> collection =
                db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION);

            return (from f in collection.Find(_ => true)
                    .SortBy(f => f.Label).ToList()
                select (IFacet)new Facet
                {
                    Id = f.Id,
                    Label = f.Label,
                    Description = f.Description,
                    PartDefinitions = f.PartDefinitions
                }).ToList();
        }

        /// <summary>
        /// Gets the specified item's facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        /// <returns>facet or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public IFacet GetFacet(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItemFacet> collection =
                db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION);

            return collection.Find(f => f.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Adds or updates the specified facet.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <exception cref="ArgumentNullException">null facet</exception>
        public void AddFacet(IFacet facet)
        {
            if (facet == null) throw new ArgumentNullException(nameof(facet));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItemFacet> collection =
                db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION);

            collection.ReplaceOne(f => f.Id.Equals(facet.Id),
                new StoredItemFacet(facet),
                new UpdateOptions {IsUpsert = true});
        }

        /// <summary>
        /// Deletes the specified facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteFacet(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItemFacet> collection =
                db.GetCollection<StoredItemFacet>(StoredItemFacet.COLLECTION);

            collection.DeleteOne(f => f.Id.Equals(id));
        }
        #endregion

        #region Tags
        /// <summary>
        /// Gets the IDs of all the tags sets.
        /// </summary>
        /// <returns>IDs</returns>
        public IList<string> GetTagSetIds()
        {
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredTagSet> collection =
                db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION);

            return (from set in collection.AsQueryable()
                orderby set.Id
                select set.Id).ToList();
        }

        /// <summary>
        /// Gets the tag set with the specified ID.
        /// </summary>
        /// <param name="id">The tag set ID.</param>
        /// <returns>tag set, or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public TagSet GetTagSet(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredTagSet> collection =
                db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION);

            StoredTagSet stored = collection.AsQueryable()
                .FirstOrDefault(set => set.Id == id);
            return stored == null
                ? null
                : new TagSet
                {
                    Id = stored.Id,
                    Tags = stored.Tags.ToList()
                };
        }

        /// <summary>
        /// Adds or updates the specified tag set.
        /// </summary>
        /// <param name="set">The set.</param>
        /// <exception cref="ArgumentNullException">null set</exception>
        public void AddTagSet(TagSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredTagSet> collection =
                db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION);

            collection.ReplaceOne(old => old.Id.Equals(set.Id),
                new StoredTagSet(set),
                new UpdateOptions {IsUpsert = true});
        }

        /// <summary>
        /// Deletes the specified tag set.
        /// </summary>
        /// <param name="id">The tag set ID.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteTagSet(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredTagSet> collection =
                db.GetCollection<StoredTagSet>(StoredTagSet.COLLECTION);

            collection.DeleteOne(set => set.Id.Equals(id));
        }
        #endregion

        #region Items
        private static IItemInfo CreateItemInfo(StoredItem item)
        {
            return new ItemInfo
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Facet = item.FacetId,
                SortKey = item.SortKey,
                Flags = item.Flags,
                UserId = item.UserId,
                TimeModified = item.TimeModified
            };
        }

        /// <summary>
        /// Gets a page of items.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>items page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public PagedData<IItemInfo> GetItemsPage(ItemFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItem> collection =
                db.GetCollection<StoredItem>(StoredItem.COLLECTION);

            IQueryable<StoredItem> items = collection.AsQueryable();

            if (filter.Title != null)
                items = items.Where(i => i.Title.Contains(filter.Title));

            if (filter.Description != null)
                items = items.Where(i => i.Description.Contains(filter.Description));

            if (filter.FacetId != null)
                items = items.Where(i => i.FacetId.Equals(filter.FacetId));

            if (filter.Flags.HasValue)
                items = items.Where(i => i.Flags.Equals(filter.Flags.Value));

            if (filter.UserId != null)
                items = items.Where(i => i.UserId.Equals(filter.UserId));

            if (filter.MinModified.HasValue)
                items = items.Where(i => i.TimeModified >= filter.MinModified.Value);

            if (filter.MaxModified.HasValue)
                items = items.Where(i => i.TimeModified <= filter.MaxModified.Value);

            int total = items.Count();
            if (total == 0) return new PagedData<IItemInfo>(0, new List<IItemInfo>());

            items = items.OrderBy(i => i.SortKey);
            var page = items.Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            IList<IItemInfo> results = page.Select(CreateItemInfo).ToList();
            return new PagedData<IItemInfo>(total, results);
        }

        private static string GetFrTypeFromRoleId(string roleId)
        {
            int i = roleId.IndexOf('.');
            return i > -1 ? roleId.Substring(0, i) : roleId;
        }

        private static string BuildPartTypeProviderId(BsonDocument doc)
        {
            // the requested type ID should include the role ID when dealing with
            // text layer parts, whose role ID is assumed to be equal to their
            // fragment type ID, which always begin with "fr-", up to the 1st
            // dot if any.
            return doc["typeId"].AsString +
                   (!doc["roleId"].IsBsonNull
                     && Regex.IsMatch(doc["roleId"].AsString, "fr-.+")
                       ? $":{GetFrTypeFromRoleId(doc["roleId"].AsString)}"
                       : "");
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItem> collection =
                db.GetCollection<StoredItem>(StoredItem.COLLECTION);

            StoredItem stored = collection.Find(
                i => i.Id.Equals(id)).FirstOrDefault();
            if (stored == null) return null;

            Item item = new Item
            {
                Id = stored.Id,
                Title = stored.Title,
                Description = stored.Description,
                FacetId = stored.FacetId,
                SortKey = stored.SortKey,
                Flags = stored.Flags,
                UserId = stored.UserId,
                TimeModified = stored.TimeModified
            };

            // add parts if required
            if (includeParts)
            {
                IMongoCollection<BsonDocument> parts =
                    db.GetCollection<BsonDocument>(COLL_PARTS);
                foreach (BsonDocument doc in parts.AsQueryable()
                    .Where(p => p["itemId"].Equals(id)))
                {
                    // the requested type ID should include the role ID when
                    // dealing with text layer parts, whose role ID is assumed
                    // to be equal to their fragment type ID, which always begin
                    // with "fr-"
                    string reqTypeId = BuildPartTypeProviderId(doc);
                    Type t = _partTypeProvider.Get(reqTypeId);

                    if (t == null)
                    {
                        Debug.WriteLine("Unable to get part type from part ID " +
                            $"{doc["typeId"].AsString}");
                        continue;
                    }
                    IPart part = (IPart) BsonSerializer.Deserialize(doc, t);
                    item.Parts.Add(part);
                }
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItem> items =
                db.GetCollection<StoredItem>(StoredItem.COLLECTION);

            item.TimeModified = DateTime.UtcNow;

            if (history)
            {
                // add the new item to the history, as newly created or updated
                StoredItem old = items.Find(i => i.Id.Equals(item.Id))
                    .FirstOrDefault();
                StoredHistoryItem historyItem = new StoredHistoryItem(item)
                {
                    Status = old == null ? EditStatus.Created : EditStatus.Updated,
                };

                IMongoCollection<StoredHistoryItem> historyItems =
                    db.GetCollection<StoredHistoryItem>(
                        StoredHistoryItem.COLLECTION);
                historyItems.InsertOne(historyItem);
            }

            items.ReplaceOne(i => i.Id.Equals(item.Id),
                new StoredItem(item),
                new UpdateOptions {IsUpsert = true});
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);

            // find the item to delete
            IMongoCollection<StoredItem> items =
                db.GetCollection<StoredItem>(StoredItem.COLLECTION);
            StoredItem stored = items.Find(i => i.Id.Equals(id)).FirstOrDefault();
            if (stored == null) return;

            // delete the item's parts (one at a time as we need to store them 
            // into history, too)
            IMongoCollection<BsonDocument> partsCollection =
                db.GetCollection<BsonDocument>(COLL_PARTS);
            var parts = (from p in partsCollection.AsQueryable()
                where p["itemId"].Equals(id)
                select p).ToList();
            foreach (var p in parts) DeletePart(p["_id"].AsString, userId, history);

            // store the item being deleted into history, as deleted now by 
            // the specified user
            if (history)
            {
                StoredHistoryItem historyItem = new StoredHistoryItem(stored)
                {
                    TimeModified = DateTime.UtcNow,
                    UserId = userId,
                    Status = EditStatus.Deleted
                };

                IMongoCollection<StoredHistoryItem> historyItems =
                    db.GetCollection<StoredHistoryItem>(
                        StoredHistoryItem.COLLECTION);
                historyItems.InsertOne(historyItem);
            }

            // delete the item
            items.DeleteOne(i => i.Id == id);
        }

        /// <summary>
        /// Sets the item flags.
        /// </summary>
        /// <param name="id">The item identifier.</param>
        /// <param name="flags">The flags value.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void SetItemFlags(string id, int flags)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredItem> items =
                db.GetCollection<StoredItem>(StoredItem.COLLECTION);

            items.UpdateOne(i => i.Id.Equals(id),
                Builders<StoredItem>.Update.Set("flags", flags));
        }

        private static IHistoryItemInfo CreateHistoryItemInfo(
            StoredHistoryItem item)
        {
            return new HistoryItemInfo
            {
                Id = item.Id,
                ReferenceId = item.ReferenceId,
                Title = item.Title,
                Description = item.Description,
                Facet = item.FacetId,
                SortKey = item.SortKey,
                Flags = item.Flags,
                UserId = item.UserId,
                TimeModified = item.TimeModified,
                Status = item.Status
            };
        }

        /// <summary>
        /// Gets a page of history for the specified item.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history items page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public PagedData<IHistoryItemInfo> GetHistoryItemsPage(
            HistoryItemFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredHistoryItem> collection =
                db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);

            IQueryable<StoredHistoryItem> items = collection.AsQueryable();

            if (filter.ReferenceId != null)
                items = items.Where(i => i.ReferenceId.Equals(filter.ReferenceId));

            if (filter.UserId != null)
                items = items.Where(i => i.UserId.Equals(filter.UserId));

            if (filter.Status.HasValue)
                items = items.Where(i => i.Status.Equals(filter.Status.Value));

            if (filter.MinModified.HasValue)
            {
                items = items.Where(i =>
                    i.TimeModified.ToUniversalTime() >= filter.MinModified.Value);
            }

            if (filter.MaxModified.HasValue)
            {
                items = items.Where(i =>
                    i.TimeModified.ToUniversalTime() >= filter.MaxModified.Value);
            }

            int total = items.Count();
            if (total == 0)
                return new PagedData<IHistoryItemInfo>(0, new List<IHistoryItemInfo>());

            items = items.OrderBy(i => i.TimeModified);
            var page = items.Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            IList<IHistoryItemInfo> results = page.Select(CreateHistoryItemInfo).ToList();
            return new PagedData<IHistoryItemInfo>(total, results);
        }

        private static IHistoryItem CreateHistoryItem(StoredHistoryItem item)
        {
            return new HistoryItem
            {
                Id = item.Id,
                ReferenceId = item.ReferenceId,
                Title = item.Title,
                Description = item.Description,
                FacetId = item.FacetId,
                SortKey = item.SortKey,
                Flags = item.Flags,
                UserId = item.UserId,
                TimeModified = item.TimeModified,
                Status = item.Status
            };
        }

        /// <summary>
        /// Gets the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        /// <returns>item or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public IHistoryItem GetHistoryItem(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredHistoryItem> items =
                db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);

            StoredHistoryItem item = items.Find(
                i => i.Id.Equals(id)).FirstOrDefault();
            if (item == null) return null;

            return CreateHistoryItem(item);
        }

        /// <summary>
        /// Adds the specified history item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public void AddHistoryItem(IHistoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredHistoryItem> items =
                db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);

            items.ReplaceOne(h => h.Id.Equals(item.Id),
                new StoredHistoryItem(item));
        }

        /// <summary>
        /// Deletes the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public void DeleteHistoryItem(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<StoredHistoryItem> items =
                db.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);

            items.DeleteOne(i => i.Id.Equals(id));
        }
        #endregion

        #region Parts
        private static IPartInfo CreatePartInfo(BsonDocument doc)
        {
            return new PartInfo
            {
                Id = doc["_id"].AsString,
                ItemId = doc["itemId"].AsString,
                TypeId = doc["typeId"].AsString,
                RoleId = doc["roleId"].IsBsonNull ? null : doc["roleId"].AsString,
                UserId = doc["userId"].AsString,
                TimeModified = doc["timeModified"].ToUniversalTime()
            };
        }

        /// <summary>
        /// Gets the specified page of matching parts, or all the matching parts
        /// when the page size is 0.
        /// </summary>
        /// <param name="filter">The parts filter.</param>
        /// <returns>parts page</returns>
        /// <exception cref="ArgumentNullException">null filter</exception>
        public PagedData<IPartInfo> GetPartsPage(PartFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_PARTS);

            IQueryable<BsonDocument> parts = collection.AsQueryable();

            if (filter.ItemIds?.Length > 0)
            {
                if (filter.ItemIds.Length == 1)
                {
                    parts = parts.Where(d => d["itemId"].Equals(filter.ItemIds[0]));
                }
                else
                {
                    // http://stackoverflow.com/questions/39065424/mongodb-c-sharp-linq-contains-against-a-string-array-throws-argumentexception/39068513#39068513
                    List<BsonValue> aValues = filter.ItemIds
                        .Select(BsonValue.Create).ToList();
                    parts = parts.Where(d => aValues.Contains(d["itemId"]));
                }
            }

            if (filter.TypeId != null)
                parts = parts.Where(d => d["typeId"].Equals(filter.TypeId));

            if (filter.RoleId != null)
                parts = parts.Where(d => d["roleId"].Equals(filter.RoleId));

            if (filter.UserId != null)
                parts = parts.Where(d => d["userId"].Equals(filter.UserId));

            if (filter.MinModified.HasValue)
            {
                parts = parts.Where(d => d["timeModified"].ToUniversalTime()
                    >= filter.MinModified.Value);
            }

            if (filter.MaxModified.HasValue)
            {
                parts = parts.Where(d => d["timeModified"].ToUniversalTime()
                    <= filter.MaxModified.Value);
            }

            int total = parts.Count();
            if (total == 0)
                return new PagedData<IPartInfo>(0, new List<IPartInfo>());

            List<BsonDocument> results;
            if (filter.PageSize == 0)
            {
                // if no sort order specified, sort by TypeId,RoleId
                if (filter.SortExpressions == null)
                {
                    results = parts.OrderBy(d => d["typeId"])
                        .ThenBy(d => d["roleId"])
                        .ToList();
                }
                else
                {
                    // else apply the requested sort order
                    foreach (Tuple<string,bool> t in filter.SortExpressions)
                    {
                        parts = t.Item2
                            ? parts.OrderBy(d => d[t.Item1])
                            : parts.OrderByDescending(d => d[t.Item1]);
                    }
                    results = parts.ToList();
                }
            }
            else
            {
                // if no sort order specified, sort by TypeId,RoleId
                if (filter.SortExpressions == null)
                {
                    parts = parts.OrderBy(d => d["typeId"])
                        .ThenBy(d => d["roleId"]);
                }
                else
                {
                    foreach (Tuple<string, bool> t in filter.SortExpressions)
                    {
                        parts = t.Item2
                            ? parts.OrderBy(d => d[t.Item1])
                            : parts.OrderByDescending(d => d[t.Item1]);
                    }
                }
                results = parts.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();
            }

            return new PagedData<IPartInfo>(total,
                results.Select(CreatePartInfo).ToList());
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_PARTS);

            IQueryable<BsonDocument> parts = collection.AsQueryable();

            if (itemIds.Length == 1)
            {
                parts = parts.Where(d => d["itemId"].Equals(itemIds[0]));
            }
            else
            {
                // http://stackoverflow.com/questions/39065424/mongodb-c-sharp-linq-contains-against-a-string-array-throws-argumentexception/39068513#39068513
                List<BsonValue> values = itemIds.Select(BsonValue.Create).ToList();
                parts = parts.Where(d => values.Contains(d["itemId"]));
            }

            if (typeId != null)
                parts = parts.Where(d => d["typeId"].Equals(typeId));

            if (roleId != null)
                parts = parts.Where(d => d["roleId"].Equals(roleId));

            var results = parts.OrderBy(d => d["typeId"])
                .ThenBy(d => d["roleId"])
                .ToList();
            List<IPart> itemParts = new List<IPart>();
            foreach (BsonDocument doc in results)
            {
                string reqTypeId = BuildPartTypeProviderId(doc);
                Type t = _partTypeProvider.Get(reqTypeId);
                if (t == null)
                {
                    Debug.WriteLine("Unable to get part type from part ID " +
                        $"{doc["typeId"].AsString}");
                    continue;
                }
                itemParts.Add((IPart)BsonSerializer.Deserialize(doc, t));
            }

            return itemParts;
        }

        /// <summary>
        /// Gets the layer parts role IDs and part IDs for the specified item.
        /// This is useful when you want to have a list of all the item's layer
        /// parts IDs (part ID and role ID) so that you can retrieve each of
        /// them separately.
        /// </summary>
        /// <param name="id">The item's identifier.</param>
        /// <returns>array of tuples where 1=role ID and 2=part ID</returns>
        /// <exception cref="ArgumentNullException">null item ID</exception>
        public List<Tuple<string, string>> GetItemLayerPartIds(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);

            // we assume that all the layer parts have their role ID equal to their
            // fragments ID, which by convention starts with "fr-".
            var builder = new FilterDefinitionBuilder<BsonDocument>();
            var filter = builder.And(
                builder.Eq(d => d["itemId"], BsonValue.Create(id)),
                builder.Regex(d => d["roleId"],
                new BsonRegularExpression(new Regex("^fr-"))));

            var a = db.GetCollection<BsonDocument>(COLL_PARTS)
                .Find(filter)
                .SortBy(d => d["roleId"])
                .ToList();

            return (from d in a
                    select Tuple.Create(d["roleId"].AsString, d["_id"].AsString))
                .ToList();
        }

        /// <summary>
        /// Gets the specified part.
        /// </summary>
        /// <typeparam name="T">the type of the part to retrieve</typeparam>
        /// <param name="id">The part identifier.</param>
        /// <returns>part or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public T GetPart<T>(string id) where T : class, IPart
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_PARTS);
            BsonDocument doc = collection.Find(p => p["_id"].Equals(id))
                .FirstOrDefault();

            return doc == null ? null : BsonSerializer.Deserialize<T>(doc);
        }

        /// <summary>
        /// Gets the JSON code representing the part with the specified ID.
        /// </summary>
        /// <param name="id">The part identifier.</param>
        /// <returns>JSON code or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public string GetPartJson(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_PARTS);
            BsonDocument doc = collection.Find(p => p["_id"].Equals(id))
                .FirstOrDefault();

            return doc?.ToJson();
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> parts =
                db.GetCollection<BsonDocument>(COLL_PARTS);

            part.TimeModified = DateTime.UtcNow;

            if (history)
            {
                BsonDocument old = parts.Find(d => d["_id"].Equals(part.Id))
                    .FirstOrDefault();
                BsonDocument historyPart = new BsonDocument
                {
                    new BsonElement("_id", Guid.NewGuid().ToString("N")),
                    new BsonElement("timeModified", part.TimeModified),
                    new BsonElement("userId", part.UserId),
                    new BsonElement("status", old == null ?
                        EditStatus.Created : EditStatus.Updated),
                    new BsonElement("content", part.ToBsonDocument())
                };

                IMongoCollection<BsonDocument> historyParts =
                    db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);
                historyParts.InsertOne(historyPart);
            }

            parts.ReplaceOne(d => d["_id"].Equals(part.Id),
                part.ToBsonDocument(),
                new UpdateOptions {IsUpsert = true});
        }

        /// <summary>
        /// Adds or updates the part represented by the specified JSON code.
        /// </summary>
        /// <param name="json">The JSON code representing the part.</param>
        /// <param name="history">if set to <c>true</c>, the history should be
        /// affected.</param>
        /// <exception cref="ArgumentNullException">null JSON</exception>
        public void AddPartJson(string json, bool history = true)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            BsonDocument part = BsonDocument.Parse(json);
            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> parts =
                db.GetCollection<BsonDocument>(COLL_PARTS);

            if (history)
            {
                BsonDocument old = parts.Find(
                    d => d["_id"].Equals(part["_id"])).FirstOrDefault();
                BsonDocument historyPart = new BsonDocument
                {
                    new BsonElement("_id", Guid.NewGuid().ToString("N")),
                    new BsonElement("timeModified", part["timeModified"]),
                    new BsonElement("userId", part["userId"]),
                    new BsonElement("status", old == null ?
                    EditStatus.Created : EditStatus.Updated),
                    new BsonElement("content", part.ToBsonDocument())
                };

                IMongoCollection<BsonDocument> historyParts =
                    db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);
                historyParts.InsertOne(historyPart);
            }

            parts.ReplaceOne(d => d["_id"].Equals(part["_id"]),
                part.ToBsonDocument(),
                new UpdateOptions { IsUpsert = true });
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

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> parts =
                db.GetCollection<BsonDocument>(COLL_PARTS);

            BsonDocument part = parts.Find(
                d => d["_id"].Equals(id)).FirstOrDefault();
            if (part == null) return;

            if (history)
            {
                BsonDocument docHistory = new BsonDocument
                {
                    new BsonElement("_id", Guid.NewGuid().ToString("N")),
                    new BsonElement("userId", userId),
                    new BsonElement("timeModified", DateTime.UtcNow),
                    new BsonElement("status", EditStatus.Deleted),
                    new BsonElement("content", part)
                };
                IMongoCollection<BsonDocument> historyParts =
                    db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);
                historyParts.InsertOne(docHistory);
            }

            parts.DeleteOne(d => d["_id"].Equals(id));
        }

        /// <summary>
        /// Gets a page of history parts.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history items page</returns>
        public PagedData<IHistoryPartInfo> GetHistoryPartsPage(
            HistoryPartFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);

            IQueryable<BsonDocument> parts = collection.AsQueryable();

            if (filter.ItemIds?.Length > 0)
            {
                if (filter.ItemIds.Length == 1)
                {
                    parts = parts.Where(d => d["content"]["itemId"]
                        .Equals(filter.ItemIds[0]));
                }
                else
                {
                    List<BsonValue> aValues = filter.ItemIds
                        .Select(BsonValue.Create).ToList();
                    parts = parts
                        .Where(d => aValues.Contains(d["content"]["itemId"]));
                }
            }

            if (filter.ReferenceId != null)
            {
                parts = parts.Where(d => d["content"]["_id"]
                    .Equals(filter.ReferenceId));
            }

            if (filter.Status.HasValue)
            {
                parts = parts.Where(d => d["status"]
                    .Equals(filter.Status.Value));
            }

            if (filter.UserId != null)
                parts = parts.Where(d => d["userId"].Equals(filter.UserId));

            if (filter.MinModified.HasValue)
            {
                parts = parts.Where(d => d["timeModified"].ToUniversalTime()
                    >= filter.MinModified.Value);
            }

            if (filter.MaxModified.HasValue)
            {
                parts = parts.Where(d => d["timeModified"].ToUniversalTime()
                    <= filter.MaxModified.Value);
            }

            int total = parts.Count();
            if (total == 0)
            {
                return new PagedData<IHistoryPartInfo>(0,
                    new List<IHistoryPartInfo>());
            }

            var page = parts.OrderBy(d => d["timeModified"])
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            IList<IHistoryPartInfo> pageParts = (from p in page
                select (IHistoryPartInfo) new HistoryPartInfo
                {
                    Id = p["_id"].AsString,
                    UserId = p["userId"].AsString,
                    Status = (EditStatus) p["status"].AsInt32,
                    TimeModified = p["timeModified"].ToUniversalTime(),
                    ReferenceId = p["content"]["_id"].AsString,
                    TypeId = p["content"]["typeId"].AsString,
                    ItemId = p["content"]["itemId"].AsString,
                    RoleId = p["content"]["roleId"].AsString,
                }).ToList();
            return new PagedData<IHistoryPartInfo>(total, pageParts);
        }

        /// <summary>
        /// Gets the specified history part.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>part or null if not found</returns>
        /// <exception cref="ArgumentNullException">null ID</exception>
        public IHistoryPart<T> GetHistoryPart<T>(string id)
            where T : class, IPart
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);
            BsonDocument doc = collection.Find(p => p["_id"].Equals(id))
                .FirstOrDefault();

            if (doc == null) return null;

            T part = BsonSerializer.Deserialize<T>(doc["content"].AsBsonDocument);
            return new HistoryPart<T>(part)
            {
                Id = doc["_id"].AsString,
                ReferenceId = doc["content"]["_id"].AsString,
                Status = (EditStatus) doc["status"].AsInt32
            };
        }

        /// <summary>
        /// Adds the specified history part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException">null part</exception>
        public void AddHistoryPart<T>(IHistoryPart<T> part)
            where T : class,IPart
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);

            BsonDocument doc = new BsonDocument
            {
                new BsonElement("_id", BsonValue.Create(Guid.NewGuid().ToString("N"))),
                new BsonElement("status", part.Status),
                new BsonElement("timeModified", part.Content.TimeModified),
                new BsonElement("userId", part.Content.UserId),
                new BsonElement("content", part.Content.ToBsonDocument())
            };

            collection.InsertOne(doc);
        }

        /// <summary>
        /// Deletes the specified history part.
        /// </summary>
        /// <param name="id">The history part's identifier.</param>
        /// <exception cref="ArgumentNullException">null part ID</exception>
        public void DeleteHistoryPart(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            IMongoDatabase db = _client.GetDatabase(_options.DatabaseName);
            IMongoCollection<BsonDocument> collection =
                db.GetCollection<BsonDocument>(COLL_HISTORYPARTS);

            collection.DeleteOne(d => d["_id"].Equals(id));
        }
        #endregion

        #region Indexes
        /// <summary>
        /// Creates the required indexes in the specified database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <exception cref="ArgumentNullException">null database</exception>
        public static void CreateIndexes(IMongoDatabase database)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            var itemsCollection = database.GetCollection<StoredItem>(StoredItem.COLLECTION);
            itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(
                Builders<StoredItem>.IndexKeys.Ascending(i => i.Title)));
            itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(
                Builders<StoredItem>.IndexKeys.Ascending(i => i.SortKey)));
            itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(
                Builders<StoredItem>.IndexKeys.Ascending(i => i.FacetId)));
            itemsCollection.Indexes.CreateOne(new CreateIndexModel<StoredItem>(
                Builders<StoredItem>.IndexKeys.Ascending(i => i.Flags)));
            //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.Title));
            //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.SortKey));
            //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.FacetId));
            //itemsCollection.Indexes.CreateOne(Builders<StoredItem>.IndexKeys.Ascending(i => i.Flags));

            IMongoCollection<BsonDocument> partsCollection = database.GetCollection<BsonDocument>(COLL_PARTS);
            IndexKeysDefinitionBuilder<BsonDocument> bsonKdb = new IndexKeysDefinitionBuilder<BsonDocument>();
            partsCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Ascending("itemId")));
            partsCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(
                Builders<BsonDocument>.IndexKeys.Ascending("typeId")));
            //partsCollection.Indexes.CreateOne(Builders<BsonDocument>.IndexKeys.Ascending("itemId"));
            //partsCollection.Indexes.CreateOne(Builders<BsonDocument>.IndexKeys.Ascending("typeId"));

            var historyCollection = database.GetCollection<StoredHistoryItem>(StoredHistoryItem.COLLECTION);
            IndexKeysDefinitionBuilder<StoredHistoryItem> histKdb = new IndexKeysDefinitionBuilder<StoredHistoryItem>();
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.ReferenceId)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Status)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Title)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.SortKey)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.FacetId)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Flags)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.TimeModified)));
            historyCollection.Indexes.CreateOne(new CreateIndexModel<StoredHistoryItem>(
                Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.UserId)));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.ReferenceId));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Status));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Title));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.SortKey));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.FacetId));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.Flags));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.TimeModified));
            //historyCollection.Indexes.CreateOne(Builders<StoredHistoryItem>.IndexKeys.Ascending(i => i.UserId));
        }
        #endregion
    }

    /// <summary>
    /// Options for <see cref="MongoCadmusRepository"/>.
    /// </summary>
    public sealed class MongoCadmusRepositoryOptions
    {
        /// <summary>
        /// Gets or sets the MongoDB connection string template, where <c>{0}</c>
        /// is a placeholder for the database name.
        /// </summary>
        /// <value>The default value is <c>mongodb://localhost:27017/{0}</c>.
        /// </value>
        public string ConnectionStringTemplate { get; set; }

        /// <summary>
        /// Gets or sets the MongoDB database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="MongoCadmusRepositoryOptions"/> class.
        /// </summary>
        public MongoCadmusRepositoryOptions()
        {
            ConnectionStringTemplate = "mongodb://localhost:27017/{0}";
        }
    }
}
