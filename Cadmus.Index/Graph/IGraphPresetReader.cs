using Cadmus.Core.Config;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Graph preset data reader. This is used to read a set of nodes and
    /// node mappings from some source, typically to inject them into an
    /// existing database via <see cref="IGraphRepository"/>.
    /// </summary>
    public interface IGraphPresetReader
    {
        /// <summary>
        /// Reads the preset nodes from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Nodes.</returns>
        IEnumerable<UriNode> ReadNodes(Stream stream);

        /// <summary>
        /// Reads the preset node mappings from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="idOffset">The offset to add to each node mapping ID
        /// read from the input. This can be used when you have to merge
        /// mappings into an existing database.</param>
        /// <returns>Node mappings.</returns>
        IEnumerable<NodeMapping> ReadMappings(Stream stream,
            int idOffset = 0);

        /// <summary>
        /// Reads the class thesauri from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Thesauri.</returns>
        IEnumerable<Thesaurus> ReadThesauri(Stream stream);
    }
}
