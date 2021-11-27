using Cadmus.Index.Graph;
using Cadmus.Index.Sql;
using Cadmus.Index.Sql.Graph;
using Fusi.Tools.Config;
using MySql.Data.MySqlClient;
using SqlKata.Compilers;
using System;
using System.Data;

namespace Cadmus.Index.MySql
{
    /// <summary>
    /// MySql graph repository.
    /// Tag: <c>graph-repository.sql-my</c>.
    /// </summary>
    /// <seealso cref="SqlGraphRepositoryBase" />
    [Tag("graph-repository.sql-my")]
    public sealed class MySqlGraphRepository : SqlGraphRepositoryBase,
        IGraphRepository, IConfigurable<SqlOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlGraphRepository"/>
        /// class.
        /// </summary>
        public MySqlGraphRepository() : base(new MySqlCompiler(), new MySqlHelper())
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
        /// Gets a connection.
        /// </summary>
        /// <returns>Connection.</returns>
        protected override IDbConnection GetConnection()
            => new MySqlConnection(ConnectionString);
    }
}
