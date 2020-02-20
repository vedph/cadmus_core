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

        // YXLayerPartBase<TFragment>, we test these once

        [Fact]
        public void AddFragment_NoOverlap_Added()
        {
            var part = GetPart();

            part.AddFragment(new CommentLayerFragment
            {
                Location = "2.1",
                Tag = "scholarly",
                Text = "The comment."
            });

            Assert.Equal(4, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.2"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "2.1"));
        }

        [Fact]
        public void AddFragment_Overlap_Replaced()
        {
            var part = GetPart();

            part.AddFragment(new CommentLayerFragment
            {
                Location = "1.1",
                Tag = "scholarly",
                Text = "New at 1.1"
            });
            part.AddFragment(new CommentLayerFragment
            {
                Location = "1.3-2.1",
                Tag = "scholarly",
                Text = "New at 1.3-2.1"
            });

            Assert.Equal(3, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.2"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3-2.1"));
        }

        [Fact]
        public void DeleteFragmentsAtIntegral_NoMatch_Nope()
        {
            var part = GetPart();

            part.DeleteFragmentsAtIntegral("5.1");

            Assert.Equal(3, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.2"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3"));
        }

        [Fact]
        public void DeleteFragmentsAtIntegral_RangeOverlap_Nope()
        {
            var part = GetPart();
            part.AddFragment(new CommentLayerFragment
            {
                Location = "1.4-1.6",
                Tag = "scholarly",
                Text = "The comment."
            });

            part.DeleteFragmentsAtIntegral("1.4");

            Assert.Equal(4, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.2"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.4-1.6"));
        }

        [Fact]
        public void DeleteFragmentsAtIntegral_IntegralMatch_Deleted()
        {
            var part = GetPart();

            part.DeleteFragmentsAtIntegral("1.2");

            Assert.Equal(2, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3"));
        }

        [Fact]
        public void DeleteFragmentsAtIntegral_NonIntegralMatch_Deleted()
        {
            var part = GetPart();
            part.AddFragment(new CommentLayerFragment
            {
                Location = "5.1@1x3",
                Tag = "scholarly",
                Text = "The comment."
            });

            part.DeleteFragmentsAtIntegral("5.1");

            Assert.Equal(3, part.Fragments.Count);
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.1"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.2"));
            Assert.NotNull(part.Fragments.Find(fr => fr.Location == "1.3"));
        }

        [Fact]
        public void GetFragmentsAtIntegral_NoMatch_Nope()
        {
            var part = GetPart();

            IList<CommentLayerFragment> frr = part.GetFragmentsAtIntegral("5.1");

            Assert.Empty(frr);
        }

        [Fact]
        public void GetFragmentsAtIntegral_RangeOverlap_Nope()
        {
            var part = GetPart();
            part.AddFragment(new CommentLayerFragment
            {
                Location = "1.4-1.6",
                Tag = "scholarly",
                Text = "The comment."
            });

            IList<CommentLayerFragment> frr = part.GetFragmentsAtIntegral("5.1");

            Assert.Empty(frr);
        }

        [Fact]
        public void GetFragmentsAtIntegral_IntegralMatch_Ok()
        {
            var part = GetPart();

            IList<CommentLayerFragment> frr = part.GetFragmentsAtIntegral("1.2");

            Assert.Single(frr);
            Assert.Equal("1.2", frr[0].Location);
        }

        [Fact]
        public void GetFragmentsAtIntegral_NonIntegralMatch_Ok()
        {
            var part = GetPart();
            part.AddFragment(new CommentLayerFragment
            {
                Location = "5.1@1x3",
                Tag = "scholarly",
                Text = "The comment."
            });

            IList<CommentLayerFragment> frr = part.GetFragmentsAtIntegral("5.1");

            Assert.Single(frr);
            Assert.Equal("5.1@1x3", frr[0].Location);
        }

        [Fact]
        public void GetFragmentsAt_NoOverlap_Nope()
        {
            var part = GetPart();

            IList<CommentLayerFragment> frr = part.GetFragmentsAt("5.1");

            Assert.Empty(frr);
        }

        [Fact]
        public void GetFragmentsAt_Overlap_Ok()
        {
            var part = GetPart();

            IList<CommentLayerFragment> frr = part.GetFragmentsAt("1.2-3.1");

            Assert.Equal(2, frr.Count);
            Assert.NotNull(frr.FirstOrDefault(fr => fr.Location == "1.2"));
            Assert.NotNull(frr.FirstOrDefault(fr => fr.Location == "1.3"));
        }
    }
}
