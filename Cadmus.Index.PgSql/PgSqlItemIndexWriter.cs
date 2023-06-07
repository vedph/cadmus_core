using Cadmus.Index.Sql;
using Fusi.DbManager.PgSql;
using Fusi.DbManager;
using Fusi.Tools.Configuration;
using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;

namespace Cadmus.Index.PgSql;

/// <summary>
/// PostgreSql item index writer.
/// </summary>
/// <seealso cref="SqlItemIndexWriterBase" />
/// <seealso cref="IItemIndexWriter" />
public sealed class PgSqlItemIndexWriter : SqlItemIndexWriterBase,
    IItemIndexWriter,
    IConfigurable<SqlOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgSqlItemIndexWriter"/>
    /// class.
    /// </summary>
    public PgSqlItemIndexWriter() : base(new PgSqlTokenHelper())
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
        ConnectionString = options.ConnectionString!;
    }

    /// <summary>
    /// Clears the whole index.
    /// </summary>
    public Task Clear()
    {
        string sysCS = Regex.Replace(
            ConnectionString, "Database=([^;]+)", "Database=sys");
        IDbManager manager = new PgSqlDbManager(sysCS);

        string? db = GetDbName();
        if (db != null && manager.Exists(db)) manager.ClearDatabase(db);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the name of the database from the connection string.
    /// </summary>
    /// <returns>
    /// Database name or null.
    /// </returns>
    protected override string? GetDbName()
    {
        Match m = Regex.Match(ConnectionString, "Database=([^;]+)",
            RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value : null;
    }

    static private string LoadResource(string name)
    {
        using StreamReader reader = new(
            Assembly.GetExecutingAssembly().GetManifestResourceStream(
                $"Cadmus.Index.PgSql.Assets.{name}")!, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets the PgSql schema used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    public static string GetPgSqlSchema() =>
        LoadResource("Schema.pgsql")
        + "\n"; // + TODO: PgSqlGraphRepository.GetSchema();

    /// <summary>
    /// Gets the schema SQL used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    protected override string GetSchemaSql() => GetPgSqlSchema();

    /// <summary>
    /// Gets the database manager.
    /// </summary>
    /// <returns>
    /// Database manager.
    /// </returns>
    protected override IDbManager GetDbManager() =>
        new PgSqlDbManager(ConnectionString);

    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <returns>
    /// Connection.
    /// </returns>
    protected override DbConnection GetConnection() =>
        new NpgsqlConnection(ConnectionString);

    /// <summary>
    /// Gets a new command object.
    /// </summary>
    /// <param name="connection">The connection to use, or null to use
    /// <see cref="SqlRepositoryBase.Connection" />.</param>
    /// <returns>Command.</returns>
    protected override DbCommand GetCommand(DbConnection? connection = null)
    {
        return new NpgsqlCommand
        {
            Connection = (NpgsqlConnection)
                (connection as NpgsqlConnection ?? Connection!)
        };
    }
}
