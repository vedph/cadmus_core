using System;
using System.Text.Json;
using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using Cadmus.TestBase;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Xunit;

namespace Cadmus.Mongo.Test
{
    // https://github.com/xunit/xunit/issues/1999

    [CollectionDefinition(nameof(NonParallelResourceCollection), DisableParallelization = true)]
    public class NonParallelResourceCollection { }

    [Collection(nameof(NonParallelResourceCollection))]
    public class MongoCadmusRepositoryTest : CadmusRepositoryTestBase
    {
        private const string DB_NAME = "cadmus-test";

        private readonly MongoClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public MongoCadmusRepositoryTest()
        {
            _client = new MongoClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected override ICadmusRepository GetRepository()
        {
            TagAttributeToTypeMap map = new TagAttributeToTypeMap();
            map.Add(new[]
            {
                typeof(NotePart).Assembly
            });
            MongoCadmusRepository repository = new MongoCadmusRepository(
                new StandardPartTypeProvider(map),
                new StandardItemSortKeyBuilder());
            repository.Configure(new MongoCadmusRepositoryOptions
            {
                // use the default ConnectionStringTemplate (local DB)
                ConnectionString = "mongodb://localhost:27017/" + DB_NAME
            });
            return repository;
        }

        #region Seeding
        private static void SeedFlags(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoFlagDefinition>(
                MongoFlagDefinition.COLLECTION);
            collection.InsertMany(new[]
            {
                new MongoFlagDefinition
                {
                    Id = 1,
                    Label = "Alpha",
                    Description = "The alpha flag",
                    ColorKey = "FF0000"
                },
                new MongoFlagDefinition
                {
                    Id = 2,
                    Label = "Beta",
                    Description = "The beta flag",
                    ColorKey = "00FF00"
                }
            });
        }

        private static void SeedFacets(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoFacetDefinition>(
                MongoFacetDefinition.COLLECTION);

            MongoFacetDefinition facetA = new MongoFacetDefinition
            {
                Id = "alpha",
                Label = "Alpha",
                Description = "Alpha facet",
                ColorKey = "FF0000"
            };
            facetA.PartDefinitions.AddRange(new[]
            {
                new PartDefinition
                {
                    TypeId = "categories",
                    RoleId = "categories",
                    Name = "Categories",
                    Description = "Item's categories",
                    IsRequired = true,
                    ColorKey = "FF0000",
                    GroupKey = "General",
                    SortKey = "categories"
                },
                new PartDefinition
                {
                    TypeId = "note",
                    RoleId = "note",
                    Name = "Note",
                    Description = "Generic note",
                    IsRequired = false,
                    ColorKey = "00FF00",
                    GroupKey = "General",
                    SortKey = "note"
                }
            });

            MongoFacetDefinition facetB = new MongoFacetDefinition
            {
                Id = "beta",
                Label = "Beta",
                Description = "Beta facet",
                ColorKey = "00FF00"
            };
            facetB.PartDefinitions.AddRange(new[]
            {
                new PartDefinition
                {
                    TypeId = "categories",
                    RoleId = "categories",
                    Name = "Categories",
                    Description = "Item's categories",
                    IsRequired = true,
                    ColorKey = "FF0000",
                    GroupKey = "General",
                    SortKey = "categories"
                },
                new PartDefinition
                {
                    TypeId = "keywords",
                    RoleId = "keywords",
                    Name = "Keywords",
                    Description = "Generic keywords",
                    IsRequired = false,
                    ColorKey = "00FF00",
                    GroupKey = "General",
                    SortKey = "keywords"
                }
            });

            collection.InsertMany(new[]
            {
                facetA,
                facetB
            });
        }

        private static void SeedItems(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoItem>(MongoItem.COLLECTION);

            for (int i = 1; i <= 20; i++)
            {
                string userId = (i & 1) == 1 ? "Odd" : "Even";
                DateTime dt = new DateTime(2015, 12, i, 0, 0, 0, DateTimeKind.Utc);

                MongoItem item = new MongoItem
                {
                    Id = $"item-{i:000}",
                    Title = $"Item {i}",
                    Description = $"Description of item {i}",
                    FacetId = (i & 1) == 1 ? "alpha" : "beta",
                    SortKey = $"item{i:000}",
                    Flags = i,
                    CreatorId = userId,
                    UserId = userId,
                    TimeCreated = dt,
                    TimeModified = dt
                };
                collection.InsertOne(item);
            }
        }

        private MongoPart CreateMongoPart(IPart part)
        {
            return new MongoPart(part)
            {
                Content = JsonSerializer.Serialize(part, part.GetType(), _jsonOptions)
            };
        }

        private void SeedParts(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoPart>(MongoPart.COLLECTION);

            // categories
            CategoriesPart categoriesPart = new CategoriesPart
            {
                Id = "part-001",
                ItemId = "item-001",
                RoleId = "categories",
                CreatorId = "Odd",
                UserId = "Odd",
                Categories = {"alpha", "beta"}
            };
            collection.InsertOne(CreateMongoPart(categoriesPart));

            // note
            NotePart notePart = new NotePart
            {
                Id = "part-002",
                ItemId = "item-001",
                RoleId = "note",
                CreatorId = "Odd",
                UserId = "Odd",
                Text = "Some notes."
            };
            collection.InsertOne(CreateMongoPart(notePart));

            // layer: comments
            TokenTextLayerPart<CommentLayerFragment> commentLayerPart =
                new TokenTextLayerPart<CommentLayerFragment>
                {
                    Id = "part-003",
                    ItemId = "item-001",
                    CreatorId = "Odd",
                    UserId = "Odd"
            };
            commentLayerPart.Fragments.AddRange(new []
            {
                new CommentLayerFragment
                {
                    Location = "1.2",
                    Text = "The comment to 1.2"
                },
                new CommentLayerFragment
                {
                    Location = "1.3",
                    Text = "The comment to 1.3"
                }
            });
            collection.InsertOne(CreateMongoPart(commentLayerPart));
        }

        private static void ClearHistory(IMongoDatabase db)
        {
            db.GetCollection<MongoHistoryPart>(MongoHistoryPart.COLLECTION)
                .DeleteMany(new BsonDocument());

            db.GetCollection<MongoHistoryItem>(MongoHistoryItem.COLLECTION)
                .DeleteMany(new BsonDocument());
        }

        protected override void PrepareDatabase()
        {
            // camel case everything:
            // https://stackoverflow.com/questions/19521626/mongodb-convention-packs/19521784#19521784
            ConventionPack pack = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("camel case", pack, _ => true);

            _client.DropDatabase(DB_NAME);
            IMongoDatabase db = _client.GetDatabase(DB_NAME);

            SeedFlags(db);
            SeedFacets(db);
            SeedItems(db);
            SeedParts(db);
            ClearHistory(db);
        }
        #endregion

        #region Helper
        [Fact]
        public void GetDocumentsPage_NoFilterSortByKey_Ok()
        {
            IMongoDatabase db = _client.GetDatabase(DB_NAME);
            var collection = db.GetCollection<BsonDocument>(MongoItem.COLLECTION);

            var page = MongoHelper.GetDocumentsPage(collection, 
                "{}", "{\"sortKey\":1}", 1, 10);

            Assert.Equal(10, page.Items.Count);
            Assert.Equal(20, page.Total);
        }

        [Fact]
        public void GetDocumentsPage_UserIdSortByKey_Ok()
        {
            IMongoDatabase db = _client.GetDatabase(DB_NAME);
            var collection = db.GetCollection<BsonDocument>(MongoItem.COLLECTION);

            var page = MongoHelper.GetDocumentsPage(collection,
                "{\"userId\": \"Odd\"}", "{\"sortKey\":1}", 1, 10);

            Assert.Equal(10, page.Items.Count);
            Assert.Equal(10, page.Total);
        }
        #endregion

        #region Flags
        [Fact]
        public void GetFlagDefinitions_Ok()
        {
            DoGetFlagDefinitions_Ok();
        }

        [Fact]
        public void GetFlagDefinition_NotExisting_Null()
        {
            DoGetFlagDefinition_NotExisting_Null();
        }

        [Fact]
        public void GetFlagDefinition_Existing_Ok()
        {
            DoGetFlagDefinition_Existing_Ok();
        }

        [Fact]
        public void AddFlagDefinition_Existing_Updated()
        {
            DoAddFlagDefinition_Existing_Updated();
        }

        [Fact]
        public void AddFlagDefinition_NotExisting_Added()
        {
            DoAddFlagDefinition_NotExisting_Added();
        }

        [Fact]
        public void DeleteFlagDefinition_NotExisting_Nope()
        {
            DoDeleteFlagDefinition_NotExisting_Nope();
        }

        [Fact]
        public void DeleteFlagDefinition_Existing_Deleted()
        {
            DoDeleteFlagDefinition_Existing_Deleted();
        }
        #endregion

        #region Thesauri
        [Fact]
        public void GetThesaurusIds_Ok()
        {
            DoGetThesaurusIds_Ok();
        }

        [Fact]
        public void GetThesaurus_NotExisting_Null()
        {
            DoGetThesaurus_NotExisting_Null();
        }

        [Fact]
        public void GetThesaurus_Existing_Ok()
        {
            DoGetThesaurus_Existing_Ok();
        }

        [Fact]
        public void AddThesaurus_NotExisting_Added()
        {
            DoAddThesaurus_NotExisting_Added();
        }

        [Fact]
        public void AddThesaurus_Existing_Updated()
        {
            DoAddThesaurus_Existing_Updated();
        }

        [Fact]
        public void DeleteThesaurus_NotExisting_Nope()
        {
            DoDeleteThesaurus_NotExisting_Nope();
        }

        [Fact]
        public void DeleteThesaurus_Existing_Deleted()
        {
            DoDeleteThesaurus_Existing_Deleted();
        }
        #endregion

        #region Facets
        [Fact]
        public void GetFacets_Ok()
        {
            DoGetFacets_Ok();
        }

        [Fact]
        public void GetFacet_NotExisting_Null()
        {
            DoGetFacet_NotExisting_Null();
        }

        [Fact]
        public void GetFacet_Existing_Ok()
        {
            DoGetFacet_Existing_Ok();
        }

        [Fact]
        public void AddFacet_Existing_Updated()
        {
            DoAddFacet_Existing_Updated();
        }

        [Fact]
        public void AddFacet_NotExisting_Added()
        {
            DoAddFacet_NotExisting_Added();
        }

        [Fact]
        public void DeleteFacet_NotExisting_Nope()
        {
            DoDeleteFacet_NotExisting_Nope();
        }

        [Fact]
        public void DeleteFacet_Existing_Deleted()
        {
            DoDeleteFacet_Existing_Deleted();
        }
        #endregion

        #region Items
        [Fact]
        public void GetItemsPage_1_Ok()
        {
            DoGetItemsPage_1_Ok();
        }

        [Fact]
        public void GetItemsPage_2_Ok()
        {
            DoGetItemsPage_2_Ok();
        }

        [Fact]
        public void GetItemsPage_10_Empty()
        {
            DoGetItemsPage_10_Empty();
        }

        [Fact]
        public void GetItem_NotExisting_Null()
        {
            DoGetItem_NotExisting_Null();
        }

        [Fact]
        public void GetItem_Existing_Ok()
        {
            DoGetItem_Existing_Ok();
        }

        [Fact]
        public void GetItem_ExistingExcludeParts_Ok()
        {
            DoGetItem_ExistingExcludeParts_Ok();
        }

        [Fact]
        public void AddItem_Existing_Updated()
        {
            DoAddItem_Existing_Updated();
        }

        [Fact]
        public void AddItem_NotExisting_Added()
        {
            DoAddItem_NotExisting_Added();
        }

        [Fact]
        public void SetItemFlags_NotExisting_Nope()
        {
            DoSetItemFlags_NotExisting_Nope();
        }

        [Fact]
        public void SetItemFlags_Existing_Updated()
        {
            DoSetItemFlags_Existing_Updated();
        }

        [Fact]
        public void DeleteItem_NotExisting_Nope()
        {
            DoDeleteItem_NotExisting_Nope();
        }

        [Fact]
        public void DeleteItem_Existing_Deleted()
        {
            DoDeleteItem_ExistingNoParts_Deleted();
        }

        [Fact]
        public void DeleteItem_ExistingNoPartsNoHistory_Deleted()
        {
            DoDeleteItem_ExistingNoPartsNoHistory_Deleted();
        }

        [Fact]
        public void DeleteItem_ExistingWithParts_Deleted()
        {
            DoDeleteItem_ExistingWithParts_Deleted();
        }

        [Fact]
        public void GetItemLayers_NotFound_Empty()
        {
            DoGetItemLayers_NotFound_Empty();
        }

        [Fact]
        public void GetItemLayers_LayerParts_Ok()
        {
            DoGetItemLayers_LayerParts_Ok();
        }
        #endregion

        #region Parts
        [Fact]
        public void GetPartsPage_1Any_2()
        {
            DoGetPartsPage_1Any_2();
        }

        [Fact]
        public void GetPartsPage_1TypeId_1()
        {
            DoGetPartsPage_1TypeId_1();
        }

        [Fact]
        public void GetPartsPage_1RoleId_1()
        {
            DoGetPartsPage_1RoleId_1();
        }

        [Fact]
        public void GetPartsPage_TypeIdSortExpressions_Ok()
        {
            DoGetPartsPage_TypeIdSortExpressions_Ok();
        }

        [Fact]
        public void GetPartsPage_ItemIdTypeId_Ok()
        {
            DoGetPartsPage_ItemIdTypeId_Ok();
        }

        [Fact]
        public void GetItemParts_Item1_2()
        {
            DoGetItemParts_Item1_2();
        }

        [Fact]
        public void GetItemParts_Items1And2_2()
        {
            DoGetItemParts_Items1And2_2();
        }

        [Fact]
        public void GetPart_NotExisting_Null()
        {
            DoGetPart_NotExisting_Null();
        }

        [Fact]
        public void GetPartCreatorId_NotExisting_Null()
        {
            DoGetPartCreatorId_NotExisting_Null();
        }

        [Fact]
        public void GetPartCreatorId_Existing_Ok()
        {
            DoGetPartCreatorId_Existing_Ok();
        }

        [Fact]
        public void GetPartContent_NotExisting_Null()
        {
            DoGetPartContent_NotExisting_Null();
        }

        [Fact]
        public void GetPartContent_Existing_Ok()
        {
            DoGetPartContent_Existing_Ok();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddPart_NotExisting_Added(bool history)
        {
            DoAddPart_NotExisting_Added(history);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddPart_Existing_Updated(bool history)
        {
            DoAddPart_Existing_Updated(history);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddPartFromContent_NotExisting_Added(bool history)
        {
            DoAddPartFromContent_NotExisting_Added(history);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddPartFromContent_Existing_Updated(bool history)
        {
            DoAddPartFromContent_Existing_Updated(history);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeletePart_NotExisting_Nope(bool history)
        {
            DoDeletePart_NotExisting_Nope(history);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeletePart_Existing_Deleted(bool history)
        {
            DoDeletePart_Existing_Deleted(history);
        }
        // TODO
        #endregion
    }
}
