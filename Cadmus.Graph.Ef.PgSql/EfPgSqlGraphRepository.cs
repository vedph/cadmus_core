using Fusi.DbManager.PgSql;
using Fusi.Tools.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Graph.Ef.PgSql;

/// <summary>
/// Entity Framework graph repository for PostgreSQL.
/// <para>Tag: <c>graph-repository.ef-pg</c>.</para>
/// </summary>
/// <seealso cref="EfGraphRepository" />
[Tag("graph-repository.ef-pg")]
public sealed class EfPgSqlGraphRepository : EfGraphRepository, IGraphRepository
{
    /// <summary>
    /// Gets the SQL schema.
    /// </summary>
    /// <returns>SQL DDL code.</returns>
    public static string GetSchema()
    {
        using StreamReader reader = new(typeof(EfPgSqlGraphRepository).Assembly
            .GetManifestResourceStream("Cadmus.Graph.Ef.PgSql.Assets.Schema.pgsql")!,
            Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets a new DB context configured for <see cref="ConnectionString" />.
    /// </summary>
    /// <returns>DB context.</returns>
    /// <exception cref="InvalidOperationException">No connection string
    /// configured for graph repository</exception>
    protected override CadmusGraphDbContext GetContext()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException(
                "No connection string configured for graph repository");
        }

        DbContextOptionsBuilder<CadmusGraphDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(ConnectionString);
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        return new CadmusGraphDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Builds the regex match expression for field value vs pattern.
    /// </summary>
    /// <param name="field">The field name.</param>
    /// <param name="pattern">The regex pattern.</param>
    /// <returns>SQL code.</returns>
    protected override string BuildRegexMatch(string field, string pattern)
    {
        return $"{field} ~ '{pattern.Replace("'", "''")}'";
    }

    /// <summary>
    /// Creates the target store if it does not exist.
    /// </summary>
    /// <param name="payload">Optional SQL code for seeding preset data.</param>
    /// <returns>
    /// True if created, false if already existing.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Missing connection string for PgSql graph repository, or
    /// Missing database name from connection string for PgSql graph repository.
    /// </exception>
    public bool CreateStore(object? payload = null)
    {
        // extract database name from connection string
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string for PgSql graph repository");
        }
        Regex nameRegex = new("Database=([^;]+)", RegexOptions.IgnoreCase);
        Match m = nameRegex.Match(ConnectionString);
        if (!m.Success)
        {
            throw new InvalidOperationException(
                "Missing database name from connection string " +
                "for PgSql graph repository");
        }

        // create database if required
        PgSqlDbManager manager = new(
            nameRegex.Replace(ConnectionString, "Database={0}"));
        if (manager.Exists(m.Groups[1].Value)) return false;

        manager.CreateDatabase(m.Groups[1].Value, GetSchema(), payload as string);

        return true;
    }
}
