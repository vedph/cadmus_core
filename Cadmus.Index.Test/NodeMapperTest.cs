using Cadmus.Core;
using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Id = repository.AddUri("a"),    // a=rdfs:type
                Tag = "property",
                Label = "Is-a"
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
            repository.AddNode(new Node
            {
                Id = repository.AddUri("foaf:Person"),
                IsClass = true,
                Label = "Person"
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

            // item comment dsc
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

            // item a person
            NodeMapping itemPersonMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                Name = "Item person link",
                FacetFilter = "person",
                ParentId = itemMapping.Id,
                TripleP = "a",
                TripleO = "foaf:Person",
                Description = "Person item a foaf:Person"
            };
            repository.AddMapping(itemPersonMapping);

            // item facet
            NodeMapping facetMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemFacet,
                Name = "Item facet",
                Prefix = "x:facets/",
                LabelTemplate = "{facet-id}",
                Description = "Item's facet -> node"
            };
            repository.AddMapping(facetMapping);

            // item has-facet facet
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

            // item group
            NodeMapping groupMapping = new NodeMapping
            {
                SourceType = NodeSourceType.ItemGroup,
                Name = "Item group",
                Prefix = "x:groups/",
                LabelTemplate = "{group-id}",
                Description = "Item's group -> node"
            };
            repository.AddMapping(groupMapping);

            // item is-in-group group
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
            // Scipione Barbato (item):
            // item => node
            //   -> item rdfs:comment dsc
            //   -> item kad:hasFacet facet
            //   -> item kad:isInGroup writers
            //   -> item a foaf:Person => triple + O-node for foaf:person
            // facet => node
            // group = node
            Reset();
            IGraphRepository repository = GetRepository();
            AddItemRules(repository);
            NodeMapper mapper = new NodeMapper(repository);

            IItem item = new Item
            {
                Title = "Scipione Barbato",
                Description = "A man of letters.",
                FacetId = "person",
                GroupId = "writers",
                SortKey = "scipionebarbato",
                CreatorId = "creator",
                UserId = "user"
            };
            GraphSet set = mapper.MapItem(item);

            Assert.Equal(4, set.Nodes.Count);
            Assert.Equal(4, set.Triples.Count);

            // item node
            NodeResult node = set.Nodes.FirstOrDefault(
                n => n.Uri == "x:persons/writers/scipione_barbato");
            Assert.NotNull(node);
            Assert.Equal(NodeSourceType.Item, node.SourceType);
            Assert.Equal(item.Id, node.Sid);
            Assert.Equal("Scipione Barbato", node.Label);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);

            // facet node
            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:facets/person");
            Assert.NotNull(node);
            Assert.Equal(NodeSourceType.ItemFacet, node.SourceType);
            Assert.Equal(item.Id + "/facet", node.Sid);
            Assert.Equal("person", node.Label);
            Assert.True(node.IsClass);
            Assert.Null(node.Tag);

            // group node
            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:groups/writers");
            Assert.NotNull(node);
            Assert.Equal(NodeSourceType.ItemGroup, node.SourceType);
            Assert.Equal(item.Id + "/group", node.Sid);
            Assert.Equal("writers", node.Label);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);

            // class node (pre-existing, manual)
            node = set.Nodes.FirstOrDefault(n => n.Uri == "foaf:person");
            Assert.NotNull(node);
            Assert.Equal(NodeSourceType.User, node.SourceType);
            Assert.Null(node.Sid);
            Assert.Equal("Person", node.Label);
            Assert.True(node.IsClass);
            Assert.Null(node.Tag);

            // triple barbato has-comment dsc
            TripleResult triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "rdfs:comment");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Equal("A man of letters.", triple.ObjectLiteral);
            Assert.Null(triple.ObjectUri);
            Assert.Equal(item.Id, triple.Sid);
            Assert.Null(triple.Tag);

            // triple barbato has-facet person
            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "kad:hasFacet");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Equal("x:facets/person", triple.ObjectUri);
            Assert.Null(triple.ObjectLiteral);
            Assert.Equal(item.Id + "/facet", triple.Sid);
            Assert.Null(triple.Tag);

            // triple barbato is-in-group writers
            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "kad:isInGroup");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Equal("x:groups/writers", triple.ObjectUri);
            Assert.Null(triple.ObjectLiteral);
            Assert.Equal(item.Id + "/group", triple.Sid);
            Assert.Null(triple.Tag);

            // triple barbato is foaf:person
            triple = set.Triples.FirstOrDefault(t => t.PredicateUri == "a");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Equal("foaf:Person", triple.ObjectUri);
            Assert.Null(triple.ObjectLiteral);
            Assert.Equal(item.Id, triple.Sid);
            Assert.Null(triple.Tag);
        }

        private static void AddSingleEntityPartRules(IGraphRepository repository)
        {
            // properties
            repository.AddNode(new Node
            {
                Id = repository.AddUri("foaf:name"),
                IsClass = true,
                Label = "Person's name"
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

            // pin full-name
            NodeMapping pinMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Pin,
                PartType = "it.vedph.bricks.name",
                PinName = "full-name",
                TripleP = "foaf:name",
                TripleO = "$pin-value"
            };
            repository.AddMapping(pinMapping);
        }

        [Fact]
        public void MapPin_SingleEntityItemPin_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddSingleEntityPartRules(repository);
            NodeMapper mapper = new NodeMapper(repository);

            IItem item = new Item
            {
                Title = "Scipione Barbato",
                Description = "A man of letters.",
                FacetId = "person",
                GroupId = "writers",
                SortKey = "scipionebarbato",
                CreatorId = "creator",
                UserId = "user"
            };
            IPart part = new NamePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                UserId = "user"
            };

            GraphSet set = mapper.MapPin(item, part, "full-name",
                "Scipione Barbato");

            // TODO
        }
        #endregion

        [Tag("it.vedph.bricks.name")]
        internal class NamePart : PartBase
        {
            public override IList<DataPinDefinition> GetDataPinDefinitions()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<DataPin> GetDataPins(IItem item = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
