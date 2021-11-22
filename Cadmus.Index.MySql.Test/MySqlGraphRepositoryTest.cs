using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Index.Graph;
using Cadmus.Index.Sql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cadmus.Index.MySql.Test
{
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class MySqlGraphRepositoryTest
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
            MySqlGraphRepository repository = new MySqlGraphRepository();
            repository.Configure(new SqlOptions
            {
                ConnectionString = CS
            });
            return repository;
        }

        #region Namespace
        private static void AddNamespaces(int count, IGraphRepository repository)
        {
            for (int n = 1; n <= count; n++)
                repository.AddNamespace("p" + n, $"http://www.ns{n}.org");
        }

        [Fact]
        public void GetNamespaces_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(3, repository);

            DataPage<NamespaceEntry> page = repository.GetNamespaces(
                new NamespaceFilter
                {
                    PageNumber = 1,
                    PageSize = 10,
                });

            Assert.Equal(3, page.Total);
            Assert.Equal(3, page.Items.Count);
        }

        [Fact]
        public void GetNamespaces_Prefix_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(3, repository);

            DataPage<NamespaceEntry> page = repository.GetNamespaces(
                new NamespaceFilter
                {
                    PageNumber = 1,
                    PageSize = 10,
                    Prefix = "p1"
                });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
        }

        [Fact]
        public void GetNamespaces_Uri_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(3, repository);

            DataPage<NamespaceEntry> page = repository.GetNamespaces(
                new NamespaceFilter
                {
                    PageNumber = 1,
                    PageSize = 10,
                    Uri = "ns2"
                });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
        }

        [Fact]
        public void LookupNamespace_NotExisting_Null()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);

            string uri = repository.LookupNamespace("not-existing");

            Assert.Null(uri);
        }

        [Fact]
        public void LookupNamespace_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);

            string uri = repository.LookupNamespace("p1");

            Assert.Equal("http://www.ns1.org", uri);
        }

        [Fact]
        public void DeleteNamespaceByPrefix_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);

            repository.DeleteNamespaceByPrefix("not-existing");

            Assert.Equal(2, repository.GetNamespaces(new NamespaceFilter()).Total);
        }

        [Fact]
        public void DeleteNamespaceByPrefix_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);

            repository.DeleteNamespaceByPrefix("p2");

            Assert.NotNull(repository.LookupNamespace("p1"));
            Assert.Null(repository.LookupNamespace("p2"));
        }

        [Fact]
        public void DeleteNamespaceByUri_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);

            repository.DeleteNamespaceByUri("not-existing");

            Assert.Equal(2, repository.GetNamespaces(new NamespaceFilter()).Total);
        }

        [Fact]
        public void DeleteNamespaceByUri_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(2, repository);
            repository.AddNamespace("p3", "http://www.ns1.org");

            repository.DeleteNamespaceByUri("http://www.ns1.org");

            Assert.Null(repository.LookupNamespace("p1"));
            Assert.Null(repository.LookupNamespace("p3"));
            Assert.NotNull(repository.LookupNamespace("p2"));
        }
        #endregion

        #region Uid
        [Fact]
        public void AddUid_NoClash_AddedNoSuffix()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            string uid = repository.AddUid("x:persons/john_doe",
                Guid.NewGuid().ToString());

            Assert.Equal("x:persons/john_doe", uid);
        }

        [Fact]
        public void AddUid_Clash_AddedWithSuffix()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            string sid = Guid.NewGuid().ToString();
            string uid1 = repository.AddUid("x:persons/john_doe", sid);

            string uid2 = repository.AddUid("x:persons/john_doe",
                Guid.NewGuid().ToString());

            Assert.NotEqual(uid1, uid2);
        }

        [Fact]
        public void AddUid_ClashButSameSid_ReusedWithSuffix()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            string sid = Guid.NewGuid().ToString();
            string uid1 = repository.AddUid("x:persons/john_doe", sid);

            string uid2 = repository.AddUid("x:persons/john_doe", sid);

            Assert.Equal(uid1, uid2);
        }
        #endregion

        #region Uri
        [Fact]
        public void AddUri_NotExisting_Added()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            const string uri = "http://www.sample.com";

            int id = repository.AddUri(uri);

            string uri2 = repository.LookupUri(id);
            Assert.Equal(uri, uri2);
        }

        [Fact]
        public void AddUri_Existing_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            const string uri = "http://www.sample.com";
            int id = repository.AddUri(uri);

            int id2 = repository.AddUri(uri);

            Assert.Equal(id, id2);
        }
        #endregion

        #region Node
        private static void AddNodes(int count, IGraphRepository repository,
            bool tag = false)
        {
            const string itemId = "e0cd9166-d005-404d-8f18-65be1f17b48f";
            const string partId = "f321f320-26da-4164-890b-e3974e9272ba";

            for (int i = 0; i < count; i++)
            {
                string uid = $"x:node{i + 1}";
                int id = repository.AddUri(uid);

                Node node = new Node
                {
                    Id = id,
                    Label = $"Node {i+1:00}",
                    SourceType = i == 0
                        ? NodeSourceType.Item : NodeSourceType.Pin,
                    Sid = i == 0 ? itemId : partId + "/p" + i,
                    Tag = tag? ((i + 1) % 2 == 0? "even" : "odd") : null
                };
                repository.AddNode(node);
            }
        }

        [Fact]
        public void GetNodes_NoFilter_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter());

            Assert.Equal(10 + 2, page.Total);
            Assert.Equal(10 + 2, page.Items.Count);
        }

        [Fact]
        public void GetNodes_NoFilterPage2_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                PageNumber = 2,
                PageSize = 5
            });

            Assert.Equal(10 + 2, page.Total);
            Assert.Equal(5, page.Items.Count);
            Assert.Equal("Node 05", page.Items[0].Label);
            Assert.Equal("Node 09", page.Items[4].Label);
        }

        [Fact]
        public void GetNodes_ByLabel_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                Label = "05"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("Node 05", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_BySourceType_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                SourceType = NodeSourceType.Item
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("Node 01", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_BySidExact_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                Sid = "e0cd9166-d005-404d-8f18-65be1f17b48f"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("Node 01", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_BySidPrefix_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(10, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                Sid = "f321f320-26da-4164-890b-e3974e9272ba/",
                IsSidPrefix = true
            });

            Assert.Equal(9, page.Total);
            Assert.Equal(9, page.Items.Count);
            Assert.Equal("Node 02", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_ByNoTag_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(3, repository);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                Tag = ""
            });

            Assert.Equal(3, page.Total);
            Assert.Equal(3, page.Items.Count);
        }

        [Fact]
        public void GetNodes_ByTag_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNodes(3, repository, true);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                Tag = "odd"
            });

            Assert.Equal(2, page.Total);
            Assert.Equal(2, page.Items.Count);
        }

        [Fact]
        public void GetNodes_ByLinkedNode_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // nodes
            Node argos = new Node
            {
                Id = repository.AddUri("x:dogs/argos"),
                Label = "Argos"
            };
            repository.AddNode(argos);
            Node a = new Node
            {
                Id = repository.AddUri("rdf:type"),
                Label = "a"
            };
            repository.AddNode(a);
            Node animal = new Node
            {
                Id = repository.AddUri("x:animal"),
                Label = "animal",
                IsClass = true
            };
            repository.AddNode(animal);
            // triple
            repository.AddTriple(new Triple
            {
                SubjectId = argos.Id,
                PredicateId = a.Id,
                ObjectId = animal.Id
            });

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                LinkedNodeId = animal.Id
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("Argos", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_ByLinkedNodeWithMatchingRole_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // nodes
            Node argos = new Node
            {
                Id = repository.AddUri("x:dogs/argos"),
                Label = "Argos"
            };
            repository.AddNode(argos);
            Node a = new Node
            {
                Id = repository.AddUri("rdf:type"),
                Label = "a"
            };
            repository.AddNode(a);
            Node animal = new Node
            {
                Id = repository.AddUri("x:animal"),
                Label = "animal",
                IsClass = true
            };
            repository.AddNode(animal);
            // triple
            repository.AddTriple(new Triple
            {
                SubjectId = argos.Id,
                PredicateId = a.Id,
                ObjectId = animal.Id
            });

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                LinkedNodeId = animal.Id,
                LinkedNodeRole = 'O'
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
            Assert.Equal("Argos", page.Items[0].Label);
        }

        [Fact]
        public void GetNodes_ByLinkedNodeWithNonMatchingRole_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // nodes
            Node argos = new Node
            {
                Id = repository.AddUri("x:dogs/argos"),
                Label = "Argos"
            };
            repository.AddNode(argos);
            Node a = new Node
            {
                Id = repository.AddUri("rdf:type"),
                Label = "a"
            };
            repository.AddNode(a);
            Node animal = new Node
            {
                Id = repository.AddUri("x:animal"),
                Label = "animal",
                IsClass = true
            };
            repository.AddNode(animal);
            // triple
            repository.AddTriple(new Triple
            {
                SubjectId = argos.Id,
                PredicateId = a.Id,
                ObjectId = animal.Id
            });

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                LinkedNodeId = animal.Id,
                LinkedNodeRole = 'S'
            });

            Assert.Equal(0, page.Total);
            Assert.Empty(page.Items);
        }

        [Fact]
        public void DeleteNode_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            Node argos = new Node
            {
                Id = repository.AddUri("x:dogs/argos"),
                Label = "Argos"
            };
            repository.AddNode(argos);

            repository.DeleteNode(argos.Id + 10);

            Assert.NotNull(repository.GetNode(argos.Id));
        }

        [Fact]
        public void DeleteNode_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            Node argos = new Node
            {
                Id = repository.AddUri("x:dogs/argos"),
                Label = "Argos"
            };
            repository.AddNode(argos);

            repository.DeleteNode(argos.Id);

            Assert.Null(repository.GetNode(argos.Id));
        }
        #endregion

        #region Property
        private static void AddProperties(IGraphRepository repository)
        {
            Node comment = new Node
            {
                Id = repository.AddUri("rdfs:comment"),
                Label = "comment"
            };
            repository.AddNode(comment);
            Node date = new Node
            {
                Id = repository.AddUri("x:date"),
                Label = "Date"
            };
            repository.AddNode(comment);

            repository.AddProperty(new Property
            {
                Id = comment.Id,
                DataType = "xsd:string",
                Description = "A comment.",
                LiteralEditor = "qed.md"
            });
            repository.AddProperty(new Property
            {
                Id = date.Id,
                DataType = "xs:date",
                Description = "A year-based date.",
                LiteralEditor = "qed.date"
            });
        }

        [Fact]
        public void GetProperties_NoFilter_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            var page = repository.GetProperties(new PropertyFilter());

            Assert.Equal(2, page.Total);
            Assert.Equal(2, page.Items.Count);
        }

        [Fact]
        public void GetProperties_ByUid_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            var page = repository.GetProperties(new PropertyFilter
            {
                Uid = "rdfs"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
        }

        [Fact]
        public void GetProperties_ByDataType_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            var page = repository.GetProperties(new PropertyFilter
            {
                DataType = "xsd:string"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
        }

        [Fact]
        public void GetProperties_ByLiteralEditor_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            var page = repository.GetProperties(new PropertyFilter
            {
                LiteralEditor = "qed.date"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
        }

        [Fact]
        public void GetProperty_NotExisting_Null()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            Assert.Null(repository.GetProperty(123));
        }

        [Fact]
        public void GetProperty_Existing_Null()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            Assert.NotNull(repository.GetProperty(1));
        }

        [Fact]
        public void GetPropertyByUri_NotExisting_Null()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            Assert.Null(repository.GetPropertyByUri("not-existing"));
        }

        [Fact]
        public void GetPropertyByUri_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            Assert.NotNull(repository.GetPropertyByUri("rdfs:comment"));
        }

        [Fact]
        public void DeleteProperty_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            repository.DeleteProperty(123);

            Assert.Equal(2, repository.GetProperties(new PropertyFilter()).Total);
        }

        [Fact]
        public void DeleteProperty_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddProperties(repository);

            repository.DeleteProperty(1);

            Assert.Equal(1, repository.GetProperties(new PropertyFilter()).Total);
        }
        #endregion

        #region Node Mapping
        private static void AddMappings(IGraphRepository repository)
        {
            // rdfs:comment
            repository.AddNode(new Node
            {
                Id = repository.AddUri("rdfs:comment"),
                Label = "comment"
            });
            // foaf:name
            repository.AddNode(new Node
            {
                Id = repository.AddUri("foaf:name"),
                Label = "Person name"
            });

            // item mapping
            NodeMapping itemMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                FacetFilter = "person",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Name = "Item",
                Description = "Map a person item into a node"
            };
            repository.AddMapping(itemMapping);

            // dsc child of item mapping
            NodeMapping itemDsc = new NodeMapping
            {
                ParentId = itemMapping.Id,
                SourceType = NodeSourceType.Item,
                Name = "Item description",
                TripleP = "rdfs:comment",
                TripleO = "$dsc"
            };
            repository.AddMapping(itemDsc);

            // part's pin mapping
            NodeMapping nameMapping = new NodeMapping
            {
                SourceType = NodeSourceType.Pin,
                FacetFilter = "person",
                Name = "Name",
                PartType = "it.vedph.bricks.names",
                TripleP = "foaf:name",
                TripleO = "$pin-value"
            };
            repository.AddMapping(nameMapping);
        }

        [Fact]
        public void AddMapping_NotExisting_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            // item mapping
            NodeMapping mapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                FacetFilter = "person",
                Name = "Item",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Description = "Map a person item into a node"
            };
            repository.AddMapping(mapping);

            Assert.True(mapping.Id > 0);
            NodeMapping mapping2 = repository.GetMapping(mapping.Id);
            Assert.Equal(mapping.SourceType, mapping2.SourceType);
            Assert.Equal(mapping.FacetFilter, mapping2.FacetFilter);
            Assert.Equal(mapping.Prefix, mapping2.Prefix);
            Assert.Equal(mapping.LabelTemplate, mapping2.LabelTemplate);
            Assert.Equal(mapping.Description, mapping2.Description);
        }

        [Fact]
        public void AddMapping_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            // item mapping
            NodeMapping mapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                FacetFilter = "person",
                Name = "Item",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Description = "Map a person item into a node"
            };
            repository.AddMapping(mapping);

            // update
            mapping.Description = "Updated!";
            repository.AddMapping(mapping);

            NodeMapping mapping2 = repository.GetMapping(mapping.Id);
            Assert.Equal(mapping.SourceType, mapping2.SourceType);
            Assert.Equal(mapping.FacetFilter, mapping2.FacetFilter);
            Assert.Equal(mapping.Prefix, mapping2.Prefix);
            Assert.Equal(mapping.LabelTemplate, mapping2.LabelTemplate);
            Assert.Equal(mapping.Description, mapping2.Description);
        }

        [Fact]
        public void DeleteMapping_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // item mapping
            NodeMapping mapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                FacetFilter = "person",
                Name = "Item",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Description = "Map a person item into a node"
            };
            repository.AddMapping(mapping);

            repository.DeleteMapping(123);

            Assert.NotNull(repository.GetMapping(mapping.Id));
        }

        [Fact]
        public void DeleteMapping_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // item mapping
            NodeMapping mapping = new NodeMapping
            {
                SourceType = NodeSourceType.Item,
                FacetFilter = "person",
                Name = "Item",
                Prefix = "x:persons/{group-id}/",
                LabelTemplate = "{title}",
                Description = "Map a person item into a node"
            };
            repository.AddMapping(mapping);

            repository.DeleteMapping(mapping.Id);

            Assert.Null(repository.GetMapping(mapping.Id));
        }

        private static IItem GetItem(int n, string facetId)
        {
            return new Item
            {
                Title = "Item #" + n,
                Description = "Dsc for item " + n,
                FacetId = facetId,
                SortKey = $"{n:000}",
                Flags = ((n & 1) == 1) ? 1 : 0,
                CreatorId = "creator",
                UserId = "user"
            };
        }

        [Fact]
        public void FindMappings_Item_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddMappings(repository);
            IItem item = GetItem(1, "person");

            IList<NodeMapping> mappings = repository.FindMappingsFor(item);

            Assert.Single(mappings);
            Assert.Equal("Item", mappings[0].Name);
        }

        [Fact]
        public void FindMappings_Pin_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddMappings(repository);
            IItem item = GetItem(1, "person");

            IList<NodeMapping> mappings = repository.FindMappingsFor(
                item, new NamesPart(), "name");

            Assert.Single(mappings);
            Assert.Equal("Name", mappings[0].Name);
        }

        [Fact]
        public void FindMappings_PinWithSuffix_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            repository.AddMapping(new NodeMapping
            {
                Name = "Pin with suffix",
                SourceType = NodeSourceType.Pin,
                PartType = "it.vedph.bricks.names",
                PinName = "color@*"
            });
            IItem item = GetItem(1, "person");

            IList<NodeMapping> mappings = repository.FindMappingsFor(
                item, new NamesPart(), "color@angel-1v");

            Assert.Single(mappings);
            Assert.Equal("Pin with suffix", mappings[0].Name);
        }
        #endregion

        #region Triple
        private static Triple AddMichelangeloArtist(IGraphRepository repository)
        {
            Node michelangelo = new Node
            {
                Id = repository.AddUri("x:persons/michelangelo"),
                Label = "Michelangelo"
            };
            repository.AddNode(michelangelo);
            Node a = new Node
            {
                Id = repository.AddUri("rdf:type"),
                Label = "a",
                Tag = Node.TAG_PROPERTY
            };
            repository.AddNode(a);
            Node artist = new Node
            {
                Id = repository.AddUri("x:artist"),
                IsClass = true,
                Label = "Artist class"
            };
            repository.AddNode(artist);

            // triple: michelangelo a artist
            Triple triple = new Triple
            {
                SubjectId = michelangelo.Id,
                PredicateId = a.Id,
                ObjectId = artist.Id
            };
            repository.AddTriple(triple);

            return triple;
        }

        [Fact]
        public void AddTriple_NotExisting_Added()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            Triple triple = AddMichelangeloArtist(repository);

            TripleResult triple2 = repository.GetTriple(triple.Id);
            Assert.NotNull(triple2);
            Assert.Equal(triple.SubjectId, triple2.SubjectId);
            Assert.Equal(triple.PredicateId, triple2.PredicateId);
            Assert.Equal(triple.ObjectId, triple2.ObjectId);
            Assert.Equal(triple.ObjectLiteral, triple2.ObjectLiteral);
            Assert.Equal(triple.Sid, triple2.Sid);
            Assert.Equal(triple.Tag, triple2.Tag);
        }

        [Fact]
        public void AddTriple_Same_Unchanged()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            Triple triple = AddMichelangeloArtist(repository);
            Triple triple2 = AddMichelangeloArtist(repository);

            Assert.Equal(triple.Id, triple2.Id);
            Assert.Equal(1, repository.GetTriples(new TripleFilter()).Total);
        }

        private static void AddTriples(IGraphRepository repository)
        {
            // nodes
            Node michelangelo = new Node
            {
                Id = repository.AddUri("x:persons/michelangelo"),
                Label = "Michelangelo"
            };
            repository.AddNode(michelangelo);

            Node a = new Node
            {
                Id = repository.AddUri("rdf:type"),
                Label = "a",
                Tag = Node.TAG_PROPERTY
            };
            repository.AddNode(a);

            Node name = new Node
            {
                Id = repository.AddUri("foaf:name"),
                Label = "Person name",
                Tag = Node.TAG_PROPERTY
            };
            repository.AddNode(name);

            Node artist = new Node
            {
                Id = repository.AddUri("x:artist"),
                IsClass = true,
                Label = "Artist class"
            };
            repository.AddNode(artist);

            // michelangelo a artist
            repository.AddTriple(new Triple
            {
                SubjectId = michelangelo.Id,
                PredicateId = a.Id,
                ObjectId = artist.Id
            });
            // michelangelo hasName "Michelangelo Buonarroti"
            repository.AddTriple(new Triple
            {
                SubjectId = michelangelo.Id,
                PredicateId = name.Id,
                ObjectLiteral = "Michelangelo Buonarroti",
                Tag = "fake",
                Sid = "d33d98de-7e61-4c67-8ddb-0cd1b4f03dae"
            });
        }

        [Fact]
        public void GetTriples_NoFilter_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter());

            Assert.Equal(2, page.Total);
            Assert.Equal(2, page.Items.Count);
        }

        [Fact]
        public void GetTriples_BySubjectId_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                SubjectId = 1
            });

            Assert.Equal(2, page.Total);
            Assert.Equal(2, page.Items.Count);
        }

        [Fact]
        public void GetTriples_ByPredicateId_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                PredicateId = 4
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
            Assert.NotNull(page.Items[0].ObjectLiteral);
        }

        [Fact]
        public void GetTriples_ByObjectId_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                ObjectId = 5
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
        }

        [Fact]
        public void GetTriples_ByLiteral_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                ObjectLiteral = "^Michelangelo"
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
            Assert.NotNull(page.Items[0].ObjectLiteral);
        }

        [Fact]
        public void GetTriples_BySidExact_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            DataPage<TripleResult> page = repository.GetTriples(new TripleFilter
            {
                Sid = "d33d98de-7e61-4c67-8ddb-0cd1b4f03dae"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);
        }

        [Fact]
        public void GetTriples_BySidPrefix_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            DataPage<TripleResult> page = repository.GetTriples(new TripleFilter
            {
                Sid = "d33d98de-7e61-4c67-8ddb-",
                IsSidPrefix = true
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
        }

        [Fact]
        public void GetTriples_ByNoTag_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                Tag = ""
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
            Assert.Null(page.Items[0].ObjectLiteral);
        }

        [Fact]
        public void GetTriples_ByTag_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            var page = repository.GetTriples(new TripleFilter
            {
                Tag = "fake"
            });

            Assert.Equal(1, page.Total);
            Assert.Single(page.Items);
            Assert.NotNull(page.Items[0].ObjectLiteral);
        }

        [Fact]
        public void GetTriple_NotExisting_Null()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            TripleResult triple = repository.GetTriple(123);

            Assert.Null(triple);
        }

        [Fact]
        public void GetTriple_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            TripleResult triple = repository.GetTriple(2);

            Assert.NotNull(triple);
            Assert.NotNull(triple.ObjectLiteral);
            Assert.Equal("x:persons/michelangelo", triple.SubjectUri);
            Assert.Equal("foaf:name", triple.PredicateUri);
            Assert.Null(triple.ObjectUri);
        }

        [Fact]
        public void DeleteTriple_NotExisting_Nope()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            repository.DeleteTriple(123);

            Assert.Equal(2, repository.GetTriples(new TripleFilter()).Total);
        }

        [Fact]
        public void DeleteTriple_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddTriples(repository);

            repository.DeleteTriple(2);

            Assert.NotNull(repository.GetTriple(1));
            Assert.Null(repository.GetTriple(2));
        }
        #endregion

        #region Thesaurus
        private static Thesaurus GetThesaurus()
        {
            Thesaurus thesaurus = new("geometry@en");
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes",
                Value = "shapes"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.2d",
                Value = "shapes: 2D"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.2d.circle",
                Value = "shapes: 2D: circle"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.2d.triangle",
                Value = "shapes: 2D: triangle"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.3d",
                Value = "shapes: 3D"
            });
            thesaurus.AddEntry(new ThesaurusEntry
            {
                Id = "shapes.3d.cube",
                Value = "shapes: 3D: cube"
            });

            return thesaurus;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("x:classes/")]
        public void AddThesaurus_NoRoot_Ok(string prefix)
        {
            Reset();
            IGraphRepository repository = GetRepository();
            Thesaurus thesaurus = GetThesaurus();

            repository.AddThesaurus(thesaurus, false, prefix);

            // get nodes and triples
            IList<NodeResult> nodes = repository.GetNodes(new NodeFilter
            {
                IsClass = true
            }).Items;
            Assert.Equal(7, nodes.Count);

            IList<TripleResult> triples =
                repository.GetTriples(new TripleFilter()).Items;
            Assert.Equal(5, triples.Count);

            // shapes (root)
            const string sub = "rdfs:subClassOf";
            NodeResult shapes = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes");
            Assert.NotNull(shapes);

            // shapes.2d
            NodeResult shapes2d = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.2d");
            Assert.NotNull(shapes2d);
            // shapes.2d subclass of shapes
            TripleResult triple = triples.FirstOrDefault(
                t => t.SubjectUri == shapes2d.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes.Uri);

            // shapes.3d
            NodeResult shapes3d = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.3d");
            Assert.NotNull(shapes3d);
            // shapes.3d subclass of shapes
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == shapes3d.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes.Uri);

            // shapes.2d.circle
            NodeResult node = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.2d.circle");
            Assert.NotNull(node);
            // shapes.2d.circle subclass of shapes.2d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes2d.Uri);

            // shapes.2d.triangle
            node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.2d.triangle");
            Assert.NotNull(node);
            // shapes.2d.triangle subclass of shapes.2d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes2d.Uri);

            // shapes.3d.cube
            node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.3d.cube");
            Assert.NotNull(node);
            // shapes.3d.cube subclass of shapes.3d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes3d.Uri);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("x:classes/")]
        public void AddThesaurus_Root_Ok(string prefix)
        {
            Reset();
            IGraphRepository repository = GetRepository();
            Thesaurus thesaurus = GetThesaurus();

            repository.AddThesaurus(thesaurus, true, prefix);

            // get nodes and triples
            IList<NodeResult> nodes = repository.GetNodes(new NodeFilter
            {
                IsClass = true
            }).Items;
            Assert.Equal(8, nodes.Count);

            IList<TripleResult> triples =
                repository.GetTriples(new TripleFilter()).Items;
            Assert.Equal(6, triples.Count);

            const string sub = "rdfs:subClassOf";

            // geometry (root from ID)
            NodeResult geometry = nodes.FirstOrDefault(
                n => n.Uri == prefix + "geometry");
            Assert.NotNull(geometry);

            // shapes (root entry)
            NodeResult shapes = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes");
            Assert.NotNull(shapes);
            // shapes subclass of geometry
            TripleResult triple = triples.FirstOrDefault(
                t => t.SubjectUri == shapes.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == geometry.Uri);

            // shapes.2d
            NodeResult shapes2d = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.2d");
            Assert.NotNull(shapes2d);
            // shapes.2d subclass of shapes
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == shapes2d.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes.Uri);

            // shapes.3d
            NodeResult shapes3d = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.3d");
            Assert.NotNull(shapes3d);
            // shapes.3d subclass of shapes
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == shapes3d.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes.Uri);

            // shapes.2d.circle
            NodeResult node = nodes.FirstOrDefault(
                n => n.Uri == prefix + "shapes.2d.circle");
            Assert.NotNull(node);
            // shapes.2d.circle subclass of shapes.2d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes2d.Uri);

            // shapes.2d.triangle
            node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.2d.triangle");
            Assert.NotNull(node);
            // shapes.2d.triangle subclass of shapes.2d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes2d.Uri);

            // shapes.3d.cube
            node = nodes.FirstOrDefault(n => n.Uri == prefix + "shapes.3d.cube");
            Assert.NotNull(node);
            // shapes.3d.cube subclass of shapes.3d
            triple = triples.FirstOrDefault(
                t => t.SubjectUri == node.Uri
                && t.PredicateUri == sub
                && t.ObjectUri == shapes3d.Uri);
        }
        #endregion

        [Fact]
        public async Task UpdateNodeClassesAsync_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            // add properties and classes
            Thesaurus thesaurus = GetThesaurus();
            repository.AddThesaurus(thesaurus, false);
            Node a = new()
            {
                Id = repository.AddUri("rdf:type"),
                Label = "is-a",
                Tag = "property"
            };
            repository.AddNode(a, true);
            Node sub = new()
            {
                Id = repository.AddUri("rdfs:subClassOf"),
                Label = "rdfs:subClassOf",
                Tag = "property"
            };
            repository.AddNode(sub, true);

            Node shape = repository.GetNodeByUri("shapes");
            Node circle = repository.GetNodeByUri("shapes.2d.circle");
            Node cube = repository.GetNodeByUri("shapes.3d.cube");

            // add nodes
            // dish a circle
            Node dish = new()
            {
                Id = repository.AddUri("x:dish"),
                Label = "dish"
            };
            repository.AddNode(dish);
            repository.AddTriple(new Triple
            {
                SubjectId = dish.Id,
                PredicateId = a.Id,
                ObjectId = circle.Id
            });
            // dice a cube
            Node dice = new()
            {
                Id = repository.AddUri("x:dice"),
                Label = "dice"
            };
            repository.AddNode(dice);
            repository.AddTriple(new Triple
            {
                SubjectId = dice.Id,
                PredicateId = a.Id,
                ObjectId = cube.Id
            });

            // classes hierarchy:
            // +-shapes
            // +--shapes.2d
            // +---shapes.2d.circle
            // +---shapes.2d.triangle
            // +--shapes.3d
            // +---shapes.3d.cube
            await repository.UpdateNodeClassesAsync(CancellationToken.None);

            DataPage<NodeResult> page = repository.GetNodes(new NodeFilter
            {
                ClassIds = new List<int>(new[] { shape.Id }),
                IsClass = false
            });
            Assert.Equal(2, page.Total);
        }
    }

    #region NamesPart
    [Tag("it.vedph.bricks.names")]
    internal sealed class NamesPart : PartBase
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

    // TODO graph and classes tests
}
