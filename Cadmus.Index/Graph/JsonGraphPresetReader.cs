using Cadmus.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Cadmus.Index.Graph
{
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
            _options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
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
                    _options);
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
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return Read<UriNode>(stream);
        }

        /// <summary>
        /// Reads the preset node mappings from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="idOffset">The offset to add to each node mapping ID
        /// read from the input. This can be used when you have to merge
        /// mappings into an existing database.</param>
        /// <returns>
        /// Node mappings.
        /// </returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public IEnumerable<NodeMapping> ReadMappings(Stream stream,
            int idOffset = 0)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            foreach (NodeMapping mapping in Read<NodeMapping>(stream))
            {
                if (idOffset != 0)
                {
                    mapping.Id += idOffset;
                    mapping.ParentId += idOffset;
                }
                yield return mapping;
            }
        }

        /// <summary>
        /// Reads the class thesauri from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Thesauri.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public IEnumerable<Thesaurus> ReadThesauri(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            return Read<Thesaurus>(stream);
        }
    }
}
