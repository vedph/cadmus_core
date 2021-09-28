using Cadmus.Core;
using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Xunit;

namespace Cadmus.Index.Test
{
    // https://github.com/xunit/xunit/issues/1999
    [CollectionDefinition(nameof(NonParallelResourceCollection),
        DisableParallelization = true)]
    public class NonParallelResourceCollection { }
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class NodeMapperTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
        private const string DB_NAME = "cadmus-index-test";
        static private readonly string CS = string.Format(CST, DB_NAME);

        // private static IDbConnection GetConnection() => new MySqlConnection(CS);

        private static void Reset()
        {
            IDbManager manager = new MySqlDbManager(CST);
            if (manager.Exists(DB_NAME))
            {
                manager.ClearDatabase(DB_NAME);
            }
            else
            {
                manager.CreateDatabase(DB_NAME,
                    MySqlItemIndexWriter.GetMySqlSchema(), null);
            }
        }

        private static IGraphRepository GetRepository()
        {
            MySqlGraphRepository repository =
                new MySqlGraphRepository(new MySqlTokenHelper());
            repository.Configure(new Sql.SqlOptions
            {
                ConnectionString = CS
            });
            return repository;
        }

        #region ParseItemTitle
        [Fact]
        public void ParseItemTitle_Simple_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Null(tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithUid_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [#x:the_uid]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Equal("x:the_uid", tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithPrefix_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [@x:artists/]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Equal("x:artists/", tpu.Item2);
            Assert.Null(tpu.Item3);
        }
        #endregion

        #region ParsePinName
        [Fact]
        public void ParsePinName_Simple_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("name");

            Assert.Single(comps);
            Assert.Equal("name", comps[0]);
        }

        [Fact]
        public void ParsePinName_Composite1_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha");

            Assert.Equal(2, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
        }

        [Fact]
        public void ParsePinName_Composite2_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha@beta");

            Assert.Equal(3, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
            Assert.Equal("beta", comps[2]);
        }
        #endregion

        #region MapItem
        private static void AddItemRules(IGraphRepository repository)
        {
            // properties
            repository.AddNode(new Node
            {
                Id = repository.AddUri("rdfs:comment"),
                Tag = "property",
                Label = "Comment"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("rdfs:subClassOf"),
                Tag = "property",
                Label = "SubClassOf"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:hasFacet"),
                Tag = "property",
                Label = "Has Cadmus facet"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:isInGroup"),
                Tag = "property",
                Label = "Is in Cadmus group"
            });

            // item
            NodeMapping itemMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                Name = "Person item",
                FacetFilter = "person",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Description = "Person item -> node"
            };
            repository.AddMapping(itemMapping);

            NodeMapping itemDscMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                Name = "Item description",
                ParentId = itemMapping.Id,
                TripleP = "rdfs:comment",
                TripleO = "$dsc",
                Description = "Item's description -> rdfs:comment",
            };
            repository.AddMapping(itemDscMapping);

            NodeMapping itemPersonMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                Name = "Item person link",
                FacetFilter = "person",
                ParentId = itemMapping.Id,
                TripleP = "rdfs:subClassOf",
                TripleO = "foaf:Person",
                Description = "Person item subClassOf foaf:Person"
            };
            repository.AddMapping(itemPersonMapping);

            // facet
            NodeMapping facetMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemFacet,
                Name = "Item facet",
                Prefix = "x:facets/",
                LabelTemplate = "{facet-id}",
                Description = "Item's facet -> node"
            };
            repository.AddMapping(facetMapping);

            NodeMapping facetLinkMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemFacet,
                ParentId = facetMapping.Id,
                Name = "Item facet link",
                TripleP = "kad:hasFacet",
                TripleO = "$item",
                IsReversed = true,
                Description = "Item hasFacet facet"
            };
            repository.AddMapping(facetLinkMapping);

            // group
            NodeMapping groupMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemGroup,
                Name = "Item group",
                Prefix = "x:groups/",
                LabelTemplate = "{group-id}",
                Description = "Item's group -> node"
            };
            repository.AddMapping(groupMapping);

            NodeMapping groupLinkMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemGroup,
                ParentId = groupMapping.Id,
                Name = "Item group link",
                TripleP = "kad:isInGroup",
                TripleO = "$item",
                IsReversed = true,
                Description = "Item isInGroup group"
            };
            repository.AddMapping(groupLinkMapping);
        }

        [Fact]
        public void MapItem_ItemFacetGroupClass_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddItemRules(repository);
            NodeMapper mapper = new NodeMapper(repository);

            GraphSet set = mapper.MapItem(new Item
            {
                Title = "Scipione Barbato",
                Description = "A man of letters.",
                FacetId = "person",
                GroupId = "writers",
                SortKey = "scipionebarbato"
            });

            Assert.Equal(4, set.Nodes.Count);
            Assert.Equal(4, set.Triples.Count);
            // TODO
        }
        #endregion
    }
}
