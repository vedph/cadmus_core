using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Fusi.Tools.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cadmus.Mongo.Test
{
    [CollectionDefinition(nameof(NonParallelBrowserResourceCollection),
    DisableParallelization = true)]
    public class NonParallelBrowserResourceCollection { }

    [Collection(nameof(NonParallelBrowserResourceCollection))]
    public sealed class MongoHierarchyItemBrowserTest : MongoConsumerBase
    {
        private const string CONNECTION_TEMPLATE = "mongodb://localhost:27017/{0}";
        private const string DB_NAME = "cadmus-browser-test";
        private const string CONNECTION = "mongodb://localhost:27017/" + DB_NAME;

        private void PrepareDatabase()
        {
            // create or clear
            MongoDatabaseManager manager = new MongoDatabaseManager();
            if (manager.DatabaseExists(CONNECTION))
                manager.ClearDatabase(CONNECTION);
            else
            {
                DataProfile profile = new DataProfile
                {
                    Facets = new FacetDefinition[]
                    {
                        new FacetDefinition
                        {
                            Id = "default",
                            ColorKey = "303030",
                            Description = "Default facet",
                            Label = "default",
                            PartDefinitions = new List<PartDefinition>
                            {
                                new PartDefinition
                                {
                                    ColorKey = "F08080",
                                    Description = "Hierarchy",
                                    GroupKey = "general",
                                    IsRequired = true,
                                    Name = "hierarchy",
                                    SortKey = "hierarchy",
                                    TypeId = new HierarchyPart().TypeId
                                }
                            }
                        }
                    }
                };
                manager.CreateDatabase(CONNECTION, profile);
            }
        }

        private static void SetHierarchyPart(int index, HierarchyPart part,
            string[] itemIds)
        {
            switch (index)
            {
                case 0:
                    part.Y = 1;
                    part.X = 1;
                    part.ChildrenIds.Add(itemIds[1]);
                    part.ChildrenIds.Add(itemIds[2]);
                    break;
                case 1:
                    part.Y = 2;
                    part.X = 1;
                    part.ParentId = itemIds[0];
                    break;
                case 2:
                    part.Y = 2;
                    part.X = 2;
                    part.ParentId = itemIds[0];
                    part.ChildrenIds.Add(itemIds[3]);
                    break;
                case 3:
                    part.Y = 3;
                    part.X = 1;
                    part.ParentId = itemIds[2];
                    break;
            }
        }

        private MongoItem[] SeedItems()
        {
            EnsureClientCreated(CONNECTION);
            IMongoDatabase db = Client.GetDatabase(DB_NAME);
            IMongoCollection<MongoItem> items =
                db.GetCollection<MongoItem>(MongoItem.COLLECTION);
            IMongoCollection<MongoPart> parts =
                db.GetCollection<MongoPart>(MongoPart.COLLECTION);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // generate IDs
            string[] itemIds = new string[4];
            for (int i = 0; i < 4; i++) itemIds[i] = Guid.NewGuid().ToString();

            // generate items and seed
            // 4 items: A B C D with hierarchy:
            // A (y=1)
            // +--B (y=2 x=1)
            // +--C (y=2 x=2)
            //    +--D (y=3)
            MongoItem[] mongoItems = new MongoItem[4];
            for (int i = 0; i < 4; i++)
            {
                char letter = (char)('A' + i);

                // item
                MongoItem mongoItem = new MongoItem
                {
                    Title = $"Item {letter}",
                    Description = $"Description for {letter}",
                    FacetId = "default",
                    SortKey = $"item {letter}",
                    CreatorId = "zeus",
                    UserId = "zeus"
                };
                mongoItems[i] = mongoItem;
                items.InsertOne(mongoItem);

                // hierarchy part
                HierarchyPart part = new HierarchyPart
                {
                    ItemId = mongoItem.Id,
                    CreatorId = mongoItem.CreatorId,
                    UserId = mongoItem.UserId
                };
                SetHierarchyPart(i, part, itemIds);
                parts.InsertOne(new MongoPart(part)
                {
                    Content = BsonDocument.Parse(
                        JsonSerializer.Serialize(part, part.GetType(), jsonOptions))
                });
            }

            return mongoItems;
        }

        [Fact]
        public async Task Browse_Hierarchy_Ok()
        {
            PrepareDatabase();
            MongoItem[] items = SeedItems();

            MongoHierarchyItemBrowser browser = new MongoHierarchyItemBrowser();
            browser.Configure(new ItemBrowserOptions
            {
                ConnectionString = CONNECTION_TEMPLATE,
            });

            // act
            DataPage<ItemInfo> page = await browser.BrowseAsync(
                DB_NAME,
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 0
                },
                new Dictionary<string, string>
                {
                    ["y"] = "2"
                });

            Assert.Equal(2, page.Total);
            Assert.Equal(2, page.Items.Count);
            // B
            Assert.Equal(items[1].Id, page.Items[0].Id);
            var payload = page.Items[0].Payload as MongoHierarchyItemBrowserPayload;
            Assert.NotNull(payload);
            Assert.Equal(2, payload.Y);
            Assert.Equal(1, payload.X);
            Assert.Equal(0, payload.ChildCount);
            // C
            Assert.Equal(items[2].Id, page.Items[1].Id);
            payload = page.Items[1].Payload as MongoHierarchyItemBrowserPayload;
            Assert.NotNull(payload);
            Assert.Equal(2, payload.Y);
            Assert.Equal(2, payload.X);
            Assert.Equal(1, payload.ChildCount);
        }
    }
}
