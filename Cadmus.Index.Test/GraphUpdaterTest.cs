﻿using Cadmus.Core;
using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Fusi.Tools.Data;
using System;
using System.Linq;
using Xunit;

namespace Cadmus.Index.Test
{
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class GraphUpdaterTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
        private const string DB_NAME = "cadmus-index-test";
        static private readonly string CS = string.Format(CST, DB_NAME);

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

        [Fact]
        public void Update_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            NodeMapperTest.AddDeepEntityPartRules(repository);
            repository.AddNode(new Node
            {
                Id = repository.AddUri("x:classes/death"),
                SourceType = NodeSourceType.User,
                IsClass = true,
                Label = "Death class"
            });

            NodeMapper mapper = new NodeMapper(repository);

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
            GraphSet oldSet = mapper.MapPins(item, part, new[]
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
            // 1) store initial set
            GraphUpdater updater = new GraphUpdater(repository);
            updater.Update(oldSet);

            var nodePage = repository.GetNodes(new NodeFilter());
            Assert.Equal(11, nodePage.Total);

            var triplePage = repository.GetTriples(new TripleFilter());
            Assert.Equal(7, triplePage.Total);

            // 2) add user-edited stuff:
            // - barbato a person
            // - laura a person
            Node a = repository.GetNodeByUri("a");
            Node personClass = new Node
            {
                Id = repository.AddUri("foaf:Person"),
                SourceType = NodeSourceType.User,
                IsClass = true,
                Label = "Person class"
            };
            repository.AddNode(personClass);
            repository.AddTriple(new Triple
            {
                SubjectId = repository.LookupId("x:persons/scipione_barbato"),
                PredicateId = a.Id,
                ObjectId = personClass.Id
            });
            repository.AddTriple(new Triple
            {
                SubjectId = repository.LookupId("x:persons/laura"),
                PredicateId = a.Id,
                ObjectId = personClass.Id
            });
            // simulate an edit by recreating pins and remap
            GraphSet newSet = mapper.MapPins(item, part, new[]
            {
                Tuple.Create("eid", "birth"),
                // wedding was removed, death was added
                Tuple.Create("eid", "death"),

                Tuple.Create("type@birth", "x:classes/birth"),
                // birth date was updated (1300 -> 1299)
                Tuple.Create("x:hasDate@birth", "1299"),

                Tuple.Create("type@death", "x:classes/death"),
                Tuple.Create("x:hasDate@death", "1345"),
            });

            // 3) update; the new set has nodes:
            // - x:persons/scipione_barbato [U]
            // - x:events/birth [P] ...|eid|birth
            // - x:classes/birth [U]
            // - x:events/death [P] ...|eid|death
            // - x:classes/death [U]
            // and triples:
            // - x:events/birth - #kad:isInGroup - x:persons/scipione_barbato
            // - x:events/death - #kad:isInGroup - x:persons/scipione_barbato
            // - x:events/birth - #a - x:classes/birth
            // - x:events/birth - #x:hasDate - 1299
            // - x:events/death - #a - x:classes/death
            // - x:events/death - #x:hasDate - 1345

            // you can disable transactions when debugging
            // updater.IsTransactionDisabled = true;
            updater.Update(newSet);

            // resulting nodes:
            DataPage<NodeResult> nPage = repository.GetNodes(new NodeFilter());
            Assert.Equal(11, nPage.Total);
            var nodes = nPage.Items;

            // barbato
            NodeResult barbato = nodes.FirstOrDefault(
                n => n.Uri == "x:persons/scipione_barbato");
            Assert.NotNull(barbato);

            // birth
            NodeResult birth = nodes.FirstOrDefault(
                n => n.Uri == "x:events/birth");
            Assert.NotNull(birth);

            // death
            NodeResult death = nodes.FirstOrDefault(
                n => n.Uri == "x:events/death");
            Assert.NotNull(death);

            // no more wedding nor laura
            Assert.Null(nodes.FirstOrDefault(n => n.Uri == "x:events/wedding"));
            Assert.Null(nodes.FirstOrDefault(n => n.Uri == "x:persons/laura"));

            // resulting triples:
            DataPage<TripleResult> tPage = repository.GetTriples(new TripleFilter());
            var triples = tPage.Items;
            Assert.Equal(9, tPage.Total);

            // barbato a person
            Assert.Contains(triples, t => t.SubjectUri == barbato.Uri
                && t.PredicateUri == "a"
                && t.ObjectUri == "foaf:Person");

            // birth in-group barbato
            Assert.Contains(triples, t => t.SubjectUri == birth.Uri
                && t.PredicateUri == "kad:isInGroup"
                && t.ObjectUri == barbato.Uri);

            // birth a birth-class
            Assert.Contains(triples, t => t.SubjectUri == birth.Uri
                && t.PredicateUri == "a"
                && t.ObjectUri == "x:classes/birth");

            // birth hasDate 1299
            Assert.Contains(triples, t => t.SubjectUri == birth.Uri
                && t.PredicateUri == "x:hasDate"
                && t.ObjectLiteral == "1299");

            // birth hasDate 1300 no more exists
            Assert.DoesNotContain(triples, t => t.SubjectUri == birth.Uri
                && t.PredicateUri == "x:hasDate"
                && t.ObjectLiteral == "1300");

            // death in-group barbato
            Assert.Contains(triples, t => t.SubjectUri == death.Uri
                && t.PredicateUri == "kad:isInGroup"
                && t.ObjectUri == barbato.Uri);

            // death a death-class
            Assert.Contains(triples, t => t.SubjectUri == death.Uri
                && t.PredicateUri == "a"
                && t.ObjectUri == "x:classes/death");

            // death hasDate 1345
            Assert.Contains(triples, t => t.SubjectUri == death.Uri
                && t.PredicateUri == "x:hasDate"
                && t.ObjectLiteral == "1345");
        }

        [Fact]
        public void Delete_NotExisting_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();

            GraphUpdater updater = new GraphUpdater(repository);

            updater.Delete("not-existing");
        }

        [Fact]
        public void Delete_Existing_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            NodeMapperTest.AddDeepEntityPartRules(repository);

            NodeMapper mapper = new NodeMapper(repository);

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

            GraphSet oldSet = mapper.MapPins(item, part, new[]
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
            // store set
            GraphUpdater updater = new GraphUpdater(repository);
            updater.Update(oldSet);
            NodeFilter nf = new NodeFilter();
            TripleFilter tf = new TripleFilter();

            var nodePage = repository.GetNodes(nf);
            Assert.Equal(10, nodePage.Total);

            var triplePage = repository.GetTriples(tf);
            Assert.Equal(7, triplePage.Total);

            // delete set
            updater.Delete(part.Id);

            nodePage = repository.GetNodes(nf);
            Assert.Equal(7, nodePage.Total);
            Assert.Equal(4, nodePage.Items.Count(n => n.Tag == Node.TAG_PROPERTY));
            Assert.Equal(2, nodePage.Items.Count(n => n.IsClass));
            Assert.NotNull(nodePage.Items.FirstOrDefault(
                n => n.Uri == "x:persons/scipione_barbato"));

            triplePage = repository.GetTriples(tf);
            Assert.Equal(0, triplePage.Total);
        }
    }
}
