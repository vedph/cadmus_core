using Cadmus.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Cadmus.Graph;

/// <summary>
/// JSON-based graph preset reader. This reads nodes, node mappings or
/// thesauri each from a JSON file, including an array of nodes or node
/// mappings, or thesauri.
/// Deserialization is not case sensitive.
/// </summary>
/// <seealso cref="IGraphPresetReader" />
public sealed class JsonGraphPresetReader : IGraphPresetReader
{
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonGraphPresetReader"/>
    /// class.
    /// </summary>
    public JsonGraphPresetReader()
    {
        _options = new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        _options.Converters.Add(new NodeMappingOutputJsonConverter());
    }

    private IEnumerable<T> Read<T>(Stream stream)
    {
        JsonDocument doc = JsonDocument.Parse(stream, new JsonDocumentOptions
        {
            AllowTrailingCommas = true
        });
        foreach (JsonElement elem in doc.RootElement.EnumerateArray())
        {
            yield return JsonSerializer.Deserialize<T>(elem.GetRawText(),
                _options)!;
        }
    }

    /// <summary>
    /// Reads the preset nodes from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Nodes.</returns>
    /// <exception cref="ArgumentNullException">stream</exception>
    public IEnumerable<UriNode> ReadNodes(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return Read<UriNode>(stream);
    }

    /// <summary>
    /// Reads the preset triples from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Triples.</returns>
    public IEnumerable<UriTriple> ReadTriples(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return Read<UriTriple>(stream);
    }

    /// <summary>
    /// Reads the preset thesauri from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Thesauri.</returns>
    /// <exception cref="ArgumentNullException">stream</exception>
    public IEnumerable<Thesaurus> ReadThesauri(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return Read<Thesaurus>(stream);
    }

    /// <summary>
    /// Loads node mappings from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Node mappings.</returns>
    /// <exception cref="ArgumentNullException">stream</exception>
    public IList<NodeMapping> LoadMappings(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream, Encoding.UTF8);
        return JsonSerializer.Deserialize<IList<NodeMapping>>(
            reader.ReadToEnd(),
            _options) ?? Array.Empty<NodeMapping>();
    }
}
