using Cadmus.Core;
using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Cadmus.Index.Sql;
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
            MySqlGraphRepository repository = new();
            repository.Configure(new SqlOptions
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
                Tag = Node.TAG_PROPERTY,
                Label = "Comment"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("a"),    // a=rdf:type
                Tag = Node.TAG_PROPERTY,
                Label = "Is-a"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:hasFacet"),
                Tag = Node.TAG_PROPERTY,
                Label = "Has Cadmus facet"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:isInGroup"),
                Tag = Node.TAG_PROPERTY,
                Label = "Is in Cadmus group"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("foaf:Person"),
                IsClass = true,
                Label = "Person"
            });

            // item
            NodeMapping itemMapping = new()
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
            NodeMapping itemDscMapping = new()
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
            NodeMapping itemPersonMapping = new()
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
            NodeMapping facetMapping = new()
            {
                SourceType = NodeSourceType.ItemFacet,
                Name = "Item facet",
                Prefix = "x:facets/",
                LabelTemplate = "{facet-id}",
                Description = "Item's facet -> node"
            };
            repository.AddMapping(facetMapping);

            // item has-facet facet
            NodeMapping facetLinkMapping = new()
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
            NodeMapping groupMapping = new()
            {
                SourceType = NodeSourceType.ItemGroup,
                Name = "Item group",
                Prefix = "x:groups/",
                LabelTemplate = "{group-id}",
                Description = "Item's group -> node"
            };
            repository.AddMapping(groupMapping);

            // item is-in-group group
            NodeMapping groupLinkMapping = new()
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
            NodeMapper mapper = new(repository);

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
            Assert.Equal(item.Id + "|facet", node.Sid);
            Assert.Equal("person", node.Label);
            Assert.True(node.IsClass);
            Assert.Null(node.Tag);

            // group node
            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:groups/writers");
            Assert.NotNull(node);
            Assert.Equal(NodeSourceType.ItemGroup, node.SourceType);
            Assert.Equal(item.Id + "|group", node.Sid);
            Assert.Equal("writers", node.Label);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);

            // class node (pre-existing, manual)
            node = set.Nodes.FirstOrDefault(n => n.Uri == "foaf:Person");
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
            Assert.Equal(item.Id + "|facet", triple.Sid);
            Assert.Null(triple.Tag);

            // triple barbato is-in-group writers
            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "kad:isInGroup");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Equal("x:groups/writers", triple.ObjectUri);
            Assert.Null(triple.ObjectLiteral);
            Assert.Equal(item.Id + "|group", triple.Sid);
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
        #endregion

        #region MapPins
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
            NodeMapping itemMapping = new()
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
            NodeMapping pinMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Pin full-name",
                PartType = "it.vedph.bricks.name",
                PinName = "full-name",
                TripleS = "$item",
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
            NodeMapper mapper = new(repository);

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

            // map 2 full-name pins
            GraphSet set = mapper.MapPins(item, part, new[]
            {
                Tuple.Create("full-name", "Scipione Barbato"),
                Tuple.Create("full-name", "Marco Barbato")
            });

            Assert.Equal(2, set.Triples.Count);

            TripleResult triple = set.Triples.FirstOrDefault(
                t => t.ObjectLiteral == "Scipione Barbato");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Null(triple.ObjectUri);
            Assert.Equal(part.Id + "|full-name", triple.Sid);
            Assert.Null(triple.Tag);

            triple = set.Triples.FirstOrDefault(
                t => t.ObjectLiteral == "Marco Barbato");
            Assert.NotNull(triple);
            Assert.Equal("x:persons/writers/scipione_barbato", triple.SubjectUri);
            Assert.Null(triple.ObjectUri);
            Assert.Equal(part.Id + "|full-name", triple.Sid);
            Assert.Null(triple.Tag);
        }

        private static void AddMultiEntityPartRules(IGraphRepository repository)
        {
            // properties/classes
            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:hasColor"),
                Tag = Node.TAG_PROPERTY,
                Label = "Has color"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:isInGroup"),
                Tag = Node.TAG_PROPERTY,
                Label = "Is in Cadmus group"
            });

            // manuscript item
            NodeMapping itemMapping = new()
            {
                SourceType = NodeSourceType.Item,
                Name = "Manuscript item",
                FacetFilter = "manuscript",
                Prefix = "x:manuscripts/",
                LabelTemplate = "{title}",
                Description = "Manuscript -> node"
            };
            repository.AddMapping(itemMapping);

            // eid pin
            NodeMapping eidMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Pin eid",
                FacetFilter = "manuscript",
                PartType = "it.vedph.ms-decorations",
                PinName = "eid",
                Prefix = "x:ms-decorations/",
                LabelTemplate = "{pin-value}",
                // slot key will be the EID (e.g. angel-1v); its content
                // will be the UID of the node output by this mapping
                // (e.g. x:ms/decorations/angel-1v)
                Slot = "{pin-value}"
            };
            repository.AddMapping(eidMapping);

            // eid in-group item
            NodeMapping eidGroupMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                ParentId = eidMapping.Id,
                Name = "Pin eid in group",
                PartType = "it.vedph.ms-decorations",
                PinName = "eid",
                TripleP = "kad:isInGroup",
                TripleO = "$item"
            };
            repository.AddMapping(eidGroupMapping);

            // eid has-color literal
            NodeMapping eidColorMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Pin eid color",
                PartType = "it.vedph.ms-decorations",
                PinName = "color@*",
                // the subject UID is got from a slot whose key is equal
                // to the pin's EID suffix (e.g. angel-1v from color@angel-1v)
                TripleS = "$slot:{pin-eid}",
                TripleP = "x:hasColor",
                TripleO = "$pin-value"
            };
            repository.AddMapping(eidColorMapping);
        }

        [Fact]
        public void MapPin_MultiEntityItemPin_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddMultiEntityPartRules(repository);
            NodeMapper mapper = new(repository);

            IItem item = new Item
            {
                Title = "Vat. Lat. 12",
                Description = "Vaticanus Latinus 12.",
                FacetId = "manuscript",
                SortKey = "vatlat12",
                CreatorId = "creator",
                UserId = "user"
            };
            IPart part = new MsDecorationsPart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                UserId = "user"
            };

            GraphSet set = mapper.MapPins(item, part, new[]
            {
                Tuple.Create("eid", "angel-1v"),
                Tuple.Create("eid", "demon-2r"),
                Tuple.Create("color@angel-1v", "gold"),
                Tuple.Create("color@demon-2r", "red"),
                Tuple.Create("color@demon-2r", "black")
            });

            Assert.Equal(3, set.Nodes.Count);
            Assert.Equal(5, set.Triples.Count);

            // the item node has been mapped only because involved in triples,
            // so it's just used as a reference
            NodeResult node = set.Nodes.FirstOrDefault(
                n => n.Uri == "x:manuscripts/vat_lat_12");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal(node.Uri, node.Label);
            Assert.Equal(NodeSourceType.User, node.SourceType);
            Assert.Null(node.Sid);

            node = set.Nodes.FirstOrDefault(
                n => n.Uri == "x:ms-decorations/angel-1v");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("angel-1v", node.Label);
            Assert.Equal(NodeSourceType.Pin, node.SourceType);
            Assert.Equal(part.Id + "|eid|angel-1v", node.Sid);

            node = set.Nodes.FirstOrDefault(
                n => n.Uri == "x:ms-decorations/demon-2r");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("demon-2r", node.Label);
            Assert.Equal(NodeSourceType.Pin, node.SourceType);
            Assert.Equal(part.Id + "|eid|demon-2r", node.Sid);

            TripleResult triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "kad:isInGroup" &&
                t.SubjectUri == "x:ms-decorations/angel-1v");
            Assert.NotNull(triple);
            Assert.Equal("x:manuscripts/vat_lat_12", triple.ObjectUri);
            Assert.Equal(part.Id + "|eid|angel-1v", triple.Sid);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);

            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "kad:isInGroup" &&
                t.SubjectUri == "x:ms-decorations/demon-2r");
            Assert.NotNull(triple);
            Assert.Equal("x:manuscripts/vat_lat_12", triple.ObjectUri);
            Assert.Equal(part.Id + "|eid|demon-2r", triple.Sid);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);

            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "x:hasColor" &&
                t.SubjectUri == "x:ms-decorations/angel-1v");
            Assert.NotNull(triple);
            Assert.Equal(part.Id + "|color@angel-1v", triple.Sid);
            Assert.Null(triple.ObjectUri);
            Assert.Equal("gold", triple.ObjectLiteral);
            Assert.Null(triple.Tag);

            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "x:hasColor" &&
                t.SubjectUri == "x:ms-decorations/demon-2r" &&
                t.ObjectLiteral == "red");
            Assert.NotNull(triple);
            Assert.Equal(part.Id + "|color@demon-2r", triple.Sid);
            Assert.Null(triple.ObjectUri);
            Assert.Null(triple.Tag);

            triple = set.Triples.FirstOrDefault(
                t => t.PredicateUri == "x:hasColor" &&
                t.SubjectUri == "x:ms-decorations/demon-2r" &&
                t.ObjectLiteral == "black");
            Assert.NotNull(triple);
            Assert.Equal(part.Id + "|color@demon-2r", triple.Sid);
            Assert.Null(triple.ObjectUri);
            Assert.Null(triple.Tag);
        }

        internal static void AddDeepEntityPartRules(IGraphRepository repository)
        {
            // properties/classes
            repository.AddNode(new Node
            {
                Id = repository.AddUri("a"),
                Tag = Node.TAG_PROPERTY,
                Label = "Is-a"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:hasDate"),
                Tag = Node.TAG_PROPERTY,
                Label = "Has date"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:hasSpouse"),
                Tag = Node.TAG_PROPERTY,
                Label = "Has spouse"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("kad:isInGroup"),
                Tag = Node.TAG_PROPERTY,
                Label = "Is in Cadmus group"
            });

            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:classes/birth"),
                IsClass = true,
                Label = "Birth class"
            });
            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:classes/wedding"),
                IsClass = true,
                Label = "Wedding class"
            });

            // mappings
            // person item
            NodeMapping itemMapping = new()
            {
                SourceType = NodeSourceType.Item,
                Name = "Person item",
                FacetFilter = "person",
                Prefix = "x:persons/",
                LabelTemplate = "{title}",
                Description = "Person -> node"
            };
            repository.AddMapping(itemMapping);

            // eid pin
            NodeMapping eidMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Event eid",
                PartType = "it.vedph.events",
                PinName = "eid",
                Prefix = "x:events/",
                LabelTemplate = "{pin-value}",
                Slot = "{pin-value}",
                Description = "Event pin eid -> node"
            };
            repository.AddMapping(eidMapping);

            // eid in-group item
            NodeMapping eidGroupMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                ParentId = eidMapping.Id,
                Name = "Pin eid in group",
                PartType = "it.vedph.events",
                PinName = "eid",
                TripleP = "kad:isInGroup",
                TripleO = "$item",
                Description = "Event in-group item"
            };
            repository.AddMapping(eidGroupMapping);

            // date pin
            NodeMapping dateMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Date pin",
                PartType = "it.vedph.events",
                PinName = "x:hasDate@*",
                TripleS = "$slot:{pin-eid}",
                TripleP = "$pin-name",
                TripleO = "$pin-value",
                Description = "Event has-date pin-value"
            };
            repository.AddMapping(dateMapping);

            // type pin
            NodeMapping typeMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Type pin",
                PartType = "it.vedph.events",
                PinName = "type@*",
                TripleS = "$slot:{pin-eid}",
                TripleP = "a",
                TripleO = "$pin-uid",
                Description = "Event a pin-value"
            };
            repository.AddMapping(typeMapping);

            // eid2@* pin
            NodeMapping eid2Mapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Pin eid2@*",
                PartType = "it.vedph.events",
                PinName = "eid2@*",
                LabelTemplate = "{pin-value} [#{pin-uid}]",
                Slot = "eid2/{pin-uid}"
            };
            repository.AddMapping(eid2Mapping);

            // rel@*@* pin
            NodeMapping relMapping = new()
            {
                SourceType = NodeSourceType.Pin,
                Name = "Pin rel@*@*",
                PartType = "it.vedph.events",
                PinName = "rel@*@*",
                TripleS = "$slot:{pin-eid:1}",
                TripleP = "$pin-uid",
                TripleO = "$slot:eid2/{pin-eid:2}"
            };
            repository.AddMapping(relMapping);
        }

        [Fact]
        public void MapPin_DeepEntityItemPin_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddDeepEntityPartRules(repository);
            NodeMapper mapper = new(repository);

            IItem item = new Item
            {
                Title = "Scipione Barbato",
                Description = "Scipione Barbato notaro.",
                FacetId = "person",
                SortKey = "scipionebarbato",
                CreatorId = "creator",
                UserId = "user"
            };
            IPart part = new EventsPart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                UserId = "user"
            };

            // The mock events part has a collection of events.
            // Each event has an EID, a type, a date, a place, and any number
            // of participants. Each participant has an EID and a type of
            // relation with that event. So, these events:
            // - Barbato was born in 1300.
            // - Barbato married Laura in 1340.
            // in Barbato's biographic events part are like:
            // - event with EID "birth":
            //   - type=x:classes/birth
            //   - date=1300
            // - event with EID "wedding":
            //   - type=x:classes/wedding
            //   - date=1340
            //   - participants:
            //     - EID="x:persons/laura"
            //     - relation=x:hasSpouse
            // The pins generated by this part are listed below.
            GraphSet set = mapper.MapPins(item, part, new[]
            {
                // events
                Tuple.Create("eid", "birth"),
                Tuple.Create("eid", "wedding"),
                // birth event (x@eid)
                Tuple.Create("type@birth", "x:classes/birth"),
                Tuple.Create("x:hasDate@birth", "1300"),
                // wedding event (x@eid, x@eid@eid2)
                Tuple.Create("eid2@wedding", "x:persons/laura"),
                Tuple.Create("type@wedding", "x:classes/wedding"),
                Tuple.Create("x:hasDate@wedding", "1340"),
                Tuple.Create("rel@wedding@x:persons/laura", "x:hasSpouse")
            });

            Assert.Equal(6, set.Nodes.Count);
            Assert.Equal(7, set.Triples.Count);

            NodeResult node = set.Nodes.FirstOrDefault(
                n => n.Uri == "x:persons/scipione_barbato");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            // this was added as a reference only
            Assert.Equal(node.Uri, node.Label);
            Assert.Equal(NodeSourceType.User, node.SourceType);
            Assert.Null(node.Sid);

            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:events/birth");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("birth", node.Label);
            Assert.Equal(NodeSourceType.Pin, node.SourceType);
            Assert.Equal(part.Id + "|eid|birth", node.Sid);

            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:events/wedding");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("wedding", node.Label);
            Assert.Equal(NodeSourceType.Pin, node.SourceType);
            Assert.Equal(part.Id + "|eid|wedding", node.Sid);

            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:classes/birth");
            Assert.NotNull(node);
            Assert.True(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("Birth class", node.Label);
            // this was matched among the existing nodes
            Assert.Equal(NodeSourceType.User, node.SourceType);
            Assert.Null(node.Sid);

            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:classes/wedding");
            Assert.NotNull(node);
            Assert.True(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal("Wedding class", node.Label);
            // this was matched among the existing nodes
            Assert.Equal(NodeSourceType.User, node.SourceType);
            Assert.Null(node.Sid);

            node = set.Nodes.FirstOrDefault(n => n.Uri == "x:persons/laura");
            Assert.NotNull(node);
            Assert.False(node.IsClass);
            Assert.Null(node.Tag);
            Assert.Equal(node.Uri, node.Label);
            Assert.Equal(NodeSourceType.Pin, node.SourceType);
            Assert.Equal(part.Id + "|eid2@wedding|x:persons/laura", node.Sid);

            // triples
            // birth is-in-group item
            TripleResult triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/birth" &&
                t.PredicateUri == "kad:isInGroup" &&
                t.ObjectUri == "x:persons/scipione_barbato");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|eid|birth", triple.Sid);

            // wedding is-in-group item
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/wedding" &&
                t.PredicateUri == "kad:isInGroup" &&
                t.ObjectUri == "x:persons/scipione_barbato");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|eid|wedding", triple.Sid);

            // birth a birth-class
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/birth" &&
                t.PredicateUri == "a" &&
                t.ObjectUri == "x:classes/birth");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|type@birth", triple.Sid);

            // wedding a wedding-class
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/wedding" &&
                t.PredicateUri == "a" &&
                t.ObjectUri == "x:classes/wedding");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|type@wedding", triple.Sid);

            // birth has-date 1300
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/birth" &&
                t.PredicateUri == "x:hasDate");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectUri);
            Assert.Equal("1300", triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|x:hasDate@birth", triple.Sid);

            // wedding has-date 1340
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/wedding" &&
                t.PredicateUri == "x:hasDate");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectUri);
            Assert.Equal("1340", triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|x:hasDate@wedding", triple.Sid);

            // wedding spouse laura
            triple = set.Triples.FirstOrDefault(t =>
                t.SubjectUri == "x:events/wedding" &&
                t.PredicateUri == "x:hasSpouse" &&
                t.ObjectUri == "x:persons/laura");
            Assert.NotNull(triple);
            Assert.Null(triple.ObjectLiteral);
            Assert.Null(triple.Tag);
            Assert.Equal(part.Id + "|rel@wedding@x:persons/laura", triple.Sid);
        }
        #endregion
    }

    #region Mock Parts
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

    [Tag("it.vedph.ms-decorations")]
    internal class MsDecorationsPart : PartBase
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

    [Tag("it.vedph.events")]
    internal class EventsPart: PartBase
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
    #endregion
}
