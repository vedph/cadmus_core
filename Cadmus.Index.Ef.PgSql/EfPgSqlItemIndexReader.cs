using Cadmus.Index.PgSql;
using Cadmus.Index.Sql;
using Fusi.Tools.Configuration;
using Microsoft.EntityFrameworkCore;
using System;

namespace Cadmus.Index.Ef.PgSql;

/// <summary>
/// Entity Framework-based item index reader for PostgreSQL.
/// </summary>
/// <seealso cref="EfItemIndexReader" />
[Tag("item-index-reader.pg")]
public sealed class EfPgSqlItemIndexReader : EfItemIndexReader
{
    /// <summary>
    /// Gets a new DB context configured for <see cref="ConnectionString" />.
    /// </summary>
    /// <returns>DB context.</returns>
    /// <exception cref="InvalidOperationException">No connection string
    /// configured for graph repository</exception>
    protected override CadmusIndexDbContext GetContext()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException(
                "No connection string configured for graph repository");
        }

        DbContextOptionsBuilder<CadmusIndexDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(ConnectionString);
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        return new CadmusIndexDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Gets the SQL query builder.
    /// </summary>
    /// <returns>
    /// SQL query builder.
    /// </returns>
    protected override ISqlQueryBuilder GetQueryBuilder()
        => new PgSqlQueryBuilder();
}
