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
        private static QuotationsLayerFragment GetFragment(int count)
        {
            QuotationsLayerFragment fr = new QuotationsLayerFragment
            {
                Location = "1.2"
            };

            for (int i = 0; i < count; i++)
            {
                char c = (char)('a' + i);
                fr.Entries.Add(new QuotationEntry
                {
                    Author = $"author-{c}",
                    Work = $"work-{c}",
                    Citation = $"{i + 1}",
                    CitationUri = $"urn:{i + 1}",
                    Variant = $"variant-{c}",
                    Tag = $"t-{c}",
                    Note = $"note-{c}"
                });
            }

            return fr;
        }

        [Fact]
        public void Fragment_Has_Tag()
        {
            TagAttribute attr = typeof(QuotationsLayerFragment).GetTypeInfo()
                .GetCustomAttribute<TagAttribute>();
            string typeId = attr != null ? attr.Tag : GetType().FullName;
            Assert.NotNull(typeId);
            Assert.StartsWith(PartBase.FR_PREFIX, typeId);
        }

        [Fact]
        public void Fragment_Is_Serializable()
        {
            QuotationsLayerFragment fr = GetFragment(2);

            string json = TestHelper.SerializeFragment(fr);
            QuotationsLayerFragment fr2 =
                TestHelper.DeserializeFragment<QuotationsLayerFragment>(json);

            Assert.Equal(fr.Location, fr2.Location);
            Assert.Equal(fr.Entries.Count, fr2.Entries.Count);
            for (int i = 0; i < fr.Entries.Count; i++)
            {
                QuotationEntry expEntry = fr.Entries[i];
                QuotationEntry actEntry = fr2.Entries[i];
                Assert.Equal(expEntry.Author, actEntry.Author);
                Assert.Equal(expEntry.Work, actEntry.Work);
                Assert.Equal(expEntry.Citation, actEntry.Citation);
                Assert.Equal(expEntry.CitationUri, actEntry.CitationUri);
                Assert.Equal(expEntry.Variant, actEntry.Variant);
                Assert.Equal(expEntry.Tag, actEntry.Tag);
                Assert.Equal(expEntry.Note, actEntry.Note);
            }
        }

        [Fact]
        public void GetDataPins_NoEntries_Ok()
        {
            QuotationsLayerFragment fr = GetFragment(0);

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Single(pins);
            DataPin pin = pins[0];
            Assert.Equal("fr.tot-count", pin.Name);
            Assert.Equal("0", pin.Value);
        }

        [Fact]
        public void GetDataPins_SingleEntry_Ok()
        {
            QuotationsLayerFragment fr = GetFragment(1);

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(5, pins.Count);

            // fr.tot-count
            DataPin pin = pins.Find(p => p.Name == "fr.tot-count");
            Assert.NotNull(pin);
            Assert.Equal("1", pin.Value);

            // fr.author
            pin = pins.Find(p => p.Name == "fr.author");
            Assert.NotNull(pin);
            Assert.Equal("authora", pin.Value);

            // fr.work
            pin = pins.Find(p => p.Name == "fr.work");
            Assert.NotNull(pin);
            Assert.Equal("worka", pin.Value);

            // fr.cit-uri
            pin = pins.Find(p => p.Name == "fr.cit-uri");
            Assert.NotNull(pin);
            Assert.Equal("urn:1", pin.Value);

            // fr.tag-X-count
            pin = pins.Find(p => p.Name == "fr.tag-t-a-count");
            Assert.NotNull(pin);
            Assert.Equal("1", pin.Value);
        }

        [Fact]
        public void GetDataPins_MultipleEntries_Ok()
        {
            QuotationsLayerFragment fr = GetFragment(2);

            List<DataPin> pins = fr.GetDataPins().ToList();

            Assert.Equal(9, pins.Count);

            // fr.tot-count
            DataPin pin = pins.Find(p => p.Name == "fr.tot-count");
            Assert.NotNull(pin);
            Assert.Equal("2", pin.Value);

            // fr.author
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.author" && p.Value == "authora"));
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.author" && p.Value == "authorb"));

            // fr.work
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.work" && p.Value == "worka"));
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.work" && p.Value == "workb"));

            // fr.cit-uri
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.cit-uri" && p.Value == "urn:1"));
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.cit-uri" && p.Value == "urn:2"));

            // fr.tag-X-count
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.tag-t-a-count" && p.Value == "1"));
            Assert.NotNull(pins.Find(
                p => p.Name == "fr.tag-t-b-count" && p.Value == "1"));
        }
    }
}
