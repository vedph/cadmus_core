using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// Explanation of a graph update.
/// </summary>
public sealed class GraphUpdaterExplanation
{
    /// <summary>
    /// Gets or sets the filter set by the object adapter used for the updater.
    /// </summary>
    public RunNodeMappingFilter Filter { get; set; }

    /// <summary>
    /// Gets the mappings used in the update, in their order of application.
    /// </summary>
    public IList<NodeMapping> Mappings { get; }

    /// <summary>
    /// Gets the metadata for the update.
    /// </summary>
    public IDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Gets or sets the graph set produced by the mappings.
    /// </summary>
    public GraphSet Set { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphUpdaterExplanation"/>
    /// class.
    /// </summary>
    public GraphUpdaterExplanation()
    {
        Filter = new RunNodeMappingFilter();
        Mappings = new List<NodeMapping>();
        Metadata = new Dictionary<string, object>();
        Set = new();
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"Mappings={Mappings.Count}, Metadata={Metadata.Count}";
    }
}
