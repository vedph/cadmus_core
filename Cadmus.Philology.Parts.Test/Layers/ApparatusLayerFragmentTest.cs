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
        private static ApparatusLayerFragment GetFragment(int entryCount = 3)
        {
            ApparatusLayerFragment fr = new ApparatusLayerFragment
            {
                Location = "1.23",
                Tag = "tag"
            };
            for (int i = 1; i <= entryCount; i++)
            {
                ApparatusEntry entry = new ApparatusEntry
                {
                    Type = ApparatusEntryType.Replacement,
                    Tag = "tag" + i,
                    Value = "value" + i,
                    NormValue = "norm-value" + i,
                    IsAccepted = i == 1,
                    GroupId = "group-id",
                };
                entry.Witnesses.Add(new ApparatusAnnotatedValue
                {
                    Value = "w1",
                    Note = "note to w1"
                });
                entry.Authors.Add(new ApparatusAnnotatedValue
                {
                    Value = "a1",
                    Note = "note to a1"
                });

                fr.Entries.Add(entry);
            }

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
            Assert.Equal(fr.Tag, fr2.Tag);
            Assert.Equal(fr.Entries.Count, fr2.Entries.Count);
            for (int i = 0; i < fr.Entries.Count; i++)
            {
                ApparatusEntry expected = fr.Entries[i];
                ApparatusEntry actual = fr2.Entries[i];
                Assert.Equal(expected.Type, actual.Type);
                Assert.Equal(expected.Tag, actual.Tag);
                Assert.Equal(expected.Value, actual.Value);
                Assert.Equal(expected.NormValue, actual.NormValue);
                Assert.Equal(expected.IsAccepted, actual.IsAccepted);
                Assert.Equal(expected.GroupId, actual.GroupId);
                Assert.Equal(expected.Witnesses.Count, actual.Witnesses.Count);
                Assert.Equal(expected.Authors.Count, actual.Authors.Count);
                Assert.Equal(expected.Note, actual.Note);
            }
        }

        [Fact]
        public void GetDataPins_TwoEntries_2()
        {
            ApparatusLayerFragment fr = GetFragment(2);

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(2 + 1 + 1, pins.Count);

            List<DataPin> variantPins =
                pins.Where(p => p.Name == "fr.variant").ToList();
            Assert.Equal(2, variantPins.Count);
            Assert.Equal("norm-value1", variantPins[0].Value);
            Assert.Equal("norm-value2", variantPins[1].Value);

            List<DataPin> witnessPins =
                pins.Where(p => p.Name == "fr.witness").ToList();
            Assert.Single(witnessPins);
            Assert.Equal("w1", witnessPins[0].Value);

            List<DataPin> authorPins =
                pins.Where(p => p.Name == "fr.author").ToList();
            Assert.Single(authorPins);
            Assert.Equal("a1", authorPins[0].Value);
        }
    }
}
