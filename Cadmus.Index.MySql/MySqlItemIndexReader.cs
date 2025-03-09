using Cadmus.Index.Sql;
using Fusi.Tools.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace Cadmus.Index.MySql;

/// <summary>
/// MySql item index reader.
/// <para>Tag: <c>item-index-reader.mysql</c>.</para>
/// </summary>
/// <seealso cref="SqlItemIndexReaderBase" />
[Tag("item-index-reader.mysql")]
public sealed class MySqlItemIndexReader : SqlItemIndexReaderBase,
    IConfigurable<SqlOptions>, IItemIndexReader
{
    /// <summary>
    /// Configures the object with the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">options</exception>
    public void Configure(SqlOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ConnectionString = options.ConnectionString!;
    }

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <returns>
    /// Connection.
    /// </returns>
    protected override DbConnection GetConnection() =>
        new MySqlConnection(ConnectionString);

    /// <summary>
    /// Gets a new command object.
    /// </summary>
    /// <returns>Command.</returns>
    protected override DbCommand GetCommand() =>
        new MySqlCommand();

    /// <summary>
    /// Gets the SQL query builder.
    /// </summary>
    /// <returns>
    /// SQL query builder.
    /// </returns>
    protected override ISqlQueryBuilder GetQueryBuilder() =>
        new MySqlQueryBuilder();
}
