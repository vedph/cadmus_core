using Cadmus.Core;
using Cadmus.Parts.General;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class NumberingPartTest
    {
        private static NumberingPart GetPart()
        {
            return new NumberingPart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Number = "123",
                Ordinal = 123,
                Tag = "some-tag"
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            NumberingPart part = GetPart();

            string json = TestHelper.SerializePart(part);
            NumberingPart part2 = TestHelper.DeserializePart<NumberingPart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Number, part2.Number);
            Assert.Equal(part.Ordinal, part2.Ordinal);
            Assert.Equal(part.Tag, part2.Tag);
        }

        [Fact]
        public void GetDataPins_NoTag_1()
        {
            NumberingPart part = GetPart();
            part.Tag = null;

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("ordinal", pin.Name);
            Assert.Equal(" 123", pin.Value);
        }

        [Fact]
        public void GetDataPins_Tag_1()
        {
            NumberingPart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("ordinal", pin.Name);
            Assert.Equal("some-tag 123", pin.Value);
        }
    }
}
