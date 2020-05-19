using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Antiquity.Chronology;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class ChronologyLayerFragmentTest
    {
        private static ChronologyLayerFragment GetFragment(string eventId = null)
        {
            return new ChronologyLayerFragment
            {
                Location = "1.23",
                Label = "Battle of Marathon",
                Tag = "battle",
                Date = HistoricalDate.Parse("490 BC"),
                EventId = eventId
            };
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(ChronologyLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            ChronologyLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            ChronologyLayerFragment fr2 =
                TestHelper.DeserializeFragment<ChronologyLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Label, fr2.Label);
            Assert.Equal(fr.Tag, fr2.Tag);
            Assert.Equal(fr.Date.ToString(), fr2.Date.ToString());
        }

        [Fact]
        public void GetDataPins_NoDate_0()
        {
            ChronologyLayerFragment fr = GetFragment();
            fr.Date = null;
            fr.Tag = null;

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_NoTag_1()
        {
            ChronologyLayerFragment fr = GetFragment();
            fr.Tag = null;

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal("fr.date-value", pin.Name);
            Assert.Equal(
                fr.Date.GetSortValue().ToString(CultureInfo.InvariantCulture),
                pin.Value);
        }

        [Fact]
        public void GetDataPins_Tag_2()
        {
            ChronologyLayerFragment fr = GetFragment();

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "fr.date-value");
            Assert.NotNull(pin);
            Assert.Equal(
                fr.Date.GetSortValue().ToString(CultureInfo.InvariantCulture),
                pin.Value);

            pin = pins.Find(p => p.Name == "fr.tag");
            Assert.NotNull(pin);
            Assert.Equal(fr.Tag, pin.Value);
        }

        [Fact]
        public void GetDataPins_TagAndEventId_3()
        {
            ChronologyLayerFragment fr = GetFragment("events:marathon_battle");

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(3, pins.Count);

            DataPin pin = pins.Find(p => p.Name == "fr.date-value");
            Assert.NotNull(pin);
            Assert.Equal(
                fr.Date.GetSortValue().ToString(CultureInfo.InvariantCulture),
                pin.Value);

            pin = pins.Find(p => p.Name == "fr.tag");
            Assert.NotNull(pin);
            Assert.Equal(fr.Tag, pin.Value);

            pin = pins.Find(p => p.Name == "fr.event-id");
            Assert.NotNull(pin);
            Assert.Equal(fr.EventId, pin.Value);
        }
    }
}
