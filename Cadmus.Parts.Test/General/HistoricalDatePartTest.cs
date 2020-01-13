using Cadmus.Core;
using Cadmus.Parts.General;
using Fusi.Antiquity.Chronology;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Cadmus.Parts.Test.General
{
    public sealed class HistoricalDatePartTest
    {
        private static HistoricalDatePart GetPart()
        {
            return new HistoricalDatePart
            {
                ItemId = Guid.NewGuid().ToString(),
                RoleId = "some-role",
                CreatorId = "zeus",
                UserId = "another",
                Date = HistoricalDate.Parse("c. 12 mag 23 AD -- 25 AD")
            };
        }

        [Fact]
        public void Part_Is_Serializable()
        {
            HistoricalDatePart part = GetPart();

            string json = TestHelper.SerializePart(part);
            HistoricalDatePart part2 = TestHelper.DeserializePart<HistoricalDatePart>(json);

            Assert.Equal(part.Id, part2.Id);
            Assert.Equal(part.TypeId, part2.TypeId);
            Assert.Equal(part.ItemId, part2.ItemId);
            Assert.Equal(part.RoleId, part2.RoleId);
            Assert.Equal(part.CreatorId, part2.CreatorId);
            Assert.Equal(part.UserId, part2.UserId);
            Assert.Equal(part.Date.ToString(), part2.Date.ToString());
        }

        [Fact]
        public void GetDataPins_NoDate_1()
        {
            HistoricalDatePart part = GetPart();
            part.Date = null;

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);

            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("date-sort-value", pin.Name);
            Assert.Equal("0", pin.Value);
        }

        [Fact]
        public void GetDataPins_Date_1()
        {
            HistoricalDatePart part = GetPart();

            List<DataPin> pins = part.GetDataPins().ToList();
            Assert.Single(pins);

            DataPin pin = pins[0];
            Assert.Equal(part.ItemId, pin.ItemId);
            Assert.Equal(part.Id, pin.PartId);
            Assert.Equal(part.RoleId, pin.RoleId);
            Assert.Equal("date-sort-value", pin.Name);
            Assert.Equal(
                part.Date.GetSortValue().ToString(CultureInfo.InvariantCulture),
                pin.Value);
        }
    }
}
