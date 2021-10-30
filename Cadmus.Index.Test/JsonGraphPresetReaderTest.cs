using Cadmus.Index.Graph;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        public async Task ReadNodes_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<UriNode> nodes = await reader.ReadNodesAsync(
                GetResourceStream("PresetNodes.json"));

            Assert.Equal(10, nodes.Count);
            Assert.Equal("is-a", nodes[0].Label);
            Assert.Equal("lemma", nodes[9].Label);
        }

        [Fact]
        public async Task ReadNodeMappings_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<NodeMapping> mappings = await reader.ReadMappingsAsync(
                GetResourceStream("PresetMappings.json"));

            Assert.Equal(10, mappings.Count);
            Assert.Equal("Lemma item", mappings[0].Name);
            Assert.Equal("Pin variant@* x:hasIxVariantForm ...", mappings[9].Name);
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, mappings[i].Id);
        }

        [Fact]
        public async Task ReadNodeMappingsWithOffset_Ok()
        {
            JsonGraphPresetReader reader = new();

            IList<NodeMapping> mappings = await reader.ReadMappingsAsync(
                GetResourceStream("PresetMappings.json"));
            IList<NodeMapping> mappings2 = await reader.ReadMappingsAsync(
                GetResourceStream("PresetMappings.json"), 10);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(mappings[i].Id + 10, mappings2[i].Id);
                Assert.Equal(mappings[i].ParentId + 10, mappings2[i].ParentId);
            }
        }
    }
}
