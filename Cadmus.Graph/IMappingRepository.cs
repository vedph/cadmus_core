using Fusi.Tools.Data;

namespace Cadmus.Graph;

/// <summary>
/// Node mapping repository.
/// </summary>
public interface IMappingRepository
{
    /// <summary>
    /// Gets the specified page of mappings.
    /// </summary>
    /// <param name="filter">The filter. Set page size=0 to get all
    /// the mappings at once.</param>
    /// <param name="descendants">True to load the descendants of each mapping.
    /// </param>
    /// <returns>Page.</returns>
    DataPage<NodeMapping> GetMappings(NodeMappingFilter filter, bool descendants);

    /// <summary>
    /// Gets the mapping with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Mapping or null if not found.</returns>
    NodeMapping? GetMapping(int id);

    /// <summary>
    /// Adds or updates the specified mapping by its name.
    /// </summary>
    /// <param name="mapping">The mapping.</param>
    /// <returns>The ID of the mapping.</returns>
    int AddMapping(NodeMapping mapping);

    /// <summary>
    /// Deletes the mapping with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void DeleteMapping(int id);

    /// <summary>
    /// Exports mappings into JSON code.
    /// </summary>
    /// <returns>JSON code.</returns>
    string Export();

    /// <summary>
    /// Imports mappings from the specified JSON code.
    /// </summary>
    /// <param name="json">The JSON code.</param>
    /// <returns>Count of imported mappings.</returns>
    int Import(string json);
}
