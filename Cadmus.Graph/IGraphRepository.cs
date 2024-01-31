using Cadmus.Core.Config;
using Fusi.Tools;
using Fusi.Tools.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Graph;

/// <summary>
/// The nodes graph repository in the index.
/// </summary>
public interface IGraphRepository : IUidBuilder, IMappingRepository
{
    /// <summary>
    /// Creates the target store if it does not exist.
    /// </summary>
    /// <param name="payload">Optional payload data to be used in creating
    /// the store. For instance, an SQL-based store could provide SQL code
    /// for seeding preset data.</param>
    /// <returns>True if created, false if already existing.</returns>
    bool CreateStore(object? payload = null);

    /// <summary>
    /// Gets or sets the optional cache to use for mappings. This improves
    /// performance when fetching mappings from the database. All the
    /// mappings are stored with key <c>nm-</c> + the mapping's ID.
    /// Avoid using this if editing mappings.
    /// </summary>
    public IMemoryCache? Cache { get; set; }

    /// <summary>
    /// Gets the specified page of namespaces with their prefixes.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The page.</returns>
    DataPage<NamespaceEntry> GetNamespaces(NamespaceFilter filter);

    /// <summary>
    /// Looks up the namespace from its prefix.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    /// <returns>The namespace, or null if not found.</returns>
    string? LookupNamespace(string prefix);

    /// <summary>
    /// Adds or updates the specified namespace prefix.
    /// </summary>
    /// <param name="prefix">The namespace prefix.</param>
    /// <param name="uri">The namespace URI corresponding to
    /// <paramref name="prefix"/>.</param>
    void AddNamespace(string prefix, string uri);

    /// <summary>
    /// Deletes a namespace by prefix.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    void DeleteNamespaceByPrefix(string prefix);

    /// <summary>
    /// Deletes the specified namespace with all its prefixes.
    /// </summary>
    /// <param name="uri">The namespace URI.</param>
    void DeleteNamespaceByUri(string uri);

    /// <summary>
    /// Adds the specified URI to the mapped URIs set.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>ID assigned to the URI.</returns>
    int AddUri(string uri);

    /// <summary>
    /// Lookups the URI from its numeric ID.
    /// </summary>
    /// <param name="id">The numeric ID for the URI.</param>
    /// <returns>The URI, or null if not found.</returns>
    string? LookupUri(int id);

    /// <summary>
    /// Lookups the numeric ID from its URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>The ID, or 0 if not found.</returns>
    int LookupId(string uri);

    /// <summary>
    /// Gets the requested page of nodes.
    /// </summary>
    /// <param name="filter">The nodes filter.</param>
    /// <returns>The page.</returns>
    DataPage<UriNode> GetNodes(NodeFilter filter);

    /// <summary>
    /// Gets all the nodes with the specified IDs.
    /// </summary>
    /// <param name="ids">The nodes IDs.</param>
    /// <returns>List of nodes (or null), one per ID.</returns>
    IList<UriNode?> GetNodes(IList<int> ids);

    /// <summary>
    /// Gets the node with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The node or null if not found.</returns>
    UriNode? GetNode(int id);

    /// <summary>
    /// Gets the node by its URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>The node or null if not found.</returns>
    UriNode? GetNodeByUri(string uri);

    /// <summary>
    /// Adds or updates the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="noUpdate">True to avoid updating an existing node.
    /// When this is true, the node is added when not existing; when
    /// existing, nothing is done.</param>
    void AddNode(Node node, bool noUpdate = false);

    /// <summary>
    /// Bulk imports the specified nodes.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    void ImportNodes(IEnumerable<UriNode> nodes);

    /// <summary>
    /// Deletes the node with the specified ID.
    /// </summary>
    /// <param name="id">The node identifier.</param>
    void DeleteNode(int id);

    /// <summary>
    /// Gets the nodes included in a triple with the specified predicate ID
    /// and other node ID, either as its subject or as its object.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>Page.</returns>
    DataPage<UriNode> GetLinkedNodes(LinkedNodeFilter filter);

    /// <summary>
    /// Gets the literals included in a triple with the specified subject ID
    /// and predicate ID.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>Page.</returns>
    public DataPage<UriTriple> GetLinkedLiterals(LinkedLiteralFilter filter);

    /// <summary>
    /// Gets the specified page of properties.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>Page.</returns>
    DataPage<UriProperty> GetProperties(PropertyFilter filter);

    /// <summary>
    /// Gets the property with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The property or null if not found.</returns>
    UriProperty? GetProperty(int id);

    /// <summary>
    /// Gets the property by its URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <returns>The property or null if not found.</returns>
    UriProperty? GetPropertyByUri(string uri);

    /// <summary>
    /// Adds or updates the specified property.
    /// </summary>
    /// <param name="property">The property.</param>
    void AddProperty(Property property);

    /// <summary>
    /// Deletes the property with the specified ID.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    void DeleteProperty(int id);

    /// <summary>
    /// Finds all the applicable mappings.
    /// </summary>
    /// <param name="filter">The filter to match.</param>
    /// <returns>List of mappings.</returns>
    IList<NodeMapping> FindMappings(RunNodeMappingFilter filter);

    /// <summary>
    /// Gets the specified page of triples.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>Page.</returns>
    DataPage<UriTriple> GetTriples(TripleFilter filter);

    /// <summary>
    /// Gets the triple with the specified ID.
    /// </summary>
    /// <param name="id">The triple's ID.</param>
    /// <returns>The triple, or null if not found.</returns>
    UriTriple? GetTriple(int id);

    /// <summary>
    /// Adds or updates the specified triple.
    /// </summary>
    /// <param name="triple">The triple.</param>
    void AddTriple(Triple triple);

    /// <summary>
    /// Bulk imports the specified triples.
    /// </summary>
    /// <param name="triples">The triples.</param>
    void ImportTriples(IEnumerable<UriTriple> triples);

    /// <summary>
    /// Deletes the triple with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void DeleteTriple(int id);

    /// <summary>
    /// Gets the specified page of triples variously filtered, and grouped
    /// by their predicate.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="sort">The sort order: any combination of <c>c</c>=by
    /// count, ascending; <c>C</c>=by count, descending; <c>u</c>=by URI,
    /// ascending; <c>U</c>=by URI, descending.</param>
    /// <returns>Page.</returns>
    /// <exception cref="ArgumentNullException">filter or sort</exception>
    DataPage<TripleGroup> GetTripleGroups(TripleFilter filter,
        string sort = "Cu");

    /// <summary>
    /// Adds the specified thesaurus as a set of class nodes.
    /// </summary>
    /// <param name="thesaurus">The thesaurus.</param>
    /// <param name="includeRoot">If set to <c>true</c>, include also the
    /// root entry in a hierarchical thesaurus.</param>
    /// <param name="prefix">The optional prefix to prepend to each
    /// thesaurus entry ID.</param>
    void AddThesaurus(Thesaurus thesaurus, bool includeRoot,
        string? prefix = null);

    /// <summary>
    /// Updates the classes for all the nodes belonging to any class.
    /// </summary>
    /// <param name="cancel">The cancel.</param>
    /// <param name="progress">The progress.</param>
    Task UpdateNodeClassesAsync(CancellationToken cancel,
        IProgress<ProgressReport>? progress = null);

    /// <summary>
    /// Gets the set of graph's nodes and triples whose SID starts with
    /// the specified GUID. This identifies all the nodes and triples
    /// generated from a single source item or part.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    /// <returns>The set.</returns>
    GraphSet GetGraphSet(string sourceId);

    /// <summary>
    /// Deletes the set of graph's nodes and triples whose SID starts with
    /// the specified GUID. This identifies all the nodes and triples
    /// generated from a single source item or part.
    /// </summary>
    /// <param name="sourceId">The source identifier.</param>
    void DeleteGraphSet(string sourceId);

    /// <summary>
    /// Updates the graph with the specified nodes and triples.
    /// </summary>
    /// <param name="set">The new set of nodes and triples.</param>
    void UpdateGraph(GraphSet set);
}
