using Cadmus.Graph.Ef.MySql;
using Cadmus.Index.Sql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Fusi.Tools.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cadmus.Index.MySql;

/// <summary>
/// Item index writer for MySql.
/// <para>Tag: <c>item-index-writer.mysql</c>.</para>
/// </summary>
[Tag("item-index-writer.mysql")]
public sealed class MySqlItemIndexWriter : SqlItemIndexWriterBase,
    IItemIndexWriter,
    IConfigurable<SqlOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlItemIndexWriter"/>
    /// class.
    /// </summary>
    public MySqlItemIndexWriter() : base(new MySqlTokenHelper())
    {
    }

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
    /// Clears the whole index.
    /// </summary>
    public Task Clear()
    {
        string sysCS = Regex.Replace(
            ConnectionString, "Database=([^;]+)", "Database=sys");
        IDbManager manager = new MySqlDbManager(sysCS);

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
                $"Cadmus.Index.MySql.Assets.{name}")!, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Gets the MySql schema used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    public static string GetMySqlSchema() =>
        LoadResource("Schema.mysql")
        + "\n" + EfMySqlGraphRepository.GetSchema();

    /// <summary>
    /// Gets the schema SQL used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    protected override string GetSchemaSql() => GetMySqlSchema();

    /// <summary>
    /// Gets the database manager.
    /// </summary>
    /// <returns>
    /// Database manager.
    /// </returns>
    protected override IDbManager GetDbManager() =>
        new MySqlDbManager(ConnectionString);

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
    /// <param name="connection">The connection to use, or null to use
    /// <see cref="SqlRepositoryBase.Connection" />.</param>
    /// <returns>Command.</returns>
    protected override DbCommand GetCommand(DbConnection? connection = null)
    {
        return new MySqlCommand
        {
            Connection = (MySqlConnection)
                (connection as MySqlConnection ?? Connection!)
        };
    }
}
