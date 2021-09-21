using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Base class for SQL-repositories.
    /// </summary>
    public abstract class SqlRepositoryBase
    {
        private readonly ISqlTokenHelper _tokenHelper;
        private string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepositoryBase"/>
        /// class.
        /// </summary>
        /// <param name="tokenHelper">The token helper.</param>
        /// <exception cref="ArgumentNullException">tokenHelper</exception>
        protected SqlRepositoryBase(ISqlTokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper ??
                throw new ArgumentNullException(nameof(tokenHelper));
        }

        /// <summary>
        /// Wraps the specified non-keyword token according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The wrapped token.</returns>
        protected string ET(string token) => _tokenHelper.ET(token);

        /// <summary>
        /// Wraps the specified non-keyword tokens according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The wrapped tokens separated by comma.</returns>
        protected string ETS(params string[] tokens) =>
            string.Join(",", from k in tokens select ET(k));

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        protected string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (_connectionString == value) return;
                _connectionString = value;
                OnConnectionStringChanged(value);
            }
        }

        /// <summary>
        /// Called when <see cref="ConnectionString"/> has changed.
        /// The default implementation does nothing.
        /// </summary>
        /// <param name="cs">The cs.</param>
        protected virtual void OnConnectionStringChanged(string cs)
        {
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected DbConnection Connection { get; set; }

        /// <summary>
        /// Gets the name of the database from the connection string.
        /// </summary>
        /// <returns>Database name or null.</returns>
        protected abstract string GetDbName();

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>Connection.</returns>
        protected abstract DbConnection GetConnection();

        /// <summary>
        /// Gets a new command object.
        /// </summary>
        /// <param name="connection">The connection to use, or null to use
        /// <see cref="Connection"/>.</param>
        /// <returns>Command.</returns>
        protected abstract DbCommand GetCommand(DbConnection connection = null);

        /// <summary>
        /// Gets a SQL select statement builder.
        /// </summary>
        /// <returns>Builder.</returns>
        protected SqlSelectBuilder GetSelectBuilder()
        {
            return new SqlSelectBuilder(() => GetCommand(null));
        }

        /// <summary>
        /// Adds the specified parameter to <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The optional value.</param>
        protected static void AddParameter(DbCommand command, string name,
            DbType type, object value = null)
        {
            DbParameter p = command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            if (value != null) p.Value = value;
            command.Parameters.Add(p);
        }

        /// <summary>
        /// Adds the specified parameter to <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The optional value.</param>
        protected static void AddParameter(DbCommand command, string name,
            DbType type, ParameterDirection direction,
            object value = null)
        {
            DbParameter p = command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.Direction = direction;
            if (value != null) p.Value = value;
            command.Parameters.Add(p);
        }

        /// <summary>
        /// Ensures that the database exists and the connection is open.
        /// </summary>
        protected virtual void EnsureConnected()
        {
            // ensure the connection is open
            if (Connection == null) Connection = GetConnection();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
    }
}
