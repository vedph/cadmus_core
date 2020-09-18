using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class TokenTextPartTest
    {
        private static TokenTextPart GetPart(int lineCount)
        {
            TokenTextPart part = new TokenTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Citation = "some-citation"
            };
            for (int y = 1; y <= lineCount; y++)
            {
                part.Lines.Add(new TextLine
                {
                    Y = y,
                    Text = $"Line {y}."
                });
            }
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            TokenTextPart part = GetPart(2);

            string json = TestHelper.SerializePart(part);
            TokenTextPart part2 = TestHelper.DeserializePart<TokenTextPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Citation, part2.Citation);
            Assert.Equal(part.Lines.Count, part2.Lines.Count);
            int i = 0;
            foreach (TextLine expected in part.Lines)
            {
                TextLine actual = part2.Lines[i++];
                Assert.Equal(expected.Y, actual.Y);
                Assert.Equal(expected.Text, actual.Text);
            }
        }

        [Fact]
        public void GetDataPins_NoCitation_1()
        {
            TokenTextPart part = GetPart(0);
            part.Citation = null;

            List<DataPin> pins = part.GetDataPins(null).ToList();

            Assert.Single(pins);
            Assert.Equal("line-count", pins[0].Name);
            Assert.Equal("0", pins[0].Value);
            TestHelper.AssertPinIds(part, pins[0]);
        }

        [Fact]
        public void GetDataPins_Citation_2()
        {
            TokenTextPart part = GetPart(3);

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "line-count");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);
            Assert.Equal("3", pin.Value);

            pin = pins.Find(p => p.Name == "citation");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);
            Assert.Equal("some-citation", pin.Value);
        }
    }
}
