using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class LigatureLayerFragmentTest
    {
        private static LigatureLayerFragment GetFragment()
        {
            return new LigatureLayerFragment
            {
                Location = "1.23",
                Type = LigatureType.Ligature
            };
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(LigatureLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            LigatureLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            LigatureLayerFragment fr2 =
                TestHelper.DeserializeFragment<LigatureLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Type, fr2.Type);
        }

        [Fact]
        public void GetDataPins_Ligature_1()
        {
            LigatureLayerFragment fr = GetFragment();

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal("fr.ligature", pin.Name);
            Assert.Equal("L", pin.Value);
        }
    }
}
