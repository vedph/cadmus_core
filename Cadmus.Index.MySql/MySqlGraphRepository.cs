using Cadmus.Index.Graph;
using Cadmus.Index.Sql;
using Cadmus.Index.Sql.Graph;
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
    public sealed class MySqlGraphRepository : SqlGraphRepositoryBase,
        IGraphRepository
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
            if (options.PageNumber > 0)
            {
                sb.Append("OFFSET ").Append(options.PageNumber * options.PageSize);
            }
            if (options.PageSize > 0)
            {
                sb.Append("LIMIT ").Append(options.PageSize);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the SQL code for a regular expression clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="pattern">The regular expression pattern.</param>
        /// <returns>SQL.</returns>
        /// <exception cref="ArgumentNullException">fieldName or pattern</exception>
        protected override string GetRegexClauseSql(string fieldName,
            string pattern)
        {
            if (fieldName == null)
                throw new ArgumentNullException(nameof(fieldName));
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            return $"{fieldName} " +
                $"REGEXP('{SqlHelper.SqlEncode(pattern, false, true, true)}')";
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
                    "VALUES(@id, @uri)\n" +
                    "ON DUPLICATE KEY UPDATE uri=@uri;";
                AddParameter(cmd, "@id", DbType.String, prefix);
                AddParameter(cmd, "@uri", DbType.String, uri);

                cmd.ExecuteNonQuery();
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
