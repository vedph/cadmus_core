using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class ApparatusLayerFragmentTest
    {
        private static ApparatusLayerFragment GetFragment()
        {
            ApparatusLayerFragment fr = new ApparatusLayerFragment
            {
                Location = "1.23",
                Type = LemmaVariantType.Replacement,
                IsAccepted = true,
                Value = "variant",
                Note = "dubitanter"
            };
            fr.Authors.Add("alpha");
            fr.Authors.Add("beta");
            return fr;
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(ApparatusLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            ApparatusLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            ApparatusLayerFragment fr2 =
                TestHelper.DeserializeFragment<ApparatusLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Type, fr2.Type);
            Assert.Equal(fr.IsAccepted, fr2.IsAccepted);
            Assert.Equal(fr.Value, fr2.Value);
            Assert.Equal(fr.Note, fr2.Note);
            Assert.Equal(fr.Authors.Count, fr2.Authors.Count);
            Assert.True(fr.Authors.SetEquals(fr2.Authors));
        }

        [Fact]
        public void GetDataPins_NoAuthors_0()
        {
            ApparatusLayerFragment fr = GetFragment();
            fr.Authors.Clear();

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Authors_2()
        {
            ApparatusLayerFragment fr = GetFragment();

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);

            DataPin pin = pins[0];
            Assert.Equal("fr.author", pin.Name);
            Assert.Equal("alpha", pin.Value);
            pin = pins[1];
            Assert.Equal("fr.author", pin.Name);
            Assert.Equal("beta", pin.Value);
        }
    }
}
