using Xunit;
using System.Text.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
// using DevLab.JmesPath;

namespace Cadmus.Graph.Test;

public sealed class JsonNodeMapperTest
{
    //private readonly string _json =
    //    "{ \"id\": " +
    //    "\"colors\", \"entries\": " +
    //    "[ { \"id\": \"r\", \"value\": \"red\" }, " +
    //    "{ \"id\": \"g\", \"value\": \"green\" }, " +
    //    "{ \"id\": \"b\", \"value\": \"blue\" } ], " +
    //    "\"size\": { \"w\": 21, \"h\": 29.7 } } ";

    private static IList<NodeMapping> LoadMappings(string name)
    {
        using StreamReader reader = new(TestHelper.GetResourceStream(name),
            Encoding.UTF8);

        JsonSerializerOptions options = new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new NodeMappingOutputJsonConverter());

        return JsonSerializer.Deserialize<IList<NodeMapping>>(reader.ReadToEnd(),
            options) ?? Array.Empty<NodeMapping>();
    }

    private static void ResetMapperMetadata(INodeMapper mapper)
    {
        mapper.Data.Clear();

        // mock metadata from item
        mapper.Data["item-id"] = "12345678-AAAA-BBBB-CCCC-123456789ABC";
        mapper.Data["item-uri"] = "x:items/my-item";
        mapper.Data["item-label"] = "Petrarch";
        mapper.Data["group-id"] = "group";
        mapper.Data["facet-id"] = "facet";
        mapper.Data["flags"] = "3";
        // mock metada from part
        mapper.Data["part-id"] = "12345678-DDDD-EEEEE-FFFF-123456789ABC";
    }

    [Fact]
    public void Map_Birth()
    {
        // @@
        //JmesPath jmes = new();
        //string r = jmes.Transform(_json, "id");
        //r = jmes.Transform(_json, "entries");
        //r = jmes.Transform(_json, "entries[0].id");
        //r = jmes.Transform(_json, "size");
        //r = jmes.Transform(_json, "size.h");
        //r = jmes.Transform(_json, "x");
        // @@

        NodeMapping mapping = LoadMappings("Mappings.json")
            .First(m => m.Name == "birth event");
        GraphSet set = new();

        JsonNodeMapper mapper = new();
        ResetMapperMetadata(mapper);
        string json = TestHelper.LoadResourceText("Events.json");
        mapper.Map(json, mapping, set);

        // TODO add assertions like:
        Assert.Equal(5, set.Nodes.Count);
        Assert.Equal(9, set.Triples.Count);

        //JmesPath jmes = new();
        //string? x = jmes.Transform(_json, "id");
        //JsonDocument doc = JsonDocument.Parse(x);
    }

    [Fact]
    public void Map_Work()
    {
        NodeMappingOutput output = new();
        output.Nodes["work"] = new()
        {
            Uid = "itn:works/{$item-id}/{@value}",
            Label = "itn:works/{@value}"
        };
        output.Triples.Add(new MappedTriple
        {
            S = "{?work}",
            P = "rdf:type",
            O = "itn:works/{@value}"
        });
        NodeMapping mapping = new()
        {
            Name = "work",
            SourceType = 2,
            FacetFilter = "work",
            PartTypeFilter = "it.vedph.metadata",
            Source = "metadata[?name=='eid']",
            Sid = "{$part-id}/{@[0].value}",
            Output = output
        };
        GraphSet set = new();
        JsonNodeMapper mapper = new();
        ResetMapperMetadata(mapper);
        const string json = "{\"metadata\": [{\"name\": \"eid\", \"value\":\"x\"}]}";

        mapper.Map(json, mapping, set);

        Assert.Single(set.Nodes);
        Assert.Single(set.Triples);
    }
}