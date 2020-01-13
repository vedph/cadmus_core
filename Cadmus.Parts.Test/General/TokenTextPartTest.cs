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
        private static TokenTextPart GetPart()
        {
            TokenTextPart part = new TokenTextPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Citation = "some-citation"
            };
            for (int y = 1; y <= 3; y++)
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
            TokenTextPart part = GetPart();

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
        public void GetDataPins_NoCitation_Empty()
        {
            TokenTextPart part = GetPart();
            part.Citation = null;

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Citation_1()
        {
            TokenTextPart part = GetPart();

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
