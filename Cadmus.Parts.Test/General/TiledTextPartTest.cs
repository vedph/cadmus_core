using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class TiledTextPartTest
    {
        private static Dictionary<string, string> GetData(int count, bool text)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (text) data[TextTileRow.TEXT_DATA_NAME] = "Text";
            for (int i = 0; i < count; i++)
                data[$"d{i}"] = i.ToString(CultureInfo.InvariantCulture);
            return data;
        }

        private static List<TextTile> GetTiles(int count)
        {
            List<TextTile> tiles = new List<TextTile>();
            for (int x = 1; x <= count; x++)
            {
                tiles.Add(new TextTile
                {
                    X = x,
                    Data = GetData(3, true)
                });
            }
            return tiles;
        }

        private static List<TextTileRow> GetRows(int count)
        {
            List<TextTileRow> rows = new List<TextTileRow>();
            for (int y = 1; y <= count; y++)
            {
                rows.Add(new TextTileRow
                {
                    Y = y,
                    Tiles = GetTiles(3),
                    Data = GetData(3, false)
                });
            }
            return rows;
        }

        private static TiledTextPart GetPart()
        {
            return new TiledTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Citation = "some-citation",
                Rows = GetRows(3)
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            TiledTextPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            TiledTextPart part2 = TestHelper.DeserializePart<TiledTextPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            List<TextTileRow> expectedRows = GetRows(3);
            Assert.Equal(expectedRows.Count, part.Rows.Count);
            for (int i = 0; i < expectedRows.Count; i++)
            {
                Assert.Equal(expectedRows[i].Y, part.Rows[i].Y);
                Assert.Equal(expectedRows[i].Tiles.Count, part.Rows[i].Tiles.Count);
                Assert.Equal(expectedRows[i].Data.Count, part.Rows[i].Data.Count);
            }
        }

        [Fact]
        public void GetDataPins_NoCitation_Empty()
        {
            TiledTextPart part = GetPart();
            part.Citation = null;

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Citation_1()
        {
            TiledTextPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);

            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("citation", pin.Name);
            Assert.Equal("some-citation", pin.Value);
        }
    }
}
