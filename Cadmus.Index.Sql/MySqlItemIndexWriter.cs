using Fusi.Tools.Config;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cadmus.Index.Sql
{
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
        public MySqlItemIndexWriter() : base("MySql")
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
        /// Clears the whole index.
        /// </summary>
        public Task Clear()
        {
            string sysCS = Regex.Replace(
                ConnectionString, "Database=([^;]+)", "Database=sys");
            IDbManager manager = new MySqlDbManager(sysCS);

            string db = GetDbName();
            if (manager.Exists(db)) manager.ClearDatabase(db);

            return Task.CompletedTask;
        }

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
        /// <returns>Command.</returns>
        protected override DbCommand GetCommand() =>
            new MySqlCommand();
    }
}
