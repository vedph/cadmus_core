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
                UserId = "another"
            };
            if (keywords)
            {
                part.AddKeyword("eng", "Red");
                part.AddKeyword("eng", "Green");
                part.AddKeyword("ita", "Rosso");
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
        public void GetDataPins_Tag_4()
        {
            KeywordsPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(7, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("3", pin.Value);

            // keyword.eng = green
            pin = pins.Find(p => p.Name == "keyword.eng"
                && p.Value == "green");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            pin = pins.Find(p => p.Name == "u-keyword.eng"
                && p.Value == "Green");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            // keyword.eng = red
            pin = pins.Find(p => p.Name == "keyword.eng"
                && p.Value == "red");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            pin = pins.Find(p => p.Name == "u-keyword.eng"
                && p.Value == "Red");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            // keyword.ita = rosso
            pin = pins.Find(p => p.Name == "keyword.ita"
                && p.Value == "rosso");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            pin = pins.Find(p => p.Name == "u-keyword.ita"
                && p.Value == "Rosso");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
        }
    }
}
