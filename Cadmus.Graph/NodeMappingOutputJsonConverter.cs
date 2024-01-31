using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cadmus.Graph;

/// <summary>
/// JSON converter for <see cref="NodeMappingOutput"/>. This is used
/// to serialize and deserialize mappings using a more human-readable format.
/// <list type="bullet">
/// <item>
/// <term>nodes</term>
/// <description>each node is serialized as the property of a <c>nodes</c>
/// object, having its name equal to the node's key, with string value equal
/// to <c>uri label [tag]</c>, where only <c>uri</c> is required.</description>
/// </item>
/// <item>
/// <term>triples</term>
/// <description>each triple is serialized as a string item of a <c>triples</c>
/// array, with format <c>S P O</c> or <c>S P "literal"</c>.</description>
/// </item>
/// <item>
/// <term>metadata</term>
/// <description>each metadatum is serialized as a property of a <c>metadata</c>
/// object, having its  name equal to the metadatum key, with string value
/// equal to the metadatum value.</description>
/// </item>
/// </list>
/// </summary>
public class NodeMappingOutputJsonConverter : JsonConverter<NodeMappingOutput>
{
    private static void ReadNodes(ref Utf8JsonReader reader,
        NodeMappingOutput output)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected object for output.nodes");

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string? key = reader.GetString();
            if (reader.TokenType != JsonTokenType.PropertyName || key == null)
                throw new JsonException("Expected property for output.nodes object");

            reader.Read();
            string nodeText = reader.GetString()
                ?? throw new JsonException(
                    $"Expected string value after output.nodes['{key}']");

            MappedNode? node = MappedNode.Parse(nodeText)
                ?? throw new JsonException("Invalid node: " + nodeText);
            output.Nodes[key] = node;
            reader.Read();
        }
    }

    private static void ReadTriples(ref Utf8JsonReader reader,
        NodeMappingOutput output)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array for output.triples");

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            string? tripleText = reader.GetString() ??
                throw new JsonException("Expected string item in output.triples array");
            output.Triples.Add(MappedTriple.Parse(tripleText)
                ?? throw new JsonException("Invalid triple: " + tripleText));
            reader.Read();
        }
    }

    private static void ReadMetadata(ref Utf8JsonReader reader,
        NodeMappingOutput output)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected object for output.metadata");

        reader.Read();
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            string? name = reader.GetString();
            if (reader.TokenType != JsonTokenType.PropertyName || name == null)
                throw new JsonException("Expected property for output.metadata object");

            reader.Read();
            output.Metadata[name] = reader.GetString()
                ?? throw new JsonException(
                    $"Expected string value after output.metadata['{name}']");
            reader.Read();
        }
    }

    /// <summary>
    /// Read the object.
    /// </summary>
    /// <param name="reader">Reader.</param>
    /// <param name="typeToConvert">Type to convert.</param>
    /// <param name="options">Options.</param>
    /// <returns>Object read or null.</returns>
    /// <exception cref="JsonException">error</exception>
    public override NodeMappingOutput? Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected output object");

        NodeMappingOutput output = new();

        // output.nodes and/or triples
        reader.Read();
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                switch (reader.GetString())
                {
                    case "nodes":
                        reader.Read();
                        ReadNodes(ref reader, output);
                        break;
                    case "triples":
                        reader.Read();
                        ReadTriples(ref reader, output);
                        break;
                    case "metadata":
                        reader.Read();
                        ReadMetadata(ref reader, output);
                        break;
                    default:
                        throw new JsonException(
                            "Unexpected property in output object: " +
                            reader.GetString());
                }
            }
            if (!reader.Read()) break;
        }

        return output;
    }

    /// <summary>
    /// Write the object.
    /// </summary>
    /// <param name="writer">Writer.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="options">Options.</param>
    public override void Write(Utf8JsonWriter writer,
        NodeMappingOutput value,
        JsonSerializerOptions options)
    {
        if (value == null) return;

        writer.WriteStartObject();

        // nodes
        if (value.HasNodes)
        {
            writer.WriteStartObject("nodes");
            foreach (var p in value.Nodes)
            {
                // "key": "string-value" with form "uri label [tag]"
                // where only "uri" is required
                writer.WriteString(p.Key, p.Value.ToString());
            }
            writer.WriteEndObject();
        }

        // triples: []
        if (value.HasTriples)
        {
            writer.WriteStartArray("triples");
            foreach (MappedTriple t in value.Triples)
            {
                writer.WriteStringValue(t.ToString());
            }
            writer.WriteEndArray();
        }

        // metadata
        if (value.HasMetadata)
        {
            writer.WriteStartObject("metadata");
            foreach (var p in value.Metadata)
                writer.WriteString(p.Key, p.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }
}
