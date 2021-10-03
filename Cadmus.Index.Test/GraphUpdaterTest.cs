﻿using Cadmus.Core;
using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
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
            Assert.Equal(10, nodePage.Total);

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
            // - x:persons/scipione_barbato x:persons/scipione_barbato [U]
            // - x:events/birth birth [P] ...|eid|birth
            // - x:classes/birth Birth class [U]
            // - x:events/death death [P] ...|eid|death
            // - x:classes/death x:classes/death [U]
            // and triples:
            // - x:events/birth - #kad:isInGroup - x:persons/scipione_barbato
            // - x:events/death - #kad:isInGroup - x:persons/scipione_barbato
            // - x:events/birth - #a - x:classes/birth
            // - x:events/birth - #x:hasDate - 1299
            // - x:events/death - #a - x:classes/death
            // - x:events/death - #x:hasDate - 1345
            updater.Update(newSet);

            // resulting nodes:
            // scipione
            NodeResult node = repository.GetNodeByUri("x:persons/scipione_barbato");
            Assert.NotNull(node);
            // birth
            // death
            // no more wedding nor laura
            Assert.Null(repository.GetNodeByUri("x:events/wedding"));
            Assert.Null(repository.GetNodeByUri("x:persons/laura"));
            // TODO
        }
    }
}
