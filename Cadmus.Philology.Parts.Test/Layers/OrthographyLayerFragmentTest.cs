using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class OrthographyLayerFragmentTest
    {
        private static OrthographyLayerFragment GetFragment(
            params string[] operations)
        {
            OrthographyLayerFragment fr = new OrthographyLayerFragment
            {
                Location = "1.23",
                Standard = "bixit",
            };
            foreach (string operation in operations)
                fr.Operations.Add(operation);
            return fr;
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(OrthographyLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            OrthographyLayerFragment fr = GetFragment("\"b\"@1x1=\"v\"");

            string json = TestHelper.SerializeFragment(fr);
            OrthographyLayerFragment fr2 =
                TestHelper.DeserializeFragment<OrthographyLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Standard, fr2.Standard);
            Assert.Equal(fr.Operations.Count, fr2.Operations.Count);
            Assert.Equal(fr.Operations[0], fr2.Operations[0]);
        }

        [Fact]
        public void GetDataPins_NoOperations_0()
        {
            OrthographyLayerFragment fr = GetFragment();

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_NoTaggedOperations_0()
        {
            OrthographyLayerFragment fr = GetFragment("\"b\"@1x1=\"v\"");

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_TaggedOperations_2()
        {
            OrthographyLayerFragment fr = GetFragment(
                "\"b\"@1x1=\"v\" [c]",
                "\"e\"@1x1=\"ae\" [v]",
                "\"b\"@1x1=\"p\" [c]");

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(2, pins.Count);
            DataPin pin = pins[0];
            Assert.Equal("fr.msp.c", pin.Name);
            Assert.Equal("2", pin.Value);

            pin = pins[1];
            Assert.Equal("fr.msp.v", pin.Name);
            Assert.Equal("1", pin.Value);
        }
    }
}
