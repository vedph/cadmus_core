using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.Layers
{
    public sealed class TiledTextLayerPartTest
    {
        private static TiledTextLayerPart<CommentLayerFragment> GetPart(
            bool fragments = true)
        {
            var part = new TiledTextLayerPart<CommentLayerFragment>
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another"
            };

            if (fragments)
            {
                for (int i = 1; i <= 3; i++)
                {
                    part.AddFragment(new CommentLayerFragment
                    {
                        Location = $"1.{i}",
                        Tag = i > 1 ? "scholarly" : null,
                        Text = "Text {i}.\nEnd."
                    });
                }
            }
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            var part = GetPart();

            string json = TestHelper.SerializePart(part);
            var part2 = TestHelper.DeserializePart<
                TiledTextLayerPart<CommentLayerFragment>>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            Assert.Equal(part.Fragments.Count, part2.Fragments.Count);
            foreach (CommentLayerFragment expected in part.Fragments)
            {
                CommentLayerFragment actual = part2.Fragments.Find(
                    fr => fr.Location == expected.Location);
                Assert.NotNull(actual);
                Assert.Equal(expected.Tag, actual.Tag);
                Assert.Equal(expected.Text, actual.Text);
            }
        }

        [Fact]
        public void GetDataPins_NoFragments_Empty()
        {
            var part = GetPart(false);

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Fragments_2()
        {
            var part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(2, pins.Count);

            foreach (DataPin pin in pins)
            {
                Assert.Equal(part.ItemId, pin.ItemId);
                Assert.Equal(part.Id, pin.PartId);
                Assert.Equal(part.RoleId, pin.RoleId);
                Assert.Equal("fr.tag", pin.Name);
                Assert.Equal("scholarly", pin.Value);
            }
        }

        private static TiledTextPart GetTiledTextPart(int rowCount)
        {
            TiledTextPart part = new TiledTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                CreatorId = "zeus",
                UserId = "zeus"
            };
            char c = 'a';

            for (int y = 0; y < rowCount; y++)
            {
                TextTileRow row = new TextTileRow
                {
                    Y = y + 1
                };
                part.Rows.Add(row);

                for (int x = 0; x < 3; x++)
                {
                    TextTile tile = new TextTile
                    {
                        X = x + 1,
                    };
                    tile.Data[TextTileRow.TEXT_DATA_NAME] = $"{c}{x + 1}";
                    row.Tiles.Add(tile);
                    if (++c > 'z') c = 'a';
                }
            }
            return part;
        }

        [Fact]
        public void GetTextAt_InvalidLocation_Null()
        {
            TiledTextPart textPart = GetTiledTextPart(3);
            TiledTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, "12.3");

            Assert.Null(text);
        }

        [Theory]
        [InlineData("1.1", "a1")]
        [InlineData("1.2", "b2")]
        [InlineData("1.3", "c3")]
        [InlineData("2.1", "d1")]
        [InlineData("2.2", "e2")]
        [InlineData("2.3", "f3")]
        public void GetTextAt_SingleToken_Ok(string location, string expectedText)
        {
            TiledTextPart textPart = GetTiledTextPart(2);
            TiledTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, location);

            Assert.Equal(expectedText, text);
        }

        [Theory]
        [InlineData("1.1-1.2", "a1 b2")]
        [InlineData("1.2-1.3", "b2 c3")]
        [InlineData("1.1-1.3", "a1 b2 c3")]
        [InlineData("2.1-2.2", "d1 e2")]
        [InlineData("2.2-2.3", "e2 f3")]
        [InlineData("2.1-2.3", "d1 e2 f3")]
        public void GetTextAt_SingleLineRange_Ok(string location, string expectedText)
        {
            TiledTextPart textPart = GetTiledTextPart(2);
            TiledTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, location);

            Assert.Equal(expectedText, text);
        }

        [Theory]
        [InlineData("1.1-2.3", "a1 b2 c3\r\nd1 e2 f3")]
        [InlineData("1.2-2.3", "b2 c3\r\nd1 e2 f3")]
        [InlineData("1.3-2.3", "c3\r\nd1 e2 f3")]
        [InlineData("1.1-2.2", "a1 b2 c3\r\nd1 e2")]
        [InlineData("1.1-2.1", "a1 b2 c3\r\nd1")]
        public void GetTextAt_2LinesRange_Ok(string location, string expectedText)
        {
            TiledTextPart textPart = GetTiledTextPart(3);
            TiledTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, location);

            Assert.Equal(expectedText, text);
        }

        [Theory]
        [InlineData("1.1-3.3", "a1 b2 c3\r\nd1 e2 f3\r\ng1 h2 i3")]
        [InlineData("1.2-3.3", "b2 c3\r\nd1 e2 f3\r\ng1 h2 i3")]
        [InlineData("1.3-3.3", "c3\r\nd1 e2 f3\r\ng1 h2 i3")]
        [InlineData("1.1-3.2", "a1 b2 c3\r\nd1 e2 f3\r\ng1 h2")]
        [InlineData("1.1-3.1", "a1 b2 c3\r\nd1 e2 f3\r\ng1")]
        public void GetTextAt_3LinesRange_Ok(string location, string expectedText)
        {
            TiledTextPart textPart = GetTiledTextPart(3);
            TiledTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, location);

            Assert.Equal(expectedText, text);
        }
    }
}
