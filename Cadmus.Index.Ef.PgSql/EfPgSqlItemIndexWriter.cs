using Fusi.DbManager;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Fusi.Tools.Configuration;
using Fusi.DbManager.PgSql;

namespace Cadmus.Index.Ef.PgSql;

/// <summary>
/// PostgreSQl implementation of <see cref="EfItemIndexWriter"/>.
/// </summary>
/// <seealso cref="EfItemIndexWriter" />
[Tag("item-index-writer.ef-pg")]
public sealed class EfPgSqlItemIndexWriter : EfItemIndexWriter
{
    private static string LoadResource(string name)
    {
        // load resource text from assembly manifest resource
        using StreamReader reader = new(Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"Cadmus.Index.Ef.PgSql.Assets.{name}")!,
            Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets the MySql schema used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    public static string GetPgSqlSchema() => LoadResource("Schema.mysql");

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
    /// Gets the database manager.
    /// </summary>
    /// <returns>Database manager.</returns>
    protected override IDbManager GetDbManager()
    {
        return new PgSqlDbManager(ConnectionString);
    }

    /// <summary>
    /// Gets the name of the database from the connection string.
    /// </summary>
    /// <returns>Database name or null.</returns>
    protected override string? GetDbName()
    {
        Match m = Regex.Match(ConnectionString, "Database=([^;]+)",
            RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value : null;
    }

    /// <summary>
    /// Gets the schema SQL used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    protected override string GetSchemaSql() => GetPgSqlSchema();

}