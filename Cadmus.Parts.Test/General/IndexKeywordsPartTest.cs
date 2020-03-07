using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class IndexKeywordsPartTest
    {
        private static IndexKeywordsPart GetPart(bool keywords = true)
        {
            IndexKeywordsPart part = new IndexKeywordsPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another"
            };
            if (keywords)
            {
                part.AddKeyword(new IndexKeyword
                {
                    IndexId = "colors",
                    Language = "eng",
                    Value = "red",
                    Note = "red note",
                    Tag = "tag"
                });
                part.AddKeyword(new IndexKeyword
                {
                    IndexId = "colors",
                    Language = "eng",
                    Value = "green",
                    Note = "green note",
                    Tag = "tag"
                });
                part.AddKeyword(new IndexKeyword
                {
                    Language = "eng",
                    Value = "Greek"
                });
            }
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            IndexKeywordsPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            IndexKeywordsPart part2 =
                TestHelper.DeserializePart<IndexKeywordsPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Keywords.Count, part2.Keywords.Count);

            foreach (IndexKeyword expected in part.Keywords)
            {
                Assert.Contains(part2.Keywords,
                    k => k.IndexId == expected.IndexId
                    && k.Language == expected.Language
                    && k.Value == expected.Value);
            }
        }

        [Fact]
        public void GetDataPins_NoKeywords_Empty()
        {
            IndexKeywordsPart part = GetPart(false);

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Tag_3()
        {
            IndexKeywordsPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(3, pins.Count);

            // xkeyword..eng = Greek
            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("xkeyword..eng", pin.Name);
            Assert.Equal("Greek", pin.Value);

            // xkeyword.colors.eng = green
            pin = pins[1];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("xkeyword.colors.eng", pin.Name);
            Assert.Equal("green", pin.Value);

            // xkeyword.colors.eng = red
            pin = pins[2];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("xkeyword.colors.eng", pin.Name);
            Assert.Equal("red", pin.Value);
        }
    }
}
