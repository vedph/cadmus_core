using System;
using Xunit;
using Cadmus.Core;
using Cadmus.Parts.General;
using Cadmus.Seed.Parts.General;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Bricks;
using Cadmus.Refs.Bricks;

namespace Cadmus.Parts.Test.General
{
    public sealed class CommentPartTest
    {
        private static CommentPart GetPart()
        {
            CommentPartSeeder seeder = new CommentPartSeeder();
            seeder.Configure(new CommentPartSeederOptions());
            IItem item = new Item
            {
                FacetId = "default",
                CreatorId = "zeus",
                UserId = "zeus",
                Description = "Test item",
                Title = "Test Item",
                SortKey = ""
            };
            return (CommentPart)seeder.GetPart(item, null, null);
        }

        private static CommentPart GetEmptyPart()
        {
            return new CommentPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            CommentPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            CommentPart part2 = TestHelper.DeserializePart<CommentPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            // TODO: check parts data here...
        }

        [Fact]
        public void GetDataPins_Empty_Ok()
        {
            CommentPart part = GetEmptyPart();
            part.Tag = null;

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_NotEmpty_Ok()
        {
            CommentPart part = GetEmptyPart();
            part.Tag = "tag";
            for (int n = 1; n <= 3; n++)
            {
                bool even = n % 2 == 0;

                part.References.Add(new DocReference
                {
                    Citation = $"w{n}"
                });
                part.ExternalIds.Add(new ExternalId { Value = $"i{n}" });
                part.Categories.Add($"c{n}");
                part.Keywords.Add(new IndexKeyword
                {
                    IndexId = even? "even" : "odd",
                    Language = "eng",
                    Value = $"k{(char)('a' - 1 + n)}"
                });
            }

            List<DataPin> pins = part.GetDataPins(null).ToList();
            Assert.Equal(13, pins.Count);

            // tag
            DataPin pin = pins.Find(p => p.Name == "tag");
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("tag", pin.Value);

            // key.odd.eng=ka
            pin = pins.Find(p => p.Name == "key.odd.eng" && p.Value == "ka");
            TestHelper.AssertValidDataPinNames(part, pin);

            // key.even.eng=kb
            pin = pins.Find(p => p.Name == "key.even.eng" && p.Value == "kb");
            TestHelper.AssertValidDataPinNames(part, pin);

            // key.odd.eng=kc
            pin = pins.Find(p => p.Name == "key.odd.eng" && p.Value == "kc");
            TestHelper.AssertValidDataPinNames(part, pin);

            for (int n = 1; n <= 3; n++)
            {
                // ref
                pin = pins.Find(p => p.Name == "ref" && p.Value == $"w{n}");
                Assert.NotNull(pin);
                TestHelper.AssertValidDataPinNames(part, pin);

                // id
                pin = pins.Find(p => p.Name == "id" && p.Value == $"i{n}");
                Assert.NotNull(pin);
                TestHelper.AssertValidDataPinNames(part, pin);

                // cat
                pin = pins.Find(p => p.Name == "cat" && p.Value == $"c{n}");
                Assert.NotNull(pin);
                TestHelper.AssertValidDataPinNames(part, pin);
            }
        }
    }
}
