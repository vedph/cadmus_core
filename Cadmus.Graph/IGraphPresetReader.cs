using Cadmus.Core.Config;
using System.Collections.Generic;
using System.IO;

namespace Cadmus.Graph;

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
    /// Reads the preset triples from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Triples.</returns>
    IEnumerable<UriTriple> ReadTriples(Stream stream);

    /// <summary>
    /// Reads the preset thesauri from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Thesauri.</returns>
    IEnumerable<Thesaurus> ReadThesauri(Stream stream);

    /// <summary>
    /// Loads node mappings from the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>Node mappings.</returns>
    IList<NodeMapping> LoadMappings(Stream stream);
}
