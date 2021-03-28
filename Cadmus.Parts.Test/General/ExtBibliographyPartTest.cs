using System;
using Xunit;
using Cadmus.Core;
using Cadmus.Seed.Parts.General;
using Cadmus.Parts.General;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Parts.Test.General
{
    public sealed class ExtBibliographyPartTest
    {
        private static ExtBibliographyPart GetPart()
        {
            ExtBibliographyPartSeeder seeder = new ExtBibliographyPartSeeder();
            IItem item = new Item
            {
                FacetId = "default",
                CreatorId = "zeus",
                UserId = "zeus",
                Description = "Test item",
                Title = "Test Item",
                SortKey = ""
            };
            return (ExtBibliographyPart)seeder.GetPart(item, null, null);
        }

        private static ExtBibliographyPart GetEmptyPart()
        {
            return new ExtBibliographyPart
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
            ExtBibliographyPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            ExtBibliographyPart part2 =
                TestHelper.DeserializePart<ExtBibliographyPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.NotEmpty(part2.Entries);
        }

        [Fact]
        public void GetDataPins_NoEntry_Count()
        {
            ExtBibliographyPart part = GetEmptyPart();

            List<DataPin> pins = part.GetDataPins(null).ToList();
            Assert.Single(pins);
            Assert.Equal("tot-count", pins[0].Name);
            Assert.Equal("0", pins[0].Value);
        }

        [Fact]
        public void GetDataPins_Entries_Ok()
        {
            ExtBibliographyPart part = GetEmptyPart();

            for (int n = 1; n <= 3; n++)
            {
                part.Entries.Add(new ExtBibEntry
                {
                    Id = n.ToString(CultureInfo.InvariantCulture),
                    Label = $"Title {n}",
                    Tag = n % 2 == 0? "even" : "odd"
                });
            }

            List<DataPin> pins = part.GetDataPins(null).ToList();
            Assert.Equal(6, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("3", pin.Value);

            // labels
            for (int n = 1; n <= 3; n++)
            {
                pin = pins.Find(p => p.Name == "label"
                    && p.Value == $"title {n}");
                Assert.NotNull(pin);
                TestHelper.AssertValidDataPinNames(part, pin);
            }

            // tags
            pin = pins.Find(p => p.Name == "tag" && p.Value == "even");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            pin = pins.Find(p => p.Name == "tag" && p.Value == "odd");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
        }
    }
}
