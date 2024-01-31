using Cadmus.Core.Config;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Cadmus.Graph.Test;

public sealed class JsonGraphPresetReaderTest
{
    private Stream GetResourceStream(string name)
    {
        return GetType().Assembly.GetManifestResourceStream(
            "Cadmus.Graph.Test.Assets." + name)!;
    }

    [Fact]
    public void ReadNodes_Ok()
    {
        JsonGraphPresetReader reader = new();

        IList<UriNode> nodes = reader.ReadNodes(
            GetResourceStream("Nodes.json")).ToList();

        Assert.Equal(10, nodes.Count);
        Assert.Equal("is-a", nodes[0].Label);
        Assert.Equal("lemma", nodes[9].Label);
    }

    [Fact]
    public void ReadNodeMappings_Ok()
    {
        JsonGraphPresetReader reader = new();

        IList<NodeMapping> mappings = reader.LoadMappings(
            GetResourceStream("Mappings.json"));

        Assert.Equal(2, mappings.Count);
        foreach (NodeMapping mapping in mappings)
        {
            Assert.NotNull(mapping.Output);
            Assert.True(mapping.HasChildren);
            Assert.NotEmpty(mapping.Children);
        }
    }

    [Fact]
    public void ReadThesauri_Ok()
    {
        JsonGraphPresetReader reader = new();

        IList<Thesaurus> thesauri = reader.ReadThesauri(
            GetResourceStream("Thesauri.json")).ToList();

        Assert.Equal(2, thesauri.Count);
        Assert.Equal("colors@en", thesauri[0].Id);
        Assert.Equal("shapes@en", thesauri[1].Id);
    }
}
