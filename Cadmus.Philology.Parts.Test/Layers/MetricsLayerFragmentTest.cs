using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class MetricsLayerFragmentTest
    {
        private static MetricsLayerFragment GetFragment()
        {
            return new MetricsLayerFragment
            {
                Location = "1.23",
                Metre = "6da^",
                Number = "123",
                IsImperfect = true,
                Note = "Some note."
            };
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(MetricsLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            MetricsLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            MetricsLayerFragment fr2 =
                TestHelper.DeserializeFragment<MetricsLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Metre, fr2.Metre);
            Assert.Equal(fr.Number, fr2.Number);
            Assert.Equal(fr.IsImperfect, fr2.IsImperfect);
            Assert.Equal(fr.Note, fr2.Note);
        }

        [Fact]
        public void GetDataPins_NoMetre_0()
        {
            MetricsLayerFragment fr = GetFragment();
            fr.Metre = null;

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Metre_1()
        {
            MetricsLayerFragment fr = GetFragment();

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal("fr.metre", pin.Name);
            Assert.Equal("6da^*", pin.Value);
        }
    }
}
