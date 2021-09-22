using Cadmus.Index.Graph;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Cadmus.Index.Sql.Graph
{
    /// <summary>
    /// Base class for SQL-based graph repositories.
    /// </summary>
    public abstract class SqlGraphRepositoryBase : SqlRepositoryBase
    {
        /// <summary>
        /// Gets the currently active transaction if any.
        /// </summary>
        /// <remarks>All the write operations use this transaction, when set.
        /// </remarks>
        protected DbTransaction Transaction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlGraphRepositoryBase"/>
        /// class.
        /// </summary>
        /// <param name="tokenHelper">The token helper.</param>
        protected SqlGraphRepositoryBase(ISqlTokenHelper tokenHelper) :
            base(tokenHelper)
        {
        }

        /// <summary>
        /// Gets the paging SQL for the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>SQL code.</returns>
        protected abstract string GetPagingSql(PagingOptions options);

        /// <summary>
        /// Gets the SQL code for a regular expression clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="pattern">The regular expression pattern.</param>
        protected abstract string GetRegexClauseSql(string fieldName,
            string pattern);

        #region Transaction        
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="context">The context.</param>
        public void BeginTransaction(object context)
        {
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Commits a write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        public void CommitTransaction(object context)
        {
            Transaction?.Commit();
            Transaction = null;
        }

        /// <summary>
        /// Rollbacks the write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        public void RollbackTransaction(object context)
        {
            Transaction?.Rollback();
            Transaction = null;
        }
        #endregion

        #region Namespace Lookup
        /// <summary>
        /// Gets the specified page of namespaces with their prefixes.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<NamespaceEntry> GetNamespaces(NamespaceFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks up the namespace from its prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The namespace, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">prefix</exception>
        public string LookupNamespace(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT uri FROM namespace_lookup WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.String, prefix);
                return cmd.ExecuteScalar() as string;
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes a namespace by prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <exception cref="ArgumentNullException">prefix</exception>
        public void DeleteNamespaceByPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM node_mapping WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.String, prefix);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the specified namespace with all its prefixes.
        /// </summary>
        /// <param name="uri">The namespace URI.</param>
        public void DeleteNamespaceByUri(string uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM namespace WHERE uri=@uri;";
                AddParameter(cmd, "@uri", DbType.String, uri);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region UID Lookup
        /// <summary>
        /// Adds the specified UID, eventually completing it with a suffix.
        /// </summary>
        /// <param name="uid">The UID as calculated from its source, without any
        /// suffix.</param>
        /// <param name="sid">The SID identifying the source for this UID.</param>
        /// <returns>The UID, eventually suffixed.</returns>
        public string AddUid(string uid, string sid)
        {
            if (uid == null) throw new ArgumentNullException(nameof(uid));
            if (sid == null) throw new ArgumentNullException(nameof(sid));

            EnsureConnected();

            try
            {
                // prepare the insertion
                DbCommand cmdIns = GetCommand();
                cmdIns.Transaction = Transaction;
                cmdIns.CommandText =
                    "INSERT INTO uid_lookup(id,sid,unsuffixed,has_suffix) " +
                    "VALUES(@id,@sid,@unsuffixed,@has_suffix);";
                AddParameter(cmdIns, "@sid", DbType.String, sid);
                AddParameter(cmdIns, "@unsuffixed", DbType.String, uid);
                AddParameter(cmdIns, "@has_suffix", DbType.Boolean, false);
                AddParameter(cmdIns, "@id", DbType.Int32, ParameterDirection.Output);

                // check if any unsuffixed UID is already in use
                DbCommand cmdSel = GetCommand();
                cmdSel.CommandText = "SELECT 1 FROM uid_lookup WHERE unsuffixed=@uid;";
                AddParameter(cmdSel, "@uid", DbType.String, uid);
                int? result = cmdSel.ExecuteScalar() as int?;

                // no: just insert the unsuffixed UID
                if (result == null)
                {
                    cmdIns.ExecuteNonQuery();
                    return uid;
                }

                // yes: check if a record with the same UID & SID exists;
                // if so, reuse it; otherwise, add a new suffixed UID
                cmdSel.CommandText = "SELECT id FROM uid_lookup " +
                    "WHERE unsuffixed=@uid AND sid=@sid;";
                AddParameter(cmdSel, "@sid", DbType.String, sid);
                result = cmdSel.ExecuteScalar() as int?;

                // yes: reuse it, nothing gets inserted
                if (result != null) return sid + "#" + result.Value;

                // no: add a new suffix
                cmdIns.Parameters["@has_suffix"].Value = true;
                cmdIns.ExecuteNonQuery();
                int id = (int)cmdIns.Parameters["@id"].Value;
                return uid + "#" + id;
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region URI Lookup
        /// <summary>
        /// Adds the specified URI to the mapped URIs set.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>ID assigned to the URI.</returns>
        /// <exception cref="ArgumentNullException">uri</exception>
        public int AddUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO uri_lookup(id,uri) VALUES(@id,@uri);";
                AddParameter(cmd, "@uri", DbType.String, uri);
                AddParameter(cmd, "@id", DbType.Int32, ParameterDirection.Output);

                cmd.ExecuteNonQuery();
                return (int)cmd.Parameters["@id"].Value;
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Lookups the URI from its numeric ID.
        /// </summary>
        /// <param name="id">The numeric ID for the URI.</param>
        /// <returns>The URI, or null if not found.</returns>
        public string LookupUri(int id)
        {
            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT uri FROM uri_lookup WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                return cmd.ExecuteScalar() as string;
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region Node
        private SqlSelectBuilder GetBuilderFor(NodeFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c");

            builder.AddWhat("node.id, node.is_class, node.label, " +
                "node.source_type, node.sid, ul.uri")
                   .AddWhat("COUNT(id)", slotId: "c")
                   .AddFrom("node", slotId: "*")
                   .AddFrom("INNER JOIN uri_lookup ul ON ul.id=node.id")
                   .AddOrder("label, id");

            // uid
            if (!string.IsNullOrEmpty(filter.Uid))
            {
                builder.AddFrom("INNER JOIN uri_lookup ul ON node.id=ul.id", slotId: "*")
                       .AddWhere("uid LIKE '%@uid%'", slotId: "*")
                       .AddParameter("@uid", DbType.String, filter.Uid, slotId: "*");
            }

            // label
            if (!string.IsNullOrEmpty(filter.Label))
            {
                builder.AddWhere("label LIKE '%@label%'", slotId: "*")
                       .AddParameter("@label", DbType.String, filter.Label, slotId: "*");
            }

            // source type
            if (filter.SourceType != null)
            {
                builder.AddWhere("source_type=@source_type", slotId: "*")
                       .AddParameter("@source_type",
                            DbType.Int32, filter.SourceType.Value, slotId: "*");
            }

            // sid
            if (!string.IsNullOrEmpty(filter.Sid))
            {
                builder.AddWhere(filter.IsSidPrefix ?
                        "sid LIKE '@sid%'" : "sid=@sid", slotId: "*")
                       .AddParameter("@sid", DbType.String, filter.Sid, slotId: "*");
            }

            // linked node ID and role
            if (filter.LinkedNodeId > 0)
            {
                builder.AddParameter("@lnid", DbType.Int32, filter.LinkedNodeId,
                    slotId: "*");

                switch (char.ToUpperInvariant(filter.LinkedNodeRole))
                {
                    case 'S':
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON t.s_id=@lnid AND t.o_id=node.id",
                            slotId: "*");
                        break;
                    case 'O':
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON t.o_id=@lnid AND t.s_id=node.id",
                            slotId: "*");
                        break;
                    default:
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON (t.s_id=@lnid AND t.o_id=node.id) OR " +
                            "(t.o_id=@lnid AND t.s_id=node.id)",
                            slotId: "*");
                        break;
                }
            }

            // class IDs
            if (filter.ClassIds?.Count > 0)
            {
                string ids = string.Join(",",
                    filter.ClassIds.Select(
                        id => SqlHelper.SqlEncode(id, false, true)));

                builder.AddFrom("INNER JOIN node_class nc " +
                    $"ON node.id=nc.node_id AND nc.class_id IN({ids})",
                    slotId: "*");
            }

            // order and limit
            builder.AddLimit(GetPagingSql(filter));

            return builder;
        }

        /// <summary>
        /// Gets the requested page of nodes.
        /// </summary>
        /// <param name="filter">The nodes filter.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<NodeResult> GetNodes(NodeFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureConnected();

            try
            {
                SqlSelectBuilder builder = GetBuilderFor(filter);

                // get count and ret if no result
                DbCommand cmd = GetCommand();
                cmd.CommandText = builder.Build("c");
                builder.AddParametersTo(cmd, "c");

                long? count = cmd.ExecuteScalar() as long?;
                if (count == null || count == 0)
                {
                    return new DataPage<NodeResult>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<NodeResult>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<NodeResult> nodes = new List<NodeResult>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nodes.Add(new NodeResult
                        {
                            Id = reader.GetInt32(0),
                            IsClass = reader.GetBoolean(1),
                            Label = reader.GetValue<string>(2),
                            SourceType = (NodeSourceType)reader.GetInt32(3),
                            Sid = reader.GetValue<string>(4),
                            Uri = reader.GetString(5)
                        });
                    }
                }
                return new DataPage<NodeResult>(filter.PageNumber, filter.PageSize,
                    (int)count, nodes);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the node with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The node or null if not found.</returns>
        public NodeResult GetNode(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT n.is_class, n.label, " +
                    "n.source_type, n.sid, ul.uri FROM node n\n" +
                    "INNER JOIN uri_lookup ul ON n.id=ul.id WHERE n.id=@id";
                AddParameter(cmd, "@id", DbType.Int32, id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new NodeResult
                    {
                        Id = id,
                        IsClass = reader.GetBoolean(0),
                        Label = reader.GetValue<string>(1),
                        SourceType = (NodeSourceType)reader.GetInt32(2),
                        Sid = reader.GetValue<string>(3),
                        Uri = reader.GetString(4)
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the node by its URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The node or null if not found.</returns>
        /// <exception cref="ArgumentNullException">uri</exception>
        public NodeResult GetNodeByUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT n.id, n.is_class, n.label, " +
                    "n.source_type, n.sid FROM node n\n" +
                    "INNER JOIN uri_lookup ul ON n.id=ul.id WHERE ul.uri=@uri";
                AddParameter(cmd, "@uri", DbType.String, uri);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new NodeResult
                    {
                        Id = reader.GetInt32(0),
                        IsClass = reader.GetBoolean(1),
                        Label = reader.GetValue<string>(2),
                        SourceType = (NodeSourceType)reader.GetInt32(3),
                        Sid = reader.GetValue<string>(4),
                        Uri = uri
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="ArgumentNullException">node</exception>
        public void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO node(is_class,label,source_type,sid) " +
                    "VALUES(@is_class,@label,@source_type,@sid);";
                AddParameter(cmd, "@is_class", DbType.Boolean, node.IsClass);
                AddParameter(cmd, "@label", DbType.String, node.Label);
                AddParameter(cmd, "@source_type", DbType.Int32, node.SourceType);
                AddParameter(cmd, "@sid", DbType.String, node.Sid);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the node with the specified ID.
        /// </summary>
        /// <param name="id">The node identifier.</param>
        public void DeleteNode(int id)
        {
            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM node WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region Property
        private SqlSelectBuilder GetBuilderFor(PropertyFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c");

            builder.AddWhat("id, data_type, lit_editor, description, ul.uri")
                   .AddWhat("COUNT(id)", slotId: "c")
                   .AddFrom("property", slotId: "*")
                   .AddFrom("INNER JOIN uri_lookup ul ON ul.id=node.id")
                   .AddOrder("ul.uri");

            if (!string.IsNullOrEmpty(filter.DataType))
            {
                builder.AddWhere("data_type=@data_type", slotId: "*")
                       .AddParameter("@data_type", DbType.String,
                            filter.DataType, slotId: "*");
            }

            if (!string.IsNullOrEmpty(filter.LiteralEditor))
            {
                builder.AddWhere("lit_editor=@lit_editor", slotId: "*")
                       .AddParameter("@lit_editor", DbType.String,
                            filter.LiteralEditor, slotId: "*");
            }

            // limit
            builder.AddLimit(GetPagingSql(filter));

            return builder;
        }

        /// <summary>
        /// Gets the specified page of properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page.</returns>
        public DataPage<PropertyResult> GetProperties(PropertyFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureConnected();

            try
            {
                SqlSelectBuilder builder = GetBuilderFor(filter);

                // get count and ret if no result
                DbCommand cmd = GetCommand();
                cmd.CommandText = builder.Build("c");
                builder.AddParametersTo(cmd, "c");

                long? count = cmd.ExecuteScalar() as long?;
                if (count == null || count == 0)
                {
                    return new DataPage<PropertyResult>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<PropertyResult>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<PropertyResult> props = new List<PropertyResult>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        props.Add(new PropertyResult
                        {
                            Id = reader.GetInt32(0),
                            DataType = reader.GetValue<string>(1),
                            LiteralEditor = reader.GetValue<string>(2),
                            Description = reader.GetValue<string>(3),
                            Uri = reader.GetString(4)
                        });
                    }
                }
                return new DataPage<PropertyResult>(filter.PageNumber,
                    filter.PageSize, (int)count, props);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the property with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The property or null if not found.</returns>
        public PropertyResult GetProperty(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT p.data_type, p.lit_editor, " +
                    "p.description, ul.uri FROM property p\n" +
                    "INNER JOIN uri_lookup ul ON p.id=ul.id WHERE p.id=@id";
                AddParameter(cmd, "@id", DbType.Int32, id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new PropertyResult
                    {
                        Id = id,
                        DataType = reader.GetValue<string>(0),
                        LiteralEditor = reader.GetValue<string>(1),
                        Description = reader.GetValue<string>(2),
                        Uri = reader.GetString(3)
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the property by its URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The property or null if not found.</returns>
        /// <exception cref="ArgumentNullException">uri</exception>
        public PropertyResult GetPropertyByUri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT p.id, p.data_type, p.lit_editor, " +
                    "p.description FROM property p\n" +
                    "INNER JOIN uri_lookup ul ON p.id=ul.id WHERE ul.uri=@uri";
                AddParameter(cmd, "@uri", DbType.String, uri);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new PropertyResult
                    {
                        Id = reader.GetInt32(0),
                        DataType = reader.GetValue<string>(1),
                        LiteralEditor = reader.GetValue<string>(2),
                        Description = reader.GetValue<string>(3),
                        Uri = uri
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException">property</exception>
        public void AddProperty(Property property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO property(" +
                    "data_type, lit_editor, description) " +
                    "VALUES(@data_type, @lit_editor, @description);";
                AddParameter(cmd, "@data_type", DbType.String, property.DataType);
                AddParameter(cmd, "@lit_editor", DbType.String, property.LiteralEditor);
                AddParameter(cmd, "@description", DbType.String, property.Description);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the property with the specified ID.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        public void DeleteProperty(int id)
        {
            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM property WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region Restriction
        private SqlSelectBuilder GetBuilderFor(PropertyRestrictionFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c");

            builder.AddWhat("pr.id, pr.property_id, pr.restriction, pr.o_id, " +
                "ul.uri AS puri, ul2.uri AS ouri")
                   .AddWhat("COUNT(id)", slotId: "c")
                   .AddFrom("property_restriction pr", slotId: "*")
                   .AddFrom("INNER JOIN uri_lookup ul ON ul.id=pr.property_id")
                   .AddFrom("LEFT JOIN uri_lookup ul2 ON ul2.id=pr.o_id")
                   .AddOrder("ul.uri, id");

            if (filter.PropertyId > 0)
            {
                builder.AddWhere("property_id=@property_id", slotId: "*")
                       .AddParameter("@property_id", DbType.Int32,
                            filter.PropertyId, slotId: "*");
            }

            if (!string.IsNullOrEmpty(filter.Restriction))
            {
                builder.AddWhere("restriction=@restriction", slotId: "*")
                       .AddParameter("@restriction", DbType.String,
                            filter.Restriction, slotId: "*");
            }

            if (filter.ObjectId > 0)
            {
                builder.AddWhere("o_id=@o_id", slotId: "*")
                       .AddParameter("@o_id", DbType.Int32,
                            filter.ObjectId, slotId: "*");
            }

            // limit
            builder.AddLimit(GetPagingSql(filter));

            return builder;
        }

        /// <summary>
        /// Gets the specified page of restrictions.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>Page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<PropertyRestrictionResult> GetRestrictions(
            PropertyRestrictionFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureConnected();

            try
            {
                SqlSelectBuilder builder = GetBuilderFor(filter);

                // get count and ret if no result
                DbCommand cmd = GetCommand();
                cmd.CommandText = builder.Build("c");
                builder.AddParametersTo(cmd, "c");

                long? count = cmd.ExecuteScalar() as long?;
                if (count == null || count == 0)
                {
                    return new DataPage<PropertyRestrictionResult>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<PropertyRestrictionResult>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<PropertyRestrictionResult> restrs =
                    new List<PropertyRestrictionResult>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        restrs.Add(new PropertyRestrictionResult
                        {
                            Id = reader.GetInt32(0),
                            PropertyId = reader.GetInt32(1),
                            Restriction = reader.GetString(2),
                            ObjectId = reader.GetValue<int>(3),
                            PropertyUri = reader.GetString(4),
                            ObjectUri = reader.GetValue<string>(5)
                        });
                    }
                }
                return new DataPage<PropertyRestrictionResult>(filter.PageNumber,
                    filter.PageSize, (int)count, restrs);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the restriction with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The restriction or null if not found.</returns>
        public PropertyRestrictionResult GetRestriction(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT pr.property_id, pr.restriction, pr.o_id " +
                    "ul.uri AS puri, ul2.uri AS ouri\n" +
                    "FROM property_restriction pr\n" +
                    "INNER JOIN uri_lookup ul ON ul.id=pr.property_id\n" +
                    "LEFT JOIN uri_lookup ul2 ON ul2.id=pr.o_id\n" +
                    "WHERE pr.id=@id";
                AddParameter(cmd, "@id", DbType.Int32, id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new PropertyRestrictionResult
                    {
                        Id = reader.GetInt32(0),
                        PropertyId = reader.GetInt32(1),
                        Restriction = reader.GetString(2),
                        ObjectId = reader.GetValue<int>(3),
                        PropertyUri = reader.GetString(4),
                        ObjectUri = reader.GetValue<string>(5)
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds the specified property restriction.
        /// </summary>
        /// <param name="restriction">The restriction.</param>
        /// <exception cref="ArgumentNullException">restriction</exception>
        public void AddRestriction(PropertyRestriction restriction)
        {
            if (restriction == null)
                throw new ArgumentNullException(nameof(restriction));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO property_restriction(" +
                    "property_id, restriction, o_id) " +
                    "VALUES(@property_id, @restriction, @o_id);";
                AddParameter(cmd, "@property_id", DbType.Int32,
                    restriction.PropertyId);
                AddParameter(cmd, "@restriction", DbType.String,
                    restriction.Restriction);
                AddParameter(cmd, "@o_id", DbType.Int32, restriction.ObjectId);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the restriction with the specified ID.
        /// </summary>
        /// <param name="id">The restriction identifier.</param>
        public void DeleteRestriction(int id)
        {
            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM property_restriction WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region Node Mapping
        private SqlSelectBuilder GetBuilderFor(NodeMappingFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c");

            builder.AddWhat("id, parent_id, source_type, name, ordinal, " +
                "facet_filter, group_filter, flags_filter, title_filter, " +
                "part_type, part_role, pin_name, prefix, label_template, " +
                "triple_s, triple_p, triple_o, triple_o_prefix, reversed, " +
                "description")
                   .AddWhat("COUNT(id)", slotId: "c")
                   .AddFrom("node_mapping", slotId: "*")
                   .AddOrder("ul.uri, id");

            if (filter.ParentId > 0)
            {
                builder.AddWhere("parent_id=@parent_id", slotId: "*")
                       .AddParameter("@parent_id", DbType.Int32, filter.ParentId);
            }

            if (filter.SourceTypes?.Count > 0)
            {
                string ids = string.Join(", ", filter.SourceTypes);
                builder.AddWhere($"source_type IN ({ids})", slotId: "*");
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                builder.AddWhere("name LIKE '%@name%'", slotId: "*")
                       .AddParameter("@name", DbType.String, filter.Name);
            }

            if (!string.IsNullOrEmpty(filter.Facet))
            {
                builder.AddWhere("facet=@facet", slotId: "*")
                       .AddParameter("@facet", DbType.String, filter.Facet);
            }

            if (!string.IsNullOrEmpty(filter.Group))
            {
                builder.AddWhere(GetRegexClauseSql("group", "@group"), slotId: "*")
                       .AddParameter("@group", DbType.String, filter.Group);
            }

            if (filter.Flags > 0)
            {
                builder.AddWhere("(flags & @flags)=@flags", slotId: "*")
                       .AddParameter("@flags", DbType.Int32, filter.Flags);
            }

            if (!string.IsNullOrEmpty(filter.Title))
            {
                builder.AddWhere(GetRegexClauseSql("title", "@title"), slotId: "*")
                       .AddParameter("@title", DbType.String, filter.Title);
            }

            if (!string.IsNullOrEmpty(filter.PartType))
            {
                builder.AddWhere("part_type=@part_type", slotId: "*")
                       .AddParameter("@part_type", DbType.String, filter.PartType);
            }

            if (!string.IsNullOrEmpty(filter.PartRole))
            {
                builder.AddWhere("part_role=@part_role", slotId: "*")
                       .AddParameter("@part_role", DbType.String, filter.PartRole);
            }

            if (!string.IsNullOrEmpty(filter.PinName))
            {
                builder.AddWhere("pin_name=@pin_name", slotId: "*")
                       .AddParameter("@pin_name", DbType.String, filter.PinName);
            }

            // limit
            builder.AddLimit(GetPagingSql(filter));

            return builder;
        }

        private static NodeMapping ReadNodeMapping(DbDataReader reader)
        {
            return new NodeMapping
            {
                Id = reader.GetInt32(0),
                ParentId = reader.GetValue<int>(1),
                SourceType = (NodeSourceType)reader.GetInt32(2),
                Name = reader.GetString(3),
                Ordinal = reader.GetValue<int>(4),
                FacetFilter = reader.GetValue<string>(5),
                GroupFilter = reader.GetValue<string>(6),
                FlagsFilter = reader.GetInt32(7),
                TitleFilter = reader.GetValue<string>(8),
                PartType = reader.GetValue<string>(9),
                PartRole = reader.GetValue<string>(10),
                PinName = reader.GetValue<string>(11),
                Prefix = reader.GetValue<string>(12),
                LabelTemplate = reader.GetValue<string>(13),
                TripleS = reader.GetValue<string>(14),
                TripleP = reader.GetValue<string>(15),
                TripleO = reader.GetValue<string>(16),
                TripleOPrefix = reader.GetValue<string>(17),
                IsReversed = reader.GetBoolean(18),
                Description = reader.GetValue<string>(19)
            };
        }

        /// <summary>
        /// Gets the specified page of node mappings.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<NodeMapping> GetNodeMappings(NodeMappingFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            EnsureConnected();

            try
            {
                SqlSelectBuilder builder = GetBuilderFor(filter);

                // get count and ret if no result
                DbCommand cmd = GetCommand();
                cmd.CommandText = builder.Build("c");
                builder.AddParametersTo(cmd, "c");

                long? count = cmd.ExecuteScalar() as long?;
                if (count == null || count == 0)
                {
                    return new DataPage<NodeMapping>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<NodeMapping>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<NodeMapping> mappings =
                    new List<NodeMapping>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mappings.Add(ReadNodeMapping(reader));
                    }
                }
                return new DataPage<NodeMapping>(filter.PageNumber,
                    filter.PageSize, (int)count, mappings);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the node mapping witht the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The mapping or null if not found.</returns>
        public NodeMapping GetNodeMapping(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT id, parent_id, ordinal, facet_filter, " +
                    "group_filter, flags_filter, title_filter, part_type, " +
                    "part_role, pin_name, source_type, prefix, label_template, " +
                    "triple_s, triple_p, triple_o, triple_o_prefix, reversed, " +
                    "description FROM node_mapping WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return ReadNodeMapping(reader);
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds the specified node mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        public void AddNodeMapping(NodeMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO node_mapping(" +
                    "parent_id, ordinal, facet_filter, group_filter, " +
                    "flags_filter, title_filter, part_type, part_role, " +
                    "pin_name, source_type, prefix, label_template, " +
                    "triple_s, triple_p, triple_o, triple_o_prefix, " +
                    "reversed, description) " +
                    "VALUES(@parent_id, @ordinal, @facet_filter, @group_filter, " +
                    "@flags_filter, @title_filter, @part_type, @part_role, " +
                    "@pin_name, @source_type, @prefix, @label_template, " +
                    "@triple_s, @triple_p, @triple_o, @triple_o_prefix, " +
                    "@reversed, @description);";

                AddParameter(cmd, "@parent_id", DbType.Int32, mapping.ParentId);
                AddParameter(cmd, "@ordinal", DbType.Int32, mapping.Ordinal);
                AddParameter(cmd, "@facet_filter", DbType.String,
                    mapping.FacetFilter);
                AddParameter(cmd, "@group_filter", DbType.String,
                    mapping.GroupFilter);
                AddParameter(cmd, "@flags_filter", DbType.Int32,
                    mapping.FlagsFilter);
                AddParameter(cmd, "@title_filter", DbType.String,
                    mapping.TitleFilter);
                AddParameter(cmd, "@part_type", DbType.String, mapping.PartType);
                AddParameter(cmd, "@part_role", DbType.String, mapping.PartRole);
                AddParameter(cmd, "@pin_name", DbType.String, mapping.PinName);
                AddParameter(cmd, "@source_type", DbType.Int32, mapping.SourceType);
                AddParameter(cmd, "@prefix", DbType.String, mapping.Prefix);
                AddParameter(cmd, "@label_template", DbType.String,
                    mapping.LabelTemplate);
                AddParameter(cmd, "@triple_s", DbType.String, mapping.TripleS);
                AddParameter(cmd, "@triple_p", DbType.String, mapping.TripleP);
                AddParameter(cmd, "@triple_o", DbType.String, mapping.TripleO);
                AddParameter(cmd, "@triple_o_prefix", DbType.String,
                    mapping.TripleOPrefix);
                AddParameter(cmd, "@reversed", DbType.Boolean, mapping.IsReversed);
                AddParameter(cmd, "@description", DbType.String,
                    mapping.Description);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the specified node mapping.
        /// </summary>
        /// <param name="id">The mapping identifier.</param>
        public void DeleteMapping(int id)
        {
            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM node_mapping WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion
    }
}
