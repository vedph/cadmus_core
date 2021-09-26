using Cadmus.Index.Graph;
using Cadmus.Index.Sql;
using Cadmus.Index.Sql.Graph;
using Fusi.Tools.Config;
using Fusi.Tools.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Index.MySql
{
    /// <summary>
    /// MySql graph repository.
    /// </summary>
    /// <seealso cref="SqlGraphRepositoryBase" />
    [Tag("graph-repository.mysql")]
    public sealed class MySqlGraphRepository : SqlGraphRepositoryBase,
        IGraphRepository, IConfigurable<SqlOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlGraphRepository"/>
        /// class.
        /// </summary>
        /// <param name="tokenHelper">The token helper.</param>
        public MySqlGraphRepository(ISqlTokenHelper tokenHelper)
            : base(tokenHelper)
        {
        }

        /// <summary>
        /// Configures the object with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public void Configure(SqlOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            ConnectionString = options.ConnectionString;
        }

        /// <summary>
        /// Gets a new command object.
        /// </summary>
        /// <param name="connection">The connection to use, or null to use
        /// <see cref="SqlRepositoryBase.Connection" />.</param>
        /// <returns>Command.</returns>
        protected override DbCommand GetCommand(DbConnection connection = null)
            => new MySqlCommand
            {
                Connection = (MySqlConnection)(connection ?? Connection)
            };

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>Connection.</returns>
        protected override DbConnection GetConnection()
            => new MySqlConnection(ConnectionString);

        /// <summary>
        /// Gets the name of the database from the connection string.
        /// </summary>
        /// <returns>
        /// Database name or null.
        /// </returns>
        protected override string GetDbName()
        {
            Match m = Regex.Match(ConnectionString, "Database=([^;]+)",
                RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : null;
        }

        /// <summary>
        /// Gets the paging SQL for the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>SQL code.</returns>
        /// <exception cref="ArgumentNullException">options</exception>
        protected override string GetPagingSql(PagingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            StringBuilder sb = new StringBuilder();
            if (options.PageSize > 0)
            {
                sb.Append("LIMIT ").Append(options.PageSize);
            }
            if (options.PageNumber > 1)
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append("OFFSET ").Append(options.GetSkipCount());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the SQL code for a regular expression clause.
        /// </summary>
        /// <param name="text">The text to be compared against the regular
        /// expression pattern. This can be a field name or a literal between
        /// quotes.</param>
        /// <param name="pattern">The regular expression pattern. This can be
        /// a field name or a literal between quotes.</param>
        /// <exception cref="ArgumentNullException">text or pattern</exception>
        protected override string GetRegexClauseSql(string text,
            string pattern)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));

            return $"{text} REGEXP({pattern})";
        }

        /// <summary>
        /// Gets the SQL code to append to an insert command in order to get
        /// the last inserted autonumber value.
        /// </summary>
        /// <returns>SQL code.</returns>
        protected override string GetSelectIdentitySql() =>
            "SELECT LAST_INSERT_ID();";

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
        protected override string GetUpsertTailSql(params string[] fields)
        {
            // https://www.techbeamers.com/mysql-upsert/
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ON DUPLICATE KEY UPDATE");
            int n = 0;
            foreach (string field in fields)
            {
                if (++n > 1) sb.Append(", ");
                sb.Append(field).Append("=@").Append(field);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the node only if it does not exist; else do nothing.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AddNodeIfNotExists(Node node)
        {
            EnsureConnected();
            try
            {
                DbCommand cmd = GetCommand();
                cmd.Transaction = Transaction;
                cmd.CommandText = "INSERT IGNORE INTO node" +
                    "(is_class, label, source_type, sid) " +
                    "VALUES(@is_class, @label, @source_type, @sid)\n"
                    + GetUpsertTailSql("is_class", "label", "source_type", "sid");
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
    }
}
