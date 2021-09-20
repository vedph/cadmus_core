using Cadmus.Core;
using Cadmus.Parts.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace Cadmus.Index.MySql.Test
{
    // https://github.com/xunit/xunit/issues/1999
    [CollectionDefinition(nameof(NonParallelResourceCollection),
        DisableParallelization = true)]
    public class NonParallelResourceCollection { }
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class MySqlItemIndexWriterTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
        private const string DB_NAME = "cadmus-index-test";
        static private readonly string CS = string.Format(CST, DB_NAME);

        #region Helpers
        //private static void Reset()
        //{
        //    IDbManager manager = new MySqlDbManager(CST);
        //    if (manager.Exists(DB_NAME)) manager.ClearDatabase(DB_NAME);
        //}

        private static IDbConnection GetConnection() => new MySqlConnection(CS);

        private static IItemIndexWriter GetWriter()
        {
            MySqlItemIndexWriter writer = new MySqlItemIndexWriter();
            writer.Configure(new Sql.SqlOptions
            {
                ConnectionString = CS
            });
            return writer;
        }

        private static IItem GetItem(int n)
        {
            return new Item
            {
                Title = "Item #" + n,
                Description = "Dsc for item " + n,
                FacetId = "default",
                SortKey = $"{n:000}",
                Flags = ((n & 1) == 1)? 1 : 0,
                CreatorId = "creator",
                UserId = "user"
            };
        }

        private static void AssertItemsEqual(IItem expected, IItem actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Description, actual.Description);
            Assert.Equal(expected.FacetId, actual.FacetId);
            Assert.Equal(expected.GroupId, actual.GroupId);
            Assert.Equal(expected.SortKey, actual.SortKey);
            Assert.Equal(expected.Flags, actual.Flags);
            Assert.Equal(expected.CreatorId, actual.CreatorId);
            //Assert.Equal(expected.TimeCreated, actual.TimeCreated);
            //Assert.Equal(expected.TimeModified, actual.TimeModified);
        }

        private static int GetItemCount(IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT COUNT(id) FROM item;";
            int? result = cmd.ExecuteScalar() as int?;
            return result != null ? result.Value : 0;
        }

        private static bool ItemExists(string itemId, IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT 1 FROM item WHERE id=@id;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                Value = itemId,
                DbType = DbType.String,
            });
            long? result = cmd.ExecuteScalar() as long?;
            return result != null;
        }

        private static void ExpectItemInIndex(IItem item, IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT title,description,facetId,groupId,\n" +
                "sortKey,flags,timeCreated,creatorId,timeModified\n" +
                "FROM item WHERE id=@id;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                Value = item.Id,
                DbType = DbType.String,
            });
            using IDataReader reader = cmd.ExecuteReader();
            Assert.True(reader.Read());

            Item actual = new Item
            {
                Id = item.Id,
                Title = reader.GetString(0),
                Description = reader.GetString(1),
                FacetId = reader.GetString(2),
                GroupId = reader.IsDBNull(3)? null : reader.GetString(3),
                SortKey = reader.GetString(4),
                Flags = reader.GetInt32(5),
                TimeCreated = reader.GetDateTime(6),
                CreatorId = reader.GetString(7),
                TimeModified = reader.GetDateTime(8)
            };

            AssertItemsEqual(item, actual);
        }

        private static void ExpectItemPinCount(string itemId, int count,
            IDbConnection connection)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT COUNT(id) FROM pin WHERE itemId=@itemId;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@itemId",
                Value = itemId,
                DbType = DbType.String,
            });

            int? actual = cmd.ExecuteScalar() as int?;
            if (count == 0 && actual == null) return;
            Assert.Equal(count, actual.Value);
        }

        private static void ExpectItemPins(string itemId,
            IList<string> pins, IDbConnection connection)
        {
            List<Tuple<string, string>> expectedPins = new(pins.Count);
            foreach (string pin in pins)
            {
                int i = pin.IndexOf('=', StringComparison.Ordinal);
                Debug.Assert(i > -1);
                expectedPins.Add(Tuple.Create(
                    pin.Substring(0, i),
                    pin[(i + 1)..]));
            }

            IDbCommand cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = "SELECT name,value FROM pin WHERE itemId=@itemId;";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@itemId",
                Value = itemId,
                DbType = DbType.String,
            });
            using IDataReader reader = cmd.ExecuteReader();
            List<Tuple<string, string>> actualPins = new();
            while (reader.Read())
            {
                actualPins.Add(Tuple.Create(
                    reader.GetString(0),
                    reader.IsDBNull(1) ? null : reader.GetString(1)));
            }

            // compare
            Assert.Equal(pins.Count, actualPins.Count);

            foreach (var expected in expectedPins)
            {
                if (actualPins.All(p => p.Item1 != expected.Item1
                    && p.Item2 != expected.Item2))
                {
                    Assert.True(false, $"{expected.Item1}={expected.Item2}");
                }
            }
        }
        #endregion

        #region WriteItem
        [Fact]
        public void WriteItem_NoParts_Ok()
        {
            // an item without parts get written in item, nothing in pin
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            IItem item = GetItem(1);

            writer.WriteItem(item);

            using IDbConnection connection = GetConnection();
            connection.Open();
            ExpectItemInIndex(item, connection);
            ExpectItemPinCount(item.Id, 0, connection);
        }

        [Fact]
        public void WriteItem_Parts_Ok()
        {
            // an item with parts get written in item, its part's pins in pin
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            IItem item = GetItem(1);
            // add part
            IPart part;
            item.Parts.Add(part = new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag",
                Text = "A note",
                UserId = "user"
            });

            writer.WriteItem(item);

            using IDbConnection connection = GetConnection();
            connection.Open();
            ExpectItemInIndex(item, connection);
            ExpectItemPins(item.Id, new[] { "tag=tag" }, connection);
        }

        [Fact]
        public void WriteItem_ExistingWithoutParts_Ok()
        {
            // an item with parts is written; then, the item without parts
            // gets written again to update its metadata only; pin unchanged.
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            IItem item = GetItem(1);
            // add part
            item.Parts.Add(new NotePart
            {
                ItemId = item.Id,
                RoleId = null,
                CreatorId = "creator",
                UserId = "user",
                Tag = "tag",
                Text = "A note"
            });

            // item + note part
            writer.WriteItem(item);

            // update the item only
            item.Title = "Updated";
            item.Parts.Clear();
            writer.WriteItem(item);

            // expect item + note
            using IDbConnection connection = GetConnection();
            connection.Open();
            ExpectItemInIndex(item, connection);
            ExpectItemPins(item.Id, new[]
            {
                "tag=tag"
            }, connection);
        }

        [Fact]
        public void WriteItem_ExistingWithParts_Ok()
        {
            // an item with a part gets written; then, it is written with
            // another part added. The item is just updated, each part's pins
            // get replaced.
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            IItem item = GetItem(1);
            // add part
            NotePart notePart = new NotePart
            {
                ItemId = item.Id,
                RoleId = null,
                CreatorId = "creator",
                UserId = "user",
                Tag = "tag",
                Text = "A note"
            };
            item.Parts.Add(notePart);

            // item + note part
            writer.WriteItem(item);

            // item + text part
            item.Title = "Updated";
            notePart.Tag = "updated";
            TokenTextPart textPart;
            item.Parts.Add(textPart = new TokenTextPart
            {
                ItemId = item.Id,
                RoleId = null,
                CreatorId = "creator",
                UserId = "user",
                Citation = "Il.1,2"
            });
            textPart.Lines.Add(new TextLine
            {
                Y = 1,
                Text = "Hello world!"
            });
            writer.WriteItem(item);

            // expect item + note + text
            using IDbConnection connection = GetConnection();
            connection.Open();
            ExpectItemInIndex(item, connection);
            ExpectItemPins(item.Id, new[]
            {
                "tag=updated",
                "line-count=1",
                "citation=Il.1,2"
            }, connection);
        }
        #endregion

        #region WriteItems
        [Fact]
        public void WriteItems_Ok()
        {
            // 2 items get written with their parts.
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            // bulk write 2 items with their parts
            IItem item1 = GetItem(1);
            item1.Parts.Add(new NotePart
            {
                ItemId = item1.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag1",
                Text = "A note",
                UserId = "user"
            });
            IItem item2 = GetItem(2);
            item2.Parts.Add(new NotePart
            {
                ItemId = item2.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag2",
                Text = "A note",
                UserId = "user"
            });

            writer.WriteItems(new[] { item1, item2 }, CancellationToken.None);

            // expect items with their pins
            using IDbConnection connection = GetConnection();
            connection.Open();
            ExpectItemInIndex(item1, connection);
            ExpectItemPins(item1.Id, new[]
            {
                "tag=tag1"
            }, connection);
            ExpectItemInIndex(item2, connection);
            ExpectItemPins(item2.Id, new[]
            {
                "tag=tag2"
            }, connection);
        }
        #endregion

        #region DeleteItem
        [Fact]
        public void DeleteItem_NotExisting_Nope()
        {
            // a non-existing item is deleted: nothing happens
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            writer.DeleteItem("not-existing");

            using IDbConnection connection = GetConnection();
            connection.Open();
            Assert.Equal(0, GetItemCount(connection));
        }

        [Fact]
        public void DeleteItem_Existing_Ok()
        {
            // an existing item with its pins is deleted;
            // all other data are unchanged.
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            // write item1 with part
            IItem item1 = GetItem(1);
            item1.Parts.Add(new NotePart
            {
                ItemId = item1.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag1",
                Text = "A note",
                UserId = "user"
            });
            writer.WriteItem(item1);

            // write item2 with part
            IItem item2 = GetItem(2);
            item2.Parts.Add(new NotePart
            {
                ItemId = item2.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag2",
                Text = "A note",
                UserId = "user"
            });
            writer.WriteItem(item2);

            // delete item1
            writer.DeleteItem(item1.Id);

            // item1 does not exist, item2 exists
            using IDbConnection connection = GetConnection();
            connection.Open();
            Assert.False(ItemExists(item1.Id, connection));
            Assert.True(ItemExists(item2.Id, connection));

            // item1 pins do not exist, item2 pins exist
            ExpectItemPins(item1.Id, Array.Empty<string>(), connection);
            ExpectItemPins(item2.Id, new[] { "tag=tag2" }, connection);
        }
        #endregion

        #region DeletePart
        [Fact]
        public void DeletePart_NotExisting_Nope()
        {
            // a non-existing part is deleted: nothing happens
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            writer.DeletePart("not-existing");

            using IDbConnection connection = GetConnection();
            connection.Open();
            Assert.Equal(0, GetItemCount(connection));
        }

        [Fact]
        public void DeletePart_Existing_Ok()
        {
            // an existing item with 2 parts has 1 of its part deleted;
            // the other part's pins are unchanged.
            IItemIndexWriter writer = GetWriter();
            writer.Clear();

            // write item1 with 2 parts
            IItem item = GetItem(1);
            item.Parts.Add(new NotePart
            {
                ItemId = item.Id,
                CreatorId = "creator",
                RoleId = null,
                Tag = "tag",
                Text = "A note",
                UserId = "user"
            });
            TokenTextPart textPart;
            item.Parts.Add(textPart = new TokenTextPart
            {
                ItemId = item.Id,
                RoleId = null,
                CreatorId = "creator",
                UserId = "user",
                Citation = "Il.1,2"
            });
            textPart.Lines.Add(new TextLine
            {
                Y = 1,
                Text = "Hello world!"
            });
            writer.WriteItem(item);

            // delete text part
            writer.DeletePart(textPart.Id);

            // item exists
            using IDbConnection connection = GetConnection();
            connection.Open();
            Assert.True(ItemExists(item.Id, connection));

            // item's pins for note part exist
            ExpectItemPins(item.Id, new[] { "tag=tag" }, connection);
        }
        #endregion
    }
}
