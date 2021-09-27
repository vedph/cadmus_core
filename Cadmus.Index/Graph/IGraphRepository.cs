using Cadmus.Core;
using Fusi.Tools;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The nodes graph repository in the index.
    /// </summary>
    public interface IGraphRepository
    {
        /// <summary>
        /// Begins a write transaction. All the write methods of this repository
        /// get connected to this transaction until it is either committed
        /// or rejected.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        void BeginTransaction(object context = null);

        /// <summary>
        /// Commits a write transaction.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        void CommitTransaction(object context = null);

        /// <summary>
        /// Rollbacks the write transaction.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        void RollbackTransaction(object context = null);

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
        string LookupNamespace(string prefix);

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
        /// Adds the specified UID, eventually completing it with a suffix.
        /// </summary>
        /// <param name="uid">The UID as calculated from its source, without any
        /// suffix.</param>
        /// <param name="sid">The SID identifying the source for this UID.</param>
        /// <returns>The UID, eventually suffixed.</returns>
        string AddUid(string uid, string sid);

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
        string LookupUri(int id);

        /// <summary>
        /// Gets the requested page of nodes.
        /// </summary>
        /// <param name="filter">The nodes filter.</param>
        /// <returns>The page.</returns>
        DataPage<NodeResult> GetNodes(NodeFilter filter);

        /// <summary>
        /// Gets the node with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The node or null if not found.</returns>
        NodeResult GetNode(int id);

        /// <summary>
        /// Gets the node by its URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The node or null if not found.</returns>
        NodeResult GetNodeByUri(string uri);

        /// <summary>
        /// Adds or updates the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="noUpdate">True to avoid updating an existing node.
        /// When this is true, the node is added when not existing; when
        /// existing, nothing is done.</param>
        void AddNode(Node node, bool noUpdate = false);

        /// <summary>
        /// Deletes the node with the specified ID.
        /// </summary>
        /// <param name="id">The node identifier.</param>
        void DeleteNode(int id);

        /// <summary>
        /// Gets the specified page of properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page.</returns>
        DataPage<PropertyResult> GetProperties(PropertyFilter filter);

        /// <summary>
        /// Gets the property with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The property or null if not found.</returns>
        PropertyResult GetProperty(int id);

        /// <summary>
        /// Gets the property by its URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The property or null if not found.</returns>
        PropertyResult GetPropertyByUri(string uri);

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
        /// Gets the specified page of node mappings.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>The page.</returns>
        DataPage<NodeMapping> GetMappings(NodeMappingFilter filter);

        /// <summary>
        /// Gets the node mapping witht the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The mapping or null if not found.</returns>
        NodeMapping GetMapping(int id);

        /// <summary>
        /// Adds or updates the specified node mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        void AddMapping(NodeMapping mapping);

        /// <summary>
        /// Deletes the specified node mapping.
        /// </summary>
        /// <param name="id">The mapping identifier.</param>
        void DeleteMapping(int id);

        /// <summary>
        /// Finds all the mappings applicable to the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="parentId">The parent mapping identifier, or 0 to
        /// get root mappings.</param>
        /// <returns>Mappings.</returns>
        IList<NodeMapping> FindMappingsFor(IItem item, int parentId = 0);

        /// <summary>
        /// Finds all the mappings applicable to the specified part's pin.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part.</param>
        /// <param name="pin">The pin name.</param>
        /// <param name="parentId">The parent mapping identifier, or 0 to
        /// get root mappings.</param>
        /// <returns>Mappings.</returns>
        IList<NodeMapping> FindMappingsFor(IItem item, IPart part, string pin,
            int parentId = 0);

        /// <summary>
        /// Gets the specified page of triples.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page.</returns>
        DataPage<TripleResult> GetTriples(TripleFilter filter);

        /// <summary>
        /// Gets the triple with the specified ID.
        /// </summary>
        /// <param name="id">The triple's ID.</param>
        TripleResult GetTriple(int id);

        /// <summary>
        /// Adds or updates the specified triple.
        /// </summary>
        /// <param name="triple">The triple.</param>
        void AddTriple(Triple triple);

        /// <summary>
        /// Deletes the triple with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void DeleteTriple(int id);

        /// <summary>
        /// Updates the classes for all the nodes belonging to any class.
        /// </summary>
        /// <param name="cancel">The cancel.</param>
        /// <param name="progress">The progress.</param>
        Task UpdateNodeClassesAsync(CancellationToken cancel,
            IProgress<ProgressReport> progress = null);

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
    }
}
