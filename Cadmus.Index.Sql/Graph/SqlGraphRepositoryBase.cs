using Cadmus.Index.Graph;
using Fusi.Tools.Data;
using System;
using System.Data;
using System.Data.Common;

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
        /// Adds the specified namespace.
        /// </summary>
        /// <param name="prefix">The namespace prefix.</param>
        /// <param name="uri">The namespace URI corresponding to
        /// <paramref name="prefix" />.</param>
        /// <exception cref="ArgumentNullException">prefix or uri</exception>
        public void AddNamespace(string prefix, string uri)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a namespace by prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <exception cref="ArgumentNullException">prefix</exception>
        public void DeleteNamespaceByPrefix(string prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified namespace with all its prefixes.
        /// </summary>
        /// <param name="uri">The namespace URI.</param>
        public void DeleteNamespaceByUri(string uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            throw new NotImplementedException();
        }

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

            DbCommand cmd = GetCommand();
            cmd.Transaction = Transaction;
            cmd.CommandText = "INSERT INTO uri_lookup(id,uri) VALUES(@id,@uri);";
            AddParameter(cmd, "@uri", DbType.String, uri);
            AddParameter(cmd, "@id", DbType.Int32, ParameterDirection.Output);

            cmd.ExecuteNonQuery();
            return (int)cmd.Parameters["@id"].Value;
        }

        /// <summary>
        /// Lookups the URI from its numeric ID.
        /// </summary>
        /// <param name="id">The numeric ID for the URI.</param>
        /// <returns>The URI, or null if not found.</returns>
        public string LookupUri(int id)
        {
            EnsureConnected();

            DbCommand cmd = GetCommand();
            cmd.Transaction = Transaction;
            cmd.CommandText = "SELECT uri FROM uri_lookup WHERE id=@id;";
            AddParameter(cmd, "@id", DbType.Int32, id);
            return cmd.ExecuteScalar() as string;
        }

        /// <summary>
        /// Gets the requested page of nodes.
        /// </summary>
        /// <param name="filter">The nodes filter.</param>
        /// <returns>The page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<Node> GetNodes(NodeFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            SqlSelectBuilder builder = GetSelectBuilder();
            builder.AddWhat("id,label,source_type,sid")
                   .AddFrom("node")
                   .AddOrder("label,id");

            // uid
            if (!string.IsNullOrEmpty(filter.Uid))
            {
                builder.AddFrom("INNER JOIN uri_lookup ul ON node.id=ul.id")
                       .AddWhere("uid LIKE '%@uid%'")
                       .AddParameter("@uid", DbType.String, filter.Uid);
            }

            // label
            if (!string.IsNullOrEmpty(filter.Label))
            {
                builder.AddWhere("label LIKE '%@label%'")
                       .AddParameter("@label", DbType.String, filter.Label);
            }

            // source type
            if (filter.SourceType != null)
            {
                builder.AddWhere("source_type=@source_type")
                       .AddParameter("@source_type",
                            DbType.Int32, filter.SourceType.Value);
            }

            // sid
            if (!string.IsNullOrEmpty(filter.Sid))
            {
                builder.AddWhere(filter.IsSidPrefix?
                        "sid LIKE '@sid%'" : "sid=@sid")
                       .AddParameter("@sid", DbType.String, filter.Sid);
            }

            // linked node ID and role
            if (filter.LinkedNodeId > 0)
            {
                builder.AddParameter("@lnid", DbType.Int32, filter.LinkedNodeId);

                switch (char.ToUpperInvariant(filter.LinkedNodeRole))
                {
                    case 'S':
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON t.s_id=@lnid AND t.o_id=node.id");
                        break;
                    case 'O':
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON t.o_id=@lnid AND t.s_id=node.id");
                        break;
                    default:
                        builder.AddFrom("INNER JOIN triple t " +
                            "ON (t.s_id=@lnid AND t.o_id=node.id) OR " +
                            "(t.o_id=@lnid AND t.s_id=node.id)");
                        break;
                }
            }

            // class IDs

            // TODO
            throw new NotImplementedException();
        }
    }
}
