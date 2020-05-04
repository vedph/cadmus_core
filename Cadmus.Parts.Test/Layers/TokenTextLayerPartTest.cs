using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Parts.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static TokenTextPart GetTokenTextPart(int lineCount)
        {
            TokenTextPart part = new TokenTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                CreatorId = "zeus",
                UserId = "zeus"
            };
            char c = 'a';
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < lineCount; y++)
            {
                sb.Clear();

                for (int x = 0; x < 3; x++)
                {
                    if (x > 0) sb.Append(' ');
                    sb.Append(c).Append(x + 1);
                    if (++c > 'z') c = 'a';
                }
                part.Lines.Add(new TextLine
                {
                    Y = y + 1,
                    Text = sb.ToString()
                });
            }
            return part;
        }

        [Fact]
        public void GetTextAt_InvalidLocation_Null()
        {
            TokenTextPart textPart = GetTokenTextPart(3);
            TokenTextLayerPart<CommentLayerFragment> layerPart = GetPart();

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
            TokenTextPart textPart = GetTokenTextPart(2);
            TokenTextLayerPart<CommentLayerFragment> layerPart = GetPart();

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
            TokenTextPart textPart = GetTokenTextPart(2);
            TokenTextLayerPart<CommentLayerFragment> layerPart = GetPart();

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
            TokenTextPart textPart = GetTokenTextPart(3);
            TokenTextLayerPart<CommentLayerFragment> layerPart = GetPart();

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
            TokenTextPart textPart = GetTokenTextPart(3);
            TokenTextLayerPart<CommentLayerFragment> layerPart = GetPart();

            string text = layerPart.GetTextAt(textPart, location);

            Assert.Equal(expectedText, text);
        }
    }
}
