using Cadmus.Core;
using Cadmus.Parts.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.Layers
{
    public sealed class TokenTextLayerPartTest
    {
        private static TokenTextLayerPart<CommentLayerFragment> GetPart(
            bool fragments = true)
        {
            var part = new TokenTextLayerPart<CommentLayerFragment>
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
                TokenTextLayerPart<CommentLayerFragment>>(json);

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
    }
}
