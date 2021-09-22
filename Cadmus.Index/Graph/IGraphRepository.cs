using Fusi.Tools.Data;

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
        /// <param name="context">A generic context object.</param>
        void BeginTransaction(object context);

        /// <summary>
        /// Commits a write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        void CommitTransaction(object context);

        /// <summary>
        /// Rollbacks the write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        void RollbackTransaction(object context);

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
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void AddNode(Node node);

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
        /// Adds the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        void AddProperty(Property property);

        /// <summary>
        /// Deletes the property with the specified ID.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        void DeleteProperty(int id);

        /// <summary>
        /// Gets the specified page of restrictions.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>Page.</returns>
        DataPage<PropertyRestrictionResult> GetRestrictions(
            PropertyRestrictionFilter filter);

        /// <summary>
        /// Gets the restriction with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The restriction or null if not found.</returns>
        PropertyRestrictionResult GetRestriction(int id);

        /// <summary>
        /// Adds the specified property restriction.
        /// </summary>
        /// <param name="restriction">The restriction.</param>
        void AddRestriction(PropertyRestriction restriction);

        /// <summary>
        /// Deletes the restriction with the specified ID.
        /// </summary>
        /// <param name="id">The restriction identifier.</param>
        void DeleteRestriction(int id);

        /// <summary>
        /// Gets the specified page of node mappings.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>The page.</returns>
        DataPage<NodeMapping> GetNodeMappings(NodeMappingFilter filter);

        /// <summary>
        /// Gets the node mapping witht the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The mapping or null if not found.</returns>
        NodeMapping GetNodeMapping(int id);

        /// <summary>
        /// Adds the specified node mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        void AddNodeMapping(NodeMapping mapping);

        /// <summary>
        /// Deletes the specified node mapping.
        /// </summary>
        /// <param name="id">The mapping identifier.</param>
        void DeleteMapping(int id);
    }
}
