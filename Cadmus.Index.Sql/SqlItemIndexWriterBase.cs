using Cadmus.Core;
using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Base class for <see cref="IItemIndexWriter"/> implementors.
    /// </summary>
    public abstract class SqlItemIndexWriterBase
    {
        private readonly string _resScriptName;
        private readonly ISqlTokenHelper _tokenHelper;
        private string _connectionString;
        private bool _exists;
        private DbCommand _insertItemCommand;
        private DbCommand _insertPinCommand;
        private DbCommand _deleteItemCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlItemIndexWriterBase" />
        /// class.
        /// </summary>
        /// <param name="resScriptName">Name of the resource SQL script
        /// for seeding the database schema.</param>
        /// <param name="tokenHelper">The SQL token helper to be used.</param>
        /// <exception cref="ArgumentNullException">resScriptName</exception>
        protected SqlItemIndexWriterBase(string resScriptName,
            ISqlTokenHelper tokenHelper)
        {
            _resScriptName = resScriptName ??
                throw new ArgumentNullException(nameof(resScriptName));
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
                _exists = false;
            }
        }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        protected DbConnection Connection { get; private set; }

        /// <summary>
        /// Gets the database manager.
        /// </summary>
        /// <returns>Database manager.</returns>
        protected abstract IDbManager GetDbManager();

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
        /// <returns>Command.</returns>
        protected abstract DbCommand GetCommand();

        private static void AddParameter(DbCommand command, string name,
            DbType type)
        {
            DbParameter p = command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            command.Parameters.Add(p);
        }

        private void BuildInsertCommands()
        {
            // item
            _insertItemCommand = GetCommand();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            _insertItemCommand.CommandText =
                $"INSERT INTO {ET("item")}(" +
                ETS("id", "title", "description", "facetId", "groupId",
                    "sortKey", "flags", "timeCreated", "creatorId",
                    "timeModified", "userId") +
                ") VALUES(" +
                "@id," +
                "@title," +
                "@description," +
                "@facetId," +
                "@groupId," +
                "@sortKey," +
                "@flags," +
                "@timeCreated," +
                "@creatorId," +
                "@timeModified," +
                "@userId" +
                ");";
            _insertItemCommand.Connection = Connection;
            AddParameter(_insertItemCommand, "@id", DbType.String);
            AddParameter(_insertItemCommand, "@title", DbType.String);
            AddParameter(_insertItemCommand, "@description", DbType.String);
            AddParameter(_insertItemCommand, "@facetId", DbType.String);
            AddParameter(_insertItemCommand, "@groupId", DbType.String);
            AddParameter(_insertItemCommand, "@sortKey", DbType.String);
            AddParameter(_insertItemCommand, "@flags", DbType.Int32);
            AddParameter(_insertItemCommand, "@timeCreated", DbType.DateTime);
            AddParameter(_insertItemCommand, "@creatorId", DbType.String);
            AddParameter(_insertItemCommand, "@timeModified", DbType.DateTime);
            AddParameter(_insertItemCommand, "@userId", DbType.String);

            _deleteItemCommand = GetCommand();
            _deleteItemCommand.CommandText = "DELETE FROM `item` WHERE `id`=@id";
            _deleteItemCommand.Connection = Connection;
            AddParameter(_deleteItemCommand, "@id", DbType.String);

            // pin
            _insertPinCommand = GetCommand();
            _insertPinCommand.CommandText =
                $"INSERT INTO {ET("pin")}(" +
                ETS("itemId", "partId", "partTypeId", "roleId",
                "name", "value", "timeIndexed") +
                ") VALUES(" +
                "@itemId," +
                "@partId," +
                "@partTypeId," +
                "@roleId," +
                "@name," +
                "@value," +
                "@timeIndexed);";
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
            _insertPinCommand.Connection = Connection;
            AddParameter(_insertPinCommand, "@itemId", DbType.String);
            AddParameter(_insertPinCommand, "@partId", DbType.String);
            AddParameter(_insertPinCommand, "@partTypeId", DbType.String);
            AddParameter(_insertPinCommand, "@roleId", DbType.String);
            AddParameter(_insertPinCommand, "@name", DbType.String);
            AddParameter(_insertPinCommand, "@value", DbType.String);
            AddParameter(_insertPinCommand, "@timeIndexed", DbType.DateTime);
        }

        /// <summary>
        /// Ensures that the database exists and the connection is open.
        /// </summary>
        protected void EnsureConnected()
        {
            // ensure the database exists
            if (!_exists)
            {
                Connection?.Close();
                Connection = null;

                IDbManager manager = GetDbManager();
                string name = GetDbName();
                if (!manager.Exists(name))
                {
                    manager.CreateDatabase(name,
                        ResourceHelper.LoadResource(_resScriptName + ".sql"),
                        null);
                    _exists = true;
                }
            }

            // ensure the connection is open
            if (Connection == null) Connection = GetConnection();
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                _insertItemCommand = null;
                _insertPinCommand = null;
                _deleteItemCommand = null;
            }

            // ensure the insert commands are built
            if (_insertItemCommand == null)
                BuildInsertCommands();
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        protected void InsertItem(IndexItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            EnsureConnected();

            // remove item and its pins
            _deleteItemCommand.Parameters["@id"].Value = item.Id;
            _deleteItemCommand.ExecuteNonQuery();

            // add item
            _insertItemCommand.Parameters["@id"].Value = item.Id;
            _insertItemCommand.Parameters["@title"].Value = item.Title;
            _insertItemCommand.Parameters["@description"].Value = item.Description;
            _insertItemCommand.Parameters["@facetId"].Value = item.FacetId;
            _insertItemCommand.Parameters["@groupId"].Value = item.GroupId;
            _insertItemCommand.Parameters["@sortKey"].Value = item.SortKey;
            _insertItemCommand.Parameters["@flags"].Value = item.Flags;
            _insertItemCommand.Parameters["@timeCreated"].Value = item.TimeCreated;
            _insertItemCommand.Parameters["@creatorId"].Value = item.CreatorId;
            _insertItemCommand.Parameters["@timeModified"].Value = item.TimeModified;
            _insertItemCommand.Parameters["@userId"].Value = item.UserId;
            _insertItemCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Inserts the pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <exception cref="ArgumentNullException">pin</exception>
        protected void InsertPin(IndexPin pin)
        {
            if (pin == null) throw new ArgumentNullException(nameof(pin));

            _insertPinCommand.Parameters["@itemId"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@partId"].Value = pin.PartId;
            _insertPinCommand.Parameters["@partTypeId"].Value = pin.PartTypeId;
            _insertPinCommand.Parameters["@roleId"].Value = pin.RoleId;
            _insertPinCommand.Parameters["@name"].Value = pin.Name;
            _insertPinCommand.Parameters["@value"].Value = pin.Value ?? "";
            _insertPinCommand.Parameters["@timeIndexed"].Value = pin.TimeIndexed;
            _insertPinCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Writes the specified item to the index.
        /// If the index does not exist, it is created.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public Task Write(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            EnsureConnected();

            InsertItem(new IndexItem(item));

            DateTime now = DateTime.UtcNow;
            foreach (IPart part in item.Parts)
            {
                foreach (DataPin pin in part.GetDataPins(item))
                {
                    InsertPin(new IndexPin
                    {
                        ItemId = pin.ItemId,
                        PartId = pin.PartId,
                        RoleId = pin.RoleId,
                        PartTypeId = part.TypeId,
                        Name = pin.Name,
                        Value = pin.Value,
                        TimeIndexed = now
                    });
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes the item with the specified identifier with all its pins
        /// entries.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <exception cref="ArgumentNullException">itemId</exception>
        public Task Delete(string itemId)
        {
            if (itemId == null) throw new ArgumentNullException(nameof(itemId));

            EnsureConnected();

            // remove item and its pins
            _deleteItemCommand.Parameters["@id"].Value = itemId;
            _deleteItemCommand.ExecuteNonQuery();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Closes the connection to the target database.
        /// </summary>
        public void Close()
        {
            Connection?.Close();
        }
    }
}
