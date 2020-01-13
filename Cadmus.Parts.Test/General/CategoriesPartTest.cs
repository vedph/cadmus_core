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
        public void GetDataPins_NoCategories_Empty()
        {
            CategoriesPart part = GetPart();

            Assert.Empty(part.GetDataPins());
        }

        [Fact]
        public void GetDataPins_TwoCategories_2()
        {
            CategoriesPart part = GetPart("alpha", "beta");

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Equal(2, pins.Count);

            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("category", pin.Name);
            Assert.Equal("alpha", pin.Value);

            pin = pins[1];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("category", pin.Name);
            Assert.Equal("beta", pin.Value);
        }
    }
}
