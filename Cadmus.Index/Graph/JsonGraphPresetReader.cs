using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// JSON-based graph preset reader. This reads nodes or node mappings
    /// from JSON files, each being an array of node or node mapping objects.
    /// Deserialization is not case sensitive.
    /// </summary>
    /// <seealso cref="IGraphPresetReader" />
    public sealed class JsonGraphPresetReader : IGraphPresetReader
    {
        private async Task<IList<T>> ReadAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T[]>(stream,
                new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                });
        }

        /// <summary>
        /// Reads the preset nodes from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Nodes.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public async Task<IList<Node>> ReadNodesAsync(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            return await ReadAsync<Node>(stream);
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
        public async Task<IList<NodeMapping>> ReadMappingsAsync(Stream stream,
            int idOffset = 0)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            IList<NodeMapping> mappings = await ReadAsync<NodeMapping>(stream);
            if (idOffset != 0)
            {
                foreach (NodeMapping mapping in mappings)
                {
                    mapping.Id += idOffset;
                    mapping.ParentId += idOffset;
                }
            }
            return mappings;
        }
    }
}
