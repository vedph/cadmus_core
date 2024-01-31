using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Cadmus.Graph.Test;

public sealed class NodeMappingDocumentTest
{
    private readonly JsonSerializerOptions _options;

    public NodeMappingDocumentTest()
    {
        _options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
        _options.Converters.Add(new NodeMappingOutputJsonConverter());
    }

    [Fact]
    public void Mapping_Serialization_Ok()
    {
        NodeMapping m = new()
        {
            Id = 1,
            Name = "alpha",
            Output = new NodeMappingOutput
            {
                Metadata = new Dictionary<string, string> { ["x"] = "y" }
            }
        };
        string json = JsonSerializer.Serialize(m, _options);
        NodeMapping? m2 = JsonSerializer.Deserialize<NodeMapping>(json, _options);

        Assert.NotNull(m2);
        Assert.Equal(m.Name, m2.Name);
        Assert.Equal(m.Output.Metadata.Count, m2.Output!.Metadata.Count);
    }

    [Fact]
    public void Document_Serialization_Ok()
    {
        NodeMappingDocument doc = new();
        doc.DocumentMappings.Add(new NodeMapping { Name = "sample" });

        string json = JsonSerializer.Serialize(doc, _options);
        NodeMappingDocument? doc2 =
            JsonSerializer.Deserialize<NodeMappingDocument>(json, _options);

        Assert.NotNull(doc2);
        Assert.Equal(doc.DocumentMappings.Count, doc2.DocumentMappings.Count);
        Assert.Equal(doc.DocumentMappings[0].Name, doc2.DocumentMappings[0].Name);
    }

    [Fact]
    public void GetMappings_Depth2_Ok()
    {
        NodeMappingDocument? doc = JsonSerializer.Deserialize<NodeMappingDocument>
            (TestHelper.LoadResourceText("MappingsDocDepth3.json"), _options);

        Assert.NotNull(doc);
        List<NodeMapping> mappings = doc.GetMappings().ToList();
        Assert.Equal(2, mappings.Count);

        // work
        NodeMapping? mapping = mappings.Find(m => m.Name == "work");
        Assert.NotNull(mapping);
        Assert.Empty(mapping.Children);

        // work chronotopes
        mapping = mappings.Find(m => m.Name == "work chronotopes");
        Assert.NotNull(mapping);

        Assert.Single(mapping.Children);
        Assert.Equal("work chronotopes/chronotopes", mapping.Children[0].Name);

        NodeMapping mappingCC = mapping.Children[0];
        Assert.Equal(3, mappingCC.Children.Count);
        Assert.Equal("work chronotopes/chronotopes/place",
            mappingCC.Children[0].Name);
        Assert.True(mappingCC.Children[0].Source?.Length > 0);

        Assert.Equal("work chronotopes/chronotopes/date",
            mappingCC.Children[1].Name);
        Assert.True(mappingCC.Children[1].Source?.Length > 0);

        Assert.Equal("work/assertion", mappingCC.Children[2].Name);
        Assert.True(mappingCC.Children[2].Source?.Length > 0);
    }
}
