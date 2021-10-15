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
    public sealed class ExternalIdsPartTest
    {
        private static ExternalIdsPart GetPart()
        {
            ExternalIdsPartSeeder seeder = new ExternalIdsPartSeeder();
            IItem item = new Item
            {
                FacetId = "default",
                CreatorId = "zeus",
                UserId = "zeus",
                Description = "Test item",
                Title = "Test Item",
                SortKey = ""
            };
            return (ExternalIdsPart)seeder.GetPart(item, null, null);
        }

        private static ExternalIdsPart GetEmptyPart()
        {
            return new ExternalIdsPart
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
            ExternalIdsPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            ExternalIdsPart part2 = TestHelper.DeserializePart<ExternalIdsPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Ids.Count, part2.Ids.Count);
        }

        [Fact]
        public void GetDataPins_NoTag_1()
        {
            ExternalIdsPart part = GetEmptyPart();

            List<DataPin> pins = part.GetDataPins(null).ToList();
            Assert.Single(pins);
            Assert.Equal("tot-count", pins[0].Name);
            Assert.Equal("0", pins[0].Value);
        }

        [Fact]
        public void GetDataPins_Ids_Ok()
        {
            ExternalIdsPart part = GetEmptyPart();
            for (int n = 1; n <= 3; n++)
            {
                part.Ids.Add(new ExternalId
                {
                    Value = $"{n:000}",
                    Tag = "tag"
                });
            }

            List<DataPin> pins = part.GetDataPins(null).ToList();
            Assert.Equal(4, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count" && p.Value == "3");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);

            pin = pins.Find(p => p.Name == "id" && p.Value == "001");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);

            pin = pins.Find(p => p.Name == "id" && p.Value == "002");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);

            pin = pins.Find(p => p.Name == "id" && p.Value == "003");
            Assert.NotNull(pin);
            TestHelper.AssertPinIds(part, pin);
        }
    }
}
