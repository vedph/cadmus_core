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

        private static TiledTextPart GetPart(int count)
        {
            return new TiledTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Citation = "some-citation",
                Rows = GetRows(count)
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            TiledTextPart part = GetPart(2);

            string json = TestHelper.SerializePart(part);
            TiledTextPart part2 = TestHelper.DeserializePart<TiledTextPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            List<TextTileRow> expectedRows = GetRows(2);
            Assert.Equal(expectedRows.Count, part.Rows.Count);
            for (int i = 0; i < expectedRows.Count; i++)
            {
                Assert.Equal(expectedRows[i].Y, part.Rows[i].Y);
                Assert.Equal(expectedRows[i].Tiles.Count, part.Rows[i].Tiles.Count);
                Assert.Equal(expectedRows[i].Data.Count, part.Rows[i].Data.Count);
            }
        }

        [Fact]
        public void GetDataPins_NoCitation_1()
        {
            TiledTextPart part = GetPart(0);
            part.Citation = null;

            List<DataPin> pins = part.GetDataPins(null).ToList();

            Assert.Single(pins);
            Assert.Equal("row-count", pins[0].Name);
            Assert.Equal("0", pins[0].Value);
            TestHelper.AssertValidDataPinNames(part, pins[0]);
        }

        [Fact]
        public void GetDataPins_Citation_2()
        {
            TiledTextPart part = GetPart(3);

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "row-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("3", pin.Value);

            pin = pins.Find(p => p.Name == "citation");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("some-citation", pin.Value);
        }
    }
}
