using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace Cadmus.Graph.Test;

public sealed class NodeMappingOutputJsonConverterTest
{
    private readonly JsonSerializerOptions _options;

    public NodeMappingOutputJsonConverterTest()
    {
        _options = new()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // avoid escaping of non-ASCII chars
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _options.Converters.Add(new NodeMappingOutputJsonConverter());
    }

    [Fact]
    public void Serialize_Roundtrip_Ok()
    {
        NodeMapping mapping = new()
        {
            Id = 1,
            Name = "test",
            Description = "A test",
            FacetFilter = "work",
            PartTypeFilter = "it.vedph.metadata",
            Sid = "{$part-id}",
            Output = new NodeMappingOutput
            {
                Metadata = new  Dictionary<string, string>
                {
                    ["a"] = "alpha",
                    ["b"] = "beta"
                },
                Nodes = new Dictionary<string, MappedNode>
                {
                    ["x"] = new MappedNode
                    {
                        Uid = "http://example.com/x",
                        Label = "x",
                    },
                    ["y"] = new MappedNode
                    {
                        Uid = "http://example.com/y",
                        Label = "y",
                    }
                },
                Triples = new List<MappedTriple>
                {
                    new MappedTriple
                    {
                        S = "http://example.com/x",
                        P = "a",
                        O = "foaf:person"
                    },
                    new MappedTriple
                    {
                        S = "http://example.com/y",
                        P = "a",
                        O = "foaf:person"
                    }
                }
            }
        };
        string json = JsonSerializer.Serialize(mapping, _options);

        NodeMapping? mapping2 = JsonSerializer.Deserialize<NodeMapping>(json, _options);
        Assert.NotNull(mapping2);
        // assert that each property has the same value
        Assert.Equal(mapping.Id, mapping2.Id);
        Assert.Equal(mapping.Name, mapping2.Name);
        Assert.Equal(mapping.Description, mapping2.Description);
        Assert.Equal(mapping.FacetFilter, mapping2.FacetFilter);
        Assert.Equal(mapping.PartTypeFilter, mapping2.PartTypeFilter);
        Assert.Equal(mapping.Sid, mapping2.Sid);
        Assert.Equal(mapping.Output.Metadata.Count, mapping2.Output!.Metadata.Count);
        Assert.Equal(mapping.Output.Nodes.Count, mapping2.Output!.Nodes.Count);
        Assert.Equal(mapping.Output.Triples.Count, mapping2.Output!.Triples.Count);
    }
}
