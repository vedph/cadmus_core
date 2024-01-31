using Fusi.Tools.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using System;
using System.Text.RegularExpressions;
using Fusi.DbManager.MySql;

namespace Cadmus.Graph.Ef.MySql;

/// <summary>
/// Entity Framework graph repository for MySql.
/// <para>Tag: <c>graph-repository.ef-my</c>.</para>
/// </summary>
/// <seealso cref="EfGraphRepository" />
[Tag("graph-repository.ef-my")]

public sealed class EfMySqlGraphRepository : EfGraphRepository, IGraphRepository
{
    /// <summary>
    /// Gets the SQL schema.
    /// </summary>
    /// <returns>SQL DDL code.</returns>
    public static string GetSchema()
    {
        using StreamReader reader = new(typeof(EfMySqlGraphRepository).Assembly
            .GetManifestResourceStream("Cadmus.Graph.Ef.MySql.Assets.Schema.mysql")!,
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
        optionsBuilder.UseMySQL(ConnectionString);
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
        return $"{field} REGEXP '{pattern.Replace("'", "''")}'";
    }

    /// <summary>
    /// Creates the target store if it does not exist.
    /// </summary>
    /// <param name="payload">Optional SQL code for seeding preset data.</param>
    /// <returns>
    /// True if created, false if already existing.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Missing connection string for MySql graph repository, or
    /// Missing database name from connection string for MySql graph repository.
    /// </exception>
    public bool CreateStore(object? payload = null)
    {
        // extract database name from connection string
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string for MySql graph repository");
        }
        Regex nameRegex = new("Database=([^;]+)", RegexOptions.IgnoreCase);
        Match m = nameRegex.Match(ConnectionString);
        if (!m.Success)
        {
            throw new InvalidOperationException(
                "Missing database name from connection string " +
                "for MySql graph repository");
        }

        // create database if required
        MySqlDbManager manager = new(
            nameRegex.Replace(ConnectionString, "Database={0}"));
        if (manager.Exists(m.Groups[1].Value)) return false;

        manager.CreateDatabase(m.Groups[1].Value, GetSchema(), payload as string);

        return true;
    }
}