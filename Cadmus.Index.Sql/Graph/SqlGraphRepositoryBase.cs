using Cadmus.Core;
using Cadmus.Index.Graph;
using Fusi.DbManager;
using Fusi.Tools;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        #region Helpers
        /// <summary>
        /// Gets the paging SQL for the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>SQL code.</returns>
        protected abstract string GetPagingSql(PagingOptions options);

        /// <summary>
        /// Gets the SQL code for a regular expression clause.
        /// </summary>
        /// <param name="text">The text to be compared against the regular
        /// expression pattern. This can be a field name or a literal between
        /// quotes.</param>
        /// <param name="pattern">The regular expression pattern. This can be
        /// a field name or a literal between quotes.</param>
        protected abstract string GetRegexClauseSql(string text, string pattern);

        /// <summary>
        /// Gets the SQL code to append to an insert command in order to get
        /// the last inserted autonumber value.
        /// </summary>
        /// <returns>SQL code.</returns>
        protected abstract string GetSelectIdentitySql();

        /// <summary>
        /// Gets the upsert tail SQL. This is the SQL code appended to a
        /// standard INSERT statement to make it work as an UPSERT. The code
        /// differs according to the RDBMS implementation, but for most RDBMS
        /// it follows the INSERT statement, so this is a quick working solution.
        /// </summary>
        /// <param name="fields">The names of all the inserted fields,
        /// assuming that the corresponding parameter names are equal but
        /// prefixed by <c>@</c>.</param>
        /// <returns>SQL.</returns>
        /// <remarks>MySql: https://www.techbeamers.com/mysql-upsert/;
        /// PostgreSQL: https://www.postgresqltutorial.com/postgresql-upsert/;
        /// SQLServer: https://stackoverflow.com/questions/1197733/
        /// does-sql-server-offer-anything-like-mysqls-on-duplicate-key-update
        /// </remarks>
        protected abstract string GetUpsertTailSql(params string[] fields);
        #endregion

        #region Transaction
        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        public void BeginTransaction(object context = null)
        {
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Commits a write transaction.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        public void CommitTransaction(object context = null)
        {
            Transaction?.Commit();
            Transaction = null;
        }

        /// <summary>
        /// Rollbacks the write transaction.
        /// </summary>
        /// <param name="context">An optional generic context object.</param>
        public void RollbackTransaction(object context = null)
        {
            Transaction?.Rollback();
            Transaction = null;
        }
        #endregion

        #region Namespace Lookup
        private SqlSelectBuilder GetBuilderFor(NamespaceFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c");

            builder.AddWhat("id, uri")
                   .AddWhat("COUNT(id)", slotId: "c")
                   .AddFrom("namespace_lookup", slotId: "*")
                   .AddOrder("id")
                   .AddLimit(GetPagingSql(filter));

            if (!string.IsNullOrEmpty(filter.Prefix))
            {
                builder.AddWhere("id LIKE @id", slotId: "*")
                       .AddParameter("@id", DbType.String, $"%{filter.Prefix}%",
                            slotId: "*");
            }

            if (!string.IsNullOrEmpty(filter.Uri))
            {
                builder.AddWhere("uri LIKE @uri", slotId: "*")
                       .AddParameter("@uri", DbType.String, $"%{filter.Uri}%",
                            slotId: "*");
            }

            return builder;
        }

        /// <summary>
        /// Gets the specified page of namespaces with their prefixes.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<NamespaceEntry> GetNamespaces(NamespaceFilter filter)
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
                    return new DataPage<NamespaceEntry>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<NamespaceEntry>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<NamespaceEntry> nss =
                    new List<NamespaceEntry>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        nss.Add(new NamespaceEntry
                        {
                            Prefix = reader.GetString(0),
                            Uri = reader.GetString(1)
                        });
                    }
                }
                return new DataPage<NamespaceEntry>(filter.PageNumber,
                    filter.PageSize, (int)count, nss);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds or updates the specified namespace prefix.
        /// </summary>
        /// <param name="prefix">The namespace prefix.</param>
        /// <param name="uri">The namespace URI corresponding to
        /// <paramref name="prefix" />.</param>
        /// <exception cref="ArgumentNullException">prefix or uri</exception>
        public void AddNamespace(string prefix, string uri)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO namespace_lookup(id, uri) " +
                    "VALUES(@id, @uri)\n" + GetUpsertTailSql("uri");
                AddParameter(cmd, "@id", DbType.String, prefix);
                AddParameter(cmd, "@uri", DbType.String, uri);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
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

            EnsureConnected();

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
                cmd.CommandText = "DELETE FROM namespace_lookup WHERE id=@id;";
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
                cmd.CommandText = "DELETE FROM namespace_lookup WHERE uri=@uri;";
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
                    "INSERT INTO uid_lookup(sid,unsuffixed,has_suffix) " +
                    "VALUES(@sid,@unsuffixed,@has_suffix);\n" +
                    GetSelectIdentitySql();
                AddParameter(cmdIns, "@sid", DbType.String, sid);
                AddParameter(cmdIns, "@unsuffixed", DbType.String, uid);
                AddParameter(cmdIns, "@has_suffix", DbType.Boolean, false);

                // check if any unsuffixed UID is already in use
                DbCommand cmdSel = GetCommand();
                cmdSel.CommandText =
                    "SELECT 1 FROM uid_lookup WHERE unsuffixed=@uid;";
                AddParameter(cmdSel, "@uid", DbType.String, uid);
                long? result = cmdSel.ExecuteScalar() as long?;

                // no: just insert the unsuffixed UID
                if (result == null)
                {
                    cmdIns.ExecuteNonQuery();
                    return uid;
                }

                // yes: check if a record with the same unsuffixed & SID exists;
                // if so, reuse it; otherwise, add a new suffixed UID
                cmdSel.CommandText = "SELECT id, has_suffix FROM uid_lookup " +
                    "WHERE unsuffixed=@uid AND sid=@sid;";
                AddParameter(cmdSel, "@sid", DbType.String, sid);
                using (var reader = cmdSel.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // found: reuse it, nothing gets inserted
                        int oldId = reader.GetInt32(0);
                        bool hasSuffix = reader.GetBoolean(1);
                        return hasSuffix? uid + "#" + oldId : uid;
                    }
                }
                // not found: add a new suffix
                cmdIns.Parameters["@has_suffix"].Value = true;
                int id = Convert.ToInt32(cmdIns.ExecuteScalar());
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
        /// Adds the specified URI in the mapped URIs set.
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
                // if the URI already exists, just return its ID
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT id FROM uri_lookup WHERE uri=@uri;";
                AddParameter(cmd, "@uri", DbType.String, uri);
                int? result = cmd.ExecuteScalar() as int?;
                if (result != null) return result.Value;

                // else insert it
                DbCommand cmdIns = GetCommand();
                cmdIns.Transaction = Transaction;
                cmdIns.CommandText = "INSERT INTO uri_lookup(uri) " +
                    "VALUES(@uri);\n" + GetSelectIdentitySql();
                AddParameter(cmdIns, "@uri", DbType.String, uri);

                return Convert.ToInt32(cmdIns.ExecuteScalar());
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

            builder.AddWhat("node.id, node.is_class, node.tag, node.label, " +
                "node.source_type, node.sid, ul.uri")
                   .AddWhat("COUNT(node.id)", slotId: "c")
                   .AddFrom("node", slotId: "*")
                   .AddFrom("INNER JOIN uri_lookup ul ON ul.id=node.id")
                   .AddOrder("label, id");

            // uid
            if (!string.IsNullOrEmpty(filter.Uid))
            {
                builder.AddFrom("INNER JOIN uri_lookup ul ON node.id=ul.id",
                            slotId: "*")
                       .AddWhere("uid LIKE @uid", slotId: "*")
                       .AddParameter("@uid", DbType.String, $"%{filter.Uid}%",
                            slotId: "*");
            }

            // class
            if (filter.IsClass.HasValue)
            {
                builder.AddWhere("is_class=@is_class", slotId: "*")
                       .AddParameter("@is_class", DbType.Boolean, filter.IsClass.Value,
                            slotId: "*");
            }

            // tag
            if (filter.Tag != null)
            {
                builder.AddWhere(filter.Tag.Length == 0
                            ? "tag IS NULL" : "tag=@tag", slotId: "*");
                if (filter.Tag.Length > 0)
                {
                    builder.AddParameter("@tag", DbType.String, filter.Tag,
                            slotId: "*");
                }
            }

            // label
            if (!string.IsNullOrEmpty(filter.Label))
            {
                builder.AddWhere("label LIKE @label", slotId: "*")
                       .AddParameter("@label", DbType.String, $"%{filter.Label}%",
                            slotId: "*");
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
                if (filter.IsSidPrefix)
                {
                    builder.AddWhere("sid LIKE @sid", slotId: "*")
                           .AddParameter("@sid", DbType.String, filter.Sid + "%",
                                slotId: "*");
                }
                else
                {
                    builder.AddWhere("sid=@sid", slotId: "*")
                           .AddParameter("@sid", DbType.String, filter.Sid,
                                slotId: "*");
                }
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
                            Tag = reader.GetValue<string>(2),
                            Label = reader.GetValue<string>(3),
                            SourceType = (NodeSourceType)reader.GetInt32(4),
                            Sid = reader.GetValue<string>(5),
                            Uri = reader.GetString(6)
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
                cmd.CommandText = "SELECT n.is_class, n.tag, n.label, " +
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
                        Tag = reader.GetValue<string>(1),
                        Label = reader.GetValue<string>(2),
                        SourceType = (NodeSourceType)reader.GetInt32(3),
                        Sid = reader.GetValue<string>(4),
                        Uri = reader.GetString(5)
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
                cmd.CommandText = "SELECT n.id, n.is_class, n.tag, n.label, " +
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
                        Tag = reader.GetValue<string>(2),
                        Label = reader.GetValue<string>(3),
                        SourceType = (NodeSourceType)reader.GetInt32(4),
                        Sid = reader.GetValue<string>(5),
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
        /// Adds the node only if it does not exist; else do nothing.
        /// </summary>
        /// <param name="node">The node.</param>
        protected abstract void AddNodeIfNotExists(Node node);

        /// <summary>
        /// Adds or updates the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="noUpdate">True to avoid updating an existing node.
        /// When this is true, the node is added when not existing; when
        /// existing, nothing is done.</param>
        /// <exception cref="ArgumentNullException">node</exception>
        public void AddNode(Node node, bool noUpdate = false)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (noUpdate)
            {
                AddNodeIfNotExists(node);
                return;
            }

            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT INTO node" +
                    "(id, is_class, tag, label, source_type, sid) " +
                    "VALUES(@id, @is_class, @tag, @label, @source_type, @sid)\n"
                    + GetUpsertTailSql("is_class", "tag", "label", "source_type", "sid");
                AddParameter(cmd, "@id", DbType.Int32, node.Id);
                AddParameter(cmd, "@is_class", DbType.Boolean, node.IsClass);
                AddParameter(cmd, "@tag", DbType.String, node.Tag);
                AddParameter(cmd, "@label", DbType.String, node.Label);
                AddParameter(cmd, "@source_type", DbType.Int32, node.SourceType);
                AddParameter(cmd, "@sid", DbType.String, node.Sid);

                cmd.ExecuteNonQuery();

                UpdateNodeClasses(node.Id);
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

            builder.AddWhat("p.id, p.data_type, p.lit_editor, p.description, ul.uri")
                   .AddWhat("COUNT(p.id)", slotId: "c")
                   .AddFrom("property p", slotId: "*")
                   .AddFrom("INNER JOIN uri_lookup ul ON ul.id=p.id")
                   .AddOrder("ul.uri");

            if (!string.IsNullOrEmpty(filter.Uid))
            {
                builder.AddFrom("INNER JOIN uri_lookup ul ON ul.id=p.id",
                            slotId: "c")
                       .AddWhere("ul.uri LIKE @uid", slotId: "*")
                       .AddParameter("@uid", DbType.String, $"%{filter.Uid}%",
                            slotId: "*");
            }

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
        /// Adds or updates the specified property.
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
                    "id, data_type, lit_editor, description) " +
                    "VALUES(@id, @data_type, @lit_editor, @description)\n" +
                    GetUpsertTailSql("data_type", "lit_editor", "description");
                AddParameter(cmd, "@id", DbType.Int32, property.Id);
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
                builder.AddWhere("name LIKE @name", slotId: "*")
                       .AddParameter("@name", DbType.String, $"%{filter.Name}%");
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

        private static NodeMapping ReadNodeMapping(DbDataReader reader,
            bool noDescription = false)
        {
            NodeMapping mapping = new NodeMapping
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
                IsReversed = reader.GetBoolean(18)
            };
            if (!noDescription)
                mapping.Description = reader.GetValue<string>(19);

            return mapping;
        }

        /// <summary>
        /// Gets the specified page of node mappings.
        /// </summary>
        /// <param name="filter">The filter. Set page size=0 to get all
        /// the mappings at once.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<NodeMapping> GetMappings(NodeMappingFilter filter)
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
        public NodeMapping GetMapping(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT id, parent_id, source_type, name, " +
                    "ordinal, facet_filter, group_filter, flags_filter, " +
                    "title_filter, part_type, part_role, pin_name, prefix, " +
                    "label_template, triple_s, triple_p, triple_o, triple_o_prefix, " +
                    "reversed, description FROM node_mapping WHERE id=@id;";
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
        /// When <paramref name="mapping"/> has ID=0 (=new mapping), its
        /// <see cref="NodeMapping.Id"/> property gets updated by this method
        /// after insertion.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        public void AddMapping(NodeMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO node_mapping(");

                // an existing mapping must add id
                if (mapping.Id > 0) sb.Append("id, ");

                sb.Append("parent_id, ordinal, facet_filter, group_filter, " +
                    "flags_filter, title_filter, part_type, part_role, " +
                    "pin_name, source_type, name, prefix, label_template, " +
                    "triple_s, triple_p, triple_o, triple_o_prefix, " +
                    "reversed, description)\n");
                sb.Append("VALUES(");

                // an existing mapping must add @id
                if (mapping.Id > 0) sb.Append("@id, ");

                sb.Append("@parent_id, @ordinal, @facet_filter, @group_filter, " +
                    "@flags_filter, @title_filter, @part_type, @part_role, " +
                    "@pin_name, @source_type, @name, @prefix, @label_template, " +
                    "@triple_s, @triple_p, @triple_o, @triple_o_prefix, " +
                    "@reversed, @description)");

                // an existing mapping is an UPSERT, otherwise we have an INSERT
                // and we must retrieve the ID of the newly inserted row
                if (mapping.Id > 0)
                {
                    AddParameter(cmd, "@id", DbType.Int32, mapping.Id);

                    sb.Append('\n').Append(
                        GetUpsertTailSql("parent_id", "ordinal", "facet_filter",
                        "group_filter", "flags_filter", "title_filter",
                        "part_type", "part_role", "pin_name", "source_type", "name",
                        "prefix", "label_template", "triple_s", "triple_p",
                        "triple_o", "triple_o_prefix", "reversed", "description"));
                }
                else
                {
                    sb.Append(";\n").Append(GetSelectIdentitySql());
                }
                cmd.CommandText = sb.ToString();

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
                AddParameter(cmd, "@name", DbType.String, mapping.Name);
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

                if (mapping.Id == 0)
                {
                    object result = cmd.ExecuteScalar();
                    mapping.Id = Convert.ToInt32(result);
                }
                else cmd.ExecuteNonQuery();
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

        private SqlSelectBuilder GetBuilderForMappings(IItem item, IPart part,
            string pin, int parentId)
        {
            SqlSelectBuilder builder = new SqlSelectBuilder(
                () => GetCommand(Connection));

            builder.AddWhat("id, parent_id, source_type, name, " +
                "ordinal, facet_filter, group_filter, flags_filter, " +
                "title_filter, part_type, part_role, pin_name, prefix, " +
                "label_template, triple_s, triple_p, triple_o, " +
                "triple_o_prefix, reversed")
                .AddFrom("node_mapping")
                .AddWhere("parent_id=@parent_id")
                .AddParameter("@parent_id", DbType.Int32, parentId)
                .AddOrder("source_type, ordinal, part_type, part_role, pin_name, name");

            // source_type IN(1,2,3) for items or =4 for parts
            if (part == null)
            {
                builder.AddWhere("AND source_type IN(" +
                    string.Join(", ", new[]
                    {
                        (int)NodeSourceType.Item,
                        (int)NodeSourceType.ItemFacet,
                        (int)NodeSourceType.ItemGroup
                    }) + ")");
            }
            else
            {
                builder.AddWhere("AND source_type=" + (int)NodeSourceType.Pin);
            }

            // facet
            builder.AddWhere("AND (facet_filter IS NULL OR facet_filter=@facet)")
                   .AddParameter("@facet", DbType.String, item.FacetId);

            // flags
            builder.AddWhere("AND (flags_filter=0 OR (flags_filter & @flags)=@flags)")
                   .AddParameter("@flags", DbType.Int32, item.Flags);

            // group
            builder.AddWhere("AND (group_filter IS NULL OR " +
                GetRegexClauseSql("@group", "group_filter") + ")")
                .AddParameter("@group", DbType.String, item.GroupId ?? "");

            // title
            builder.AddWhere("AND (title_filter IS NULL OR " +
                GetRegexClauseSql("@title", "title_filter") + ")")
                .AddParameter("@title", DbType.String, item.Title);

            if (part != null)
            {
                // part_type
                builder.AddWhere("AND (part_type IS NULL OR part_type=@part_type)")
                       .AddParameter("@part_type", DbType.String, part.TypeId);

                // part_role
                builder.AddWhere("AND (part_role IS NULL OR part_role=@part_role)")
                       .AddParameter("@part_role", DbType.String, part.RoleId ?? "");

                // pin_name
                builder.AddWhere("AND (pin_name IS NULL OR pin_name=@pin_name " +
                    "OR (INSTR(pin_name, '@*') > 0 " +
                    "AND " +
                    GetRegexClauseSql("@pin_name",
                        "CONCAT(SUBSTRING(0, INSTR(pin_name, '@*')), '.+')") +
                        "))")
                    .AddParameter("@pin_name", DbType.String, pin ?? "");
            }

            return builder;
        }

        private IList<NodeMapping> FindMappings(IItem item, IPart part, string pin,
            int parentId)
        {
            try
            {
                EnsureConnected();
                SqlSelectBuilder builder = GetBuilderForMappings(item, part,
                    pin, parentId);
                DbCommand cmd = GetCommand();
                cmd.CommandText = builder.Build();
                builder.AddParametersTo(cmd);

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    List<NodeMapping> mappings = new List<NodeMapping>();
                    while (reader.Read())
                        mappings.Add(ReadNodeMapping(reader, true));
                    return mappings;
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Finds all the mappings applicable to the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="parentId">The parent mapping identifier, or 0 to
        /// get root mappings.</param>
        /// <returns>Mappings.</returns>
        /// <exception cref="ArgumentNullException">item</exception>
        public IList<NodeMapping> FindMappingsFor(IItem item, int parentId = 0)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return FindMappings(item, null, null, parentId);
        }

        /// <summary>
        /// Finds all the mappings applicable to the specified part's pin.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part.</param>
        /// <param name="pin">The pin name.</param>
        /// <param name="parentId">The parent mapping identifier, or 0 to
        /// get root mappings.</param>
        /// <returns>Mappings.</returns>
        /// <exception cref="ArgumentNullException">item or part or pin</exception>
        public IList<NodeMapping> FindMappingsFor(IItem item, IPart part, string pin,
            int parentId = 0)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (pin == null) throw new ArgumentNullException(nameof(pin));

            return FindMappings(item, part, pin, parentId);
        }
        #endregion

        #region Triples
        private SqlSelectBuilder GetBuilderFor(TripleFilter filter)
        {
            SqlSelectBuilder builder = GetSelectBuilder();
            builder.EnsureSlots(null, "c")
                    .AddWhat("t.id, t.s_id, t.p_id, t.o_id, t.o_lit, t.sid, t.tag, " +
                             "uls.uri AS s_uri, ulp.uri AS p_uri, ulo.uri AS o_uri")
                    .AddWhat("COUNT(t.id)", slotId: "c")
                    .AddFrom("triple t", slotId: "*")
                    .AddFrom("INNER JOIN uri_lookup uls ON t.s_id=uls.id")
                    .AddFrom("INNER JOIN uri_lookup ulp ON t.p_id=ulp.id")
                    .AddFrom("LEFT JOIN uri_lookup ulo ON t.o_id=ulo.id")
                    .AddOrder("s_uri, p_uri, t.id");

            if (filter.SubjectId > 0)
            {
                builder.AddWhere("s_id=@s_id", slotId: "*")
                       .AddParameter("@s_id", DbType.Int32, filter.SubjectId,
                            slotId: "*");
            }

            if (filter.PredicateId > 0)
            {
                builder.AddWhere("p_id=@p_id", slotId: "*")
                       .AddParameter("@p_id", DbType.Int32, filter.PredicateId,
                            slotId: "*");
            }

            if (filter.ObjectId > 0)
            {
                builder.AddWhere("o_id=@o_id", slotId: "*")
                       .AddParameter("@o_id", DbType.Int32, filter.ObjectId,
                            slotId: "*");
            }

            if (!string.IsNullOrEmpty(filter.ObjectLiteral))
            {
                builder.AddWhere(GetRegexClauseSql("o_lit", "@o_lit"), slotId: "*")
                       .AddParameter("@o_lit", DbType.String, filter.ObjectLiteral,
                            slotId: "*");
            }

            // sid
            if (!string.IsNullOrEmpty(filter.Sid))
            {
                if (filter.IsSidPrefix)
                {
                    builder.AddWhere("sid LIKE @sid", slotId: "*")
                           .AddParameter("@sid", DbType.String, filter.Sid + "%",
                                slotId: "*");
                }
                else
                {
                    builder.AddWhere("sid=@sid", slotId: "*")
                           .AddParameter("@sid", DbType.String, filter.Sid,
                                slotId: "*");
                }
            }

            if (filter.Tag != null)
            {
                builder.AddWhere(filter.Tag.Length == 0
                    ? "tag IS NULL" : "tag=@tag", slotId: "*");
                if (filter.Tag.Length > 0)
                {
                    builder.AddParameter("@tag", DbType.String, filter.Tag,
                        slotId: "*");
                }
            }

            return builder;
        }

        /// <summary>
        /// Gets the specified page of triples.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<TripleResult> GetTriples(TripleFilter filter)
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
                    return new DataPage<TripleResult>(
                        filter.PageNumber, filter.PageSize, 0,
                        Array.Empty<TripleResult>());
                }

                // get page
                cmd.CommandText = builder.Build();
                List<TripleResult> triples = new List<TripleResult>();
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        triples.Add(new TripleResult
                        {
                            Id = reader.GetInt32(0),
                            SubjectId = reader.GetInt32(1),
                            PredicateId = reader.GetInt32(2),
                            ObjectId = reader.GetValue<int>(3),
                            ObjectLiteral = reader.GetValue<string>(4),
                            Sid = reader.GetValue<string>(5),
                            Tag = reader.GetValue<string>(6),
                            SubjectUri = reader.GetString(7),
                            PredicateUri = reader.GetString(8),
                            ObjectUri = reader.GetValue<string>(9)
                        });
                    }
                }
                return new DataPage<TripleResult>(filter.PageNumber,
                    filter.PageSize, (int)count, triples);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the triple with the specified ID.
        /// </summary>
        /// <param name="id">The triple's ID.</param>
        public TripleResult GetTriple(int id)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.CommandText = "SELECT t.s_id, t.p_id, t.o_id, t.o_lit, " +
                    "t.sid, t.tag, " +
                    "uls.uri AS s_uri, ulp.uri AS p_uri, ulo.uri AS o_uri\n" +
                    "FROM triple t\n" +
                    "INNER JOIN uri_lookup uls ON t.s_id=uls.id\n" +
                    "INNER JOIN uri_lookup ulp ON t.p_id=ulp.id\n" +
                    "LEFT JOIN uri_lookup ulo ON t.o_id=ulo.id\n" +
                    "WHERE t.id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return new TripleResult
                    {
                        Id = id,
                        SubjectId = reader.GetInt32(0),
                        PredicateId = reader.GetInt32(1),
                        ObjectId = reader.GetValue<int>(2),
                        ObjectLiteral = reader.GetValue<string>(3),
                        Sid = reader.GetValue<string>(4),
                        Tag = reader.GetValue<string>(5),
                        SubjectUri = reader.GetString(6),
                        PredicateUri = reader.GetString(7),
                        ObjectUri = reader.GetValue<string>(8)
                    };
                }
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Adds or updates the specified triple.
        /// When <paramref name="triple"/> has ID=0 (=new triple), its
        /// <see cref="Triple.Id"/> property gets updated by this method
        /// after insertion.
        /// </summary>
        /// <param name="triple">The triple.</param>
        /// <exception cref="ArgumentNullException">triple</exception>
        public void AddTriple(Triple triple)
        {
            if (triple == null) throw new ArgumentNullException(nameof(triple));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;

                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO triple(");

                // an existing triple must add id
                if (triple.Id > 0) sb.Append("id, ");

                sb.Append("s_id, p_id, o_id, o_lit, sid, tag) VALUES(");

                // an existing triple must add @id
                if (triple.Id > 0) sb.Append("@id, ");

                sb.Append("@s_id, @p_id, @o_id, @o_lit, @sid, @tag)");

                // an existing triple is an UPSERT, otherwise we have an INSERT
                // and we must retrieve the ID of the newly inserted row
                if (triple.Id > 0)
                {
                    AddParameter(cmd, "@id", DbType.Int32, triple.Id);
                    sb.Append('\n').Append(GetUpsertTailSql(
                        "s_id", "p_id", "o_id", "o_lit", "sid"));
                }
                else
                {
                    sb.Append(";\n").Append(GetSelectIdentitySql());
                }
                cmd.CommandText = sb.ToString();

                AddParameter(cmd, "@s_id", DbType.Int32, triple.SubjectId);
                AddParameter(cmd, "@p_id", DbType.Int32, triple.PredicateId);
                AddParameter(cmd, "@o_id", DbType.Int32, triple.ObjectId);
                AddParameter(cmd, "@o_lit", DbType.String, triple.ObjectLiteral);
                AddParameter(cmd, "@sid", DbType.String, triple.Sid);
                AddParameter(cmd, "@tag", DbType.String, triple.Tag);

                if (triple.Id == 0)
                {
                    object result = cmd.ExecuteScalar();
                    triple.Id = Convert.ToInt32(result);
                }
                else cmd.ExecuteNonQuery();

                // update subject node classes when O is not a literal
                if (triple.ObjectId == 0) UpdateNodeClasses(triple.SubjectId);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the triple with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteTriple(int id)
        {
            EnsureConnected();

            try
            {
                // get the triple to delete as its deletion might affect
                // the classes assigned to its subject node
                int subjectId, objectId;
                DbCommand selCmd = GetCommand();
                selCmd.CommandText = "SELECT s_id, o_id\n" +
                    "FROM triple WHERE id=@id;";
                AddParameter(selCmd, "@id", DbType.Int32, id);
                using (DbDataReader reader = selCmd.ExecuteReader())
                {
                    if (!reader.Read()) return;
                    subjectId = reader.GetInt32(0);
                    objectId = reader.GetInt32(1);
                }

                // delete
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "DELETE FROM triple WHERE id=@id;";
                AddParameter(cmd, "@id", DbType.Int32, id);
                cmd.ExecuteNonQuery();

                // update classes if required
                if (objectId > 0) UpdateNodeClasses(subjectId);
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        #region Node Classes
        private void UpdateNodeClasses(int nodeId)
        {
            DbCommand cmd = GetCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "populate_node_class";
            AddParameter(cmd, "instance_id", DbType.Int32, nodeId);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Updates the classes for all the nodes belonging to any class.
        /// </summary>
        /// <param name="cancel">The cancel.</param>
        /// <param name="progress">The progress.</param>
        public Task UpdateNodeClassesAsync(CancellationToken cancel,
            IProgress<ProgressReport> progress = null)
        {
            EnsureConnected();
            try
            {
                DbCommand countCmd = GetCommand();
                countCmd.CommandText = "SELECT COUNT(id) FROM node " +
                    "INNER JOIN node_class WHERE node.id=node_class.node_id;";
                long? result = countCmd.ExecuteScalar() as long?;
                if (result == null) return Task.CompletedTask;

                int total = (int)result.Value;

                DbCommand cmdSel = GetCommand();
                cmdSel.CommandText = "SELECT id FROM node " +
                    "INNER JOIN node_class WHERE node.id=node_class.node_id;";

                using (DbDataReader reader = cmdSel.ExecuteReader())
                {
                    ProgressReport report =
                        progress != null ? new ProgressReport() : null;
                    int oldPercent = 0;

                    DbCommand updCmd = GetCommand();
                    updCmd.CommandType = CommandType.StoredProcedure;
                    updCmd.CommandText = "populate_node_class";
                    AddParameter(updCmd, "instance_id", DbType.Int32);

                    while (reader.Read())
                    {
                        updCmd.Parameters[0].Value = reader.GetInt32(0);
                        updCmd.ExecuteNonQuery();

                        if (report != null && ++report.Count % 10 == 0)
                        {
                            report.Percent = report.Count * 100 / total;
                            if (report.Percent != oldPercent)
                            {
                                progress.Report(report);
                                oldPercent = report.Percent;
                            }
                        }
                        if (cancel.IsCancellationRequested)
                            return Task.CompletedTask;
                    }

                    if (report != null)
                    {
                        report.Percent = 100;
                        report.Count = total;
                        progress.Report(report);
                    }
                }

                return Task.CompletedTask;
            }
            finally
            {
                Disconnect();
            }
        }
        #endregion

        /// <summary>
        /// Gets the set of graph's nodes and triples whose SID starts with
        /// the specified GUID. This identifies all the nodes and triples
        /// generated from a single source item or part.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <returns>The set.</returns>
        /// <exception cref="ArgumentNullException">sourceId</exception>
        public GraphSet GetGraphSet(string sourceId)
        {
            if (sourceId is null)
                throw new ArgumentNullException(nameof(sourceId));

            EnsureConnected();

            try
            {
                DbCommand nodeCmd = GetCommand();
                nodeCmd.CommandText =
                    "SELECT n.id, n.is_class, n.tag, n.label, n.source_type, " +
                    "n.sid, ul.uri " +
                    "FROM node n INNER JOIN uri_lookup ul ON n.id=ul.id" +
                    "WHERE sid LIKE @sid;";
                AddParameter(nodeCmd, "@sid", DbType.String, sourceId + "%");

                List<NodeResult> nodes = new List<NodeResult>();
                using (var nodeReader = nodeCmd.ExecuteReader())
                {
                    while (nodeReader.Read())
                    {
                        nodes.Add(
                            new NodeResult
                            {
                                Id = nodeReader.GetInt32(0),
                                IsClass = nodeReader.GetBoolean(1),
                                Tag = nodeReader.GetValue<string>(2),
                                Label = nodeReader.GetValue<string>(3),
                                SourceType = (NodeSourceType)nodeReader.GetInt32(4),
                                Sid = nodeReader.GetValue<string>(5),
                                Uri = nodeReader.GetString(6)
                            });
                    }
                }

                DataPage<TripleResult> page = GetTriples(new TripleFilter
                {
                    PageNumber = 1,
                    PageSize = 0,
                    Sid = sourceId,
                    IsSidPrefix = true
                });
                return new GraphSet(nodes, page.Items);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Deletes the set of graph's nodes and triples whose SID starts with
        /// the specified GUID. This identifies all the nodes and triples
        /// generated from a single source item or part.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <exception cref="ArgumentNullException">sourceId</exception>
        public void DeleteGraphSet(string sourceId)
        {
            if (sourceId == null) throw new ArgumentNullException(nameof(sourceId));

            EnsureConnected();

            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;

                cmd.CommandText = "DELETE FROM triple WHERE sid LIKE @sid;";
                AddParameter(cmd, "@sid", DbType.String, sourceId + "%");
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM node WHERE sid LIKE @sid;";
                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
