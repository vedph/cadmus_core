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
        /// Adds the specified namespace.
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
        /// Adds the specified SID lookup entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        void AddSid(SidEntry entry);

        /// <summary>
        /// Deletes the specified SID lookup entry.
        /// </summary>
        /// <param name="id">The identifier (SID).</param>
        void DeleteSid(string id);

        /// <summary>
        /// Gets the suffix for the specified unsuffixed SID.
        /// If no SID exists with the same unsuffixed form, 0 is returned.
        /// Otherwise, the SID gets a suffix and is stored in the lookup set,
        /// while returning its suffix.
        /// </summary>
        /// <param name="unsuffixed">The SID without any suffix.</param>
        /// <returns>Suffix number (0-N).</returns>
        int GetSuffixFor(string unsuffixed);

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
        DataPage<Node> GetNodes(NodeFilter filter);

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
        DataPage<Property> GetProperties(PropertyFilter filter);

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
        DataPage<PropertyRestriction> GetRestrictions(
            PropertyRestrictionFilter filter);

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
