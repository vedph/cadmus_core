using Cadmus.Index.Graph;
using Cadmus.Index.Sql;
using Cadmus.Index.Sql.Graph;
using Fusi.Tools.Data;
using MySql.Data.MySqlClient;
using System;
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
    }
}
