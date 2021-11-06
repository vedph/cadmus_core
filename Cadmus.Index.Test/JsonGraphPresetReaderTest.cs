using Cadmus.Core.Config;
using Cadmus.Index.Graph;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Cadmus.Index.Test
{
    public sealed class JsonGraphPresetReaderTest
    {
        private Stream GetResourceStream(string name)
        {
            return GetType().Assembly.GetManifestResourceStream(
                "Cadmus.Index.Test.Assets." + name);
        }

        [Fact]
        public void ReadNodes_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<UriNode> nodes = reader.ReadNodes(
                GetResourceStream("PresetNodes.json")).ToList();

            Assert.Equal(10, nodes.Count);
            Assert.Equal("is-a", nodes[0].Label);
            Assert.Equal("lemma", nodes[9].Label);
        }

        [Fact]
        public void ReadNodeMappings_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<NodeMapping> mappings = reader.ReadMappings(
                GetResourceStream("PresetMappings.json")).ToList();

            Assert.Equal(10, mappings.Count);
            Assert.Equal("Lemma item", mappings[0].Name);
            Assert.Equal("Pin variant@* x:hasIxVariantForm ...", mappings[9].Name);
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, mappings[i].Id);
        }

        [Fact]
        public void ReadNodeMappingsWithOffset_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<NodeMapping> mappings = reader.ReadMappings(
                GetResourceStream("PresetMappings.json")).ToList();
            IList<NodeMapping> mappings2 = reader.ReadMappings(
                GetResourceStream("PresetMappings.json"), 10).ToList();

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(mappings[i].Id + 10, mappings2[i].Id);
                Assert.Equal(mappings[i].ParentId + 10, mappings2[i].ParentId);
            }
        }

        [Fact]
        public void ReadThesauri_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<Thesaurus> thesauri = reader.ReadThesauri(
                GetResourceStream("PresetThesauri.json")).ToList();

            Assert.Equal(2, thesauri.Count);
            Assert.Equal("colors@en", thesauri[0].Id);
            Assert.Equal("shapes@en", thesauri[1].Id);
        }
    }
}
