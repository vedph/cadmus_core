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
    /// Tag: <c>item-index-writer.mysql</c>.
    /// </summary>
    [Tag("item-index-writer.mysql")]
    public sealed class MySqlItemIndexWriter : SqlItemIndexWriterBase,
        IItemIndexWriter,
        IConfigurable<SqlOptions>
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
        /// Clears the whole index.
        /// </summary>
        public Task Clear()
        {
            IDbManager manager = new MySqlDbManager(ConnectionString);
            manager.ClearDatabase(GetDbName());
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

        #region IDisposable Support
        private bool _disposedValue; // to detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Connection?.Close();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
