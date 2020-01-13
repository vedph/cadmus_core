using Cadmus.Core;
using Cadmus.Philology.Parts.Layers;
using Fusi.Tools.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Cadmus.Philology.Parts.Test.Layers
{
    public sealed class WitnessesLayerFragmentTest
    {
        private static WitnessesLayerFragment GetFragment(int count)
        {
            WitnessesLayerFragment fr = new WitnessesLayerFragment
            {
                Location = "1.23",
            };
            for (int i = 0; i < count; i++)
            {
                fr.Witnesses.Add(new Witness
                {
                    Id = new string((char)('A' + i), 1),
                    Citation = $"1.{i}",
                    Text = "Text {i + 1}",
                    Note = "Note {i + 1}\n.End."
                });
            }
            return fr;
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(WitnessesLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            WitnessesLayerFragment fr = GetFragment(3);

            string json = TestHelper.SerializeFragment(fr);
            WitnessesLayerFragment fr2 =
                TestHelper.DeserializeFragment<WitnessesLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            int i = 0;
            foreach (Witness expected in fr.Witnesses)
            {
                Witness actual = fr2.Witnesses[i++];
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Citation, actual.Citation);
                Assert.Equal(expected.Text, actual.Text);
                Assert.Equal(expected.Note, actual.Note);
            }
        }

        [Fact]
        public void GetDataPins_NoWitnesses_0()
        {
            WitnessesLayerFragment fr = GetFragment(0);

            Assert.Empty(fr.GetDataPins());
        }

        [Fact]
        public void GetDataPins_Witnesses_3()
        {
            WitnessesLayerFragment fr = GetFragment(3);

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(3, pins.Count);

            DataPin pin = pins[0];
            Assert.Equal("fr.id", pin.Name);
            Assert.Equal("A", pin.Value);

            pin = pins[1];
            Assert.Equal("fr.id", pin.Name);
            Assert.Equal("B", pin.Value);

            pin = pins[2];
            Assert.Equal("fr.id", pin.Name);
            Assert.Equal("C", pin.Value);
        }
    }
}
