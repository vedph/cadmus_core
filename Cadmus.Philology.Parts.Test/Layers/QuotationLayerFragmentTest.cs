using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class QuotationLayerFragmentTest
    {
        private static QuotationLayerFragment GetFragment()
        {
            return new QuotationLayerFragment
            {
                Location = "1.23",
                Author = "Hom.",
                Work = "Il.",
                Citation = "3.24",
                VariantOf = "original",
                Note = "note"
            };
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(QuotationLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            QuotationLayerFragment fr = GetFragment();

            string json = TestHelper.SerializeFragment(fr);
            QuotationLayerFragment fr2 =
                TestHelper.DeserializeFragment<QuotationLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Author, fr2.Author);
            Assert.Equal(fr.Work, fr2.Work);
            Assert.Equal(fr.Citation, fr2.Citation);
            Assert.Equal(fr.VariantOf, fr2.VariantOf);
            Assert.Equal(fr.Note, fr2.Note);
        }

        [Fact]
        public void GetDataPins_NoAuthor_0()
        {
            QuotationLayerFragment fr = GetFragment();
            fr.Author = null;

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_NoWork_0()
        {
            QuotationLayerFragment fr = GetFragment();
            fr.Work = null;

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_AuthorAndWork_2()
        {
            QuotationLayerFragment fr = GetFragment();

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);
            DataPin pin = pins[0];
            Assert.Equal("fr.author", pin.Name);
            Assert.Equal("Hom.", pin.Value);

            pin = pins[1];
            Assert.Equal("fr.work", pin.Name);
            Assert.Equal("Il.", pin.Value);
        }
    }
}
