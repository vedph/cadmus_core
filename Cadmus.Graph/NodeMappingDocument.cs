using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Graph;

/// <summary>
/// A document including node mappings. This is a utility class used to
/// load a set of mappings from a JSON file, while avoiding repetitions in it
/// via named references. Typically, a document is loaded from a JSON stream
/// or string by just deserializing it into this class. Then you can use
/// <see cref="GetMappings"/> to get all the mappings in the document properly
/// dereferenced.
/// </summary>
public class NodeMappingDocument
{
    /// <summary>
    /// Gets or sets the named mappings. These mappings are reused across
    /// the document via their names.
    /// </summary>
    public Dictionary<string, NodeMapping> NamedMappings { get; set; }

    /// <summary>
    /// Gets or sets the mappings in the document. These can be either
    /// references to named mappings, or inline mappings.
    /// </summary>
    public List<NodeMapping> DocumentMappings { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeMappingDocument"/> class.
    /// </summary>
    public NodeMappingDocument()
    {
        NamedMappings = new();
        DocumentMappings = new();
    }

    private void ResolveNamedMappings(NodeMapping mapping)
    {
        if (!mapping.HasChildren) return;

        for (int i = 0; i < mapping.Children.Count; i++)
        {
            if (mapping.Children[i].Name != null &&
                NamedMappings.TryGetValue(
                    mapping.Children[i].Name!, out NodeMapping? named))
            {
                mapping.Children[i] = named.Clone();
            }
            ResolveNamedMappings(mapping.Children[i]);
        }
    }

    /// <summary>
    /// Gets all the document's mappings, dereferencing those which are not
    /// inlined.
    /// </summary>
    /// <returns>Mappings.</returns>
    public IEnumerable<NodeMapping> GetMappings()
    {
        foreach (NodeMapping mapping in DocumentMappings)
        {
            NodeMapping resolved = (NamedMappings.TryGetValue(
                mapping.Name!, out NodeMapping? value) ? value : mapping).Clone();

            if (resolved.HasChildren) ResolveNamedMappings(resolved);

            yield return resolved;
        }
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"N: {NamedMappings.Count} - D: {DocumentMappings.Count}";
    }
}
