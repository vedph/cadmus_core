using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class KeywordsPartTest
    {
        private static KeywordsPart GetPart(bool keywords = true)
        {
            KeywordsPart part = new KeywordsPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
            };
            if (keywords)
            {
                part.AddKeyword("eng", "red");
                part.AddKeyword("eng", "green");
                part.AddKeyword("ita", "rosso");
            }
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            KeywordsPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            KeywordsPart part2 = TestHelper.DeserializePart<KeywordsPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Keywords.Count, part2.Keywords.Count);

            foreach (Keyword expected in part.Keywords)
            {
                Assert.Contains(part2.Keywords,
                    k => k.Language == expected.Language
                    && k.Value == expected.Value);
            }
        }

        [Fact]
        public void GetDataPins_NoKeywords_Empty()
        {
            KeywordsPart part = GetPart(false);

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Tag_3()
        {
            KeywordsPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(3, pins.Count);

            // keyword.eng = green
            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("keyword.eng", pin.Name);
            Assert.Equal("green", pin.Value);

            // keyword.eng = red
            pin = pins[1];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("keyword.eng", pin.Name);
            Assert.Equal("red", pin.Value);

            // keyword.ita = rosso
            pin = pins[2];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("keyword.ita", pin.Name);
            Assert.Equal("rosso", pin.Value);
        }
    }
}
