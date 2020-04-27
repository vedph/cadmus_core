using Fusi.Tools.Config;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Sql Server item index reader.
    /// <para>Tag: <c>item-index-reader.mssql</c>.</para>
    /// </summary>
    /// <seealso cref="SqlItemIndexReaderBase" />
    [Tag("item-index-reader.mssql")]
    public sealed class MsSqlItemIndexReader : SqlItemIndexReaderBase,
        IConfigurable<SqlOptions>, IItemIndexReader
    {
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
        /// Gets the connection.
        /// </summary>
        /// <returns>Connection.</returns>
        protected override DbConnection GetConnection() =>
            new SqlConnection(ConnectionString);

        /// <summary>
        /// Gets a new command object.
        /// </summary>
        /// <returns>Command.</returns>
        protected override DbCommand GetCommand() =>
            new SqlCommand();

        /// <summary>
        /// Gets the SQL query builder.
        /// </summary>
        /// <returns>
        /// SQL query builder.
        /// </returns>
        protected override ISqlQueryBuilder GetQueryBuilder() =>
            new MsSqlQueryBuilder();
    }
}
