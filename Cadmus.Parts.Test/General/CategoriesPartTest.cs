using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class CategoriesPartTest
    {
        private static CategoriesPart GetPart(params string[] categories)
        {
            CategoriesPart part = new CategoriesPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another"
            };
            foreach (string category in categories)
                part.Categories.Add(category);
            return part;
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            CategoriesPart part = GetPart("alpha", "beta");

            string json = TestHelper.SerializePart(part);
            CategoriesPart part2 = TestHelper.DeserializePart<CategoriesPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);

            Assert.Equal(2, part.Categories.Count);
            Assert.Contains("alpha", part.Categories);
            Assert.Contains("beta", part.Categories);
        }

        [Fact]
        public void GetDataPins_NoCategories_Ok()
        {
            CategoriesPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("tot-count", pin.Name);
            Assert.Equal("0", pin.Value);
        }

        [Fact]
        public void GetDataPins_TwoCategories_2()
        {
            CategoriesPart part = GetPart("alpha", "beta");

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(3, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "tot-count");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
            Assert.Equal("2", pin.Value);

            pin = pins.Find(p => p.Name == "category"
                && p.Value == "alpha");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);

            pin = pins.Find(p => p.Name == "category"
                && p.Value == "beta");
            Assert.NotNull(pin);
            TestHelper.AssertValidDataPinNames(part, pin);
        }
    }
}
