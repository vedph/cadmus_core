using Cadmus.Core;
using System;
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
        private string _connectionString;
        private bool _exists;
        private DbCommand _insertItemCommand;
        private DbCommand _insertPinCommand;
        private DbCommand _deleteItemCommand;

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
        }

        private void BuildInsertCommands()
        {
            // item
            _insertItemCommand = GetCommand();
            _insertItemCommand.CommandText = "INSERT INTO `item`(" +
                "`id`," +
                "`title`," +
                "`description`," +
                "`facetId`," +
                "`groupId`," +
                "`sortKey`," +
                "`flags`)" +
                "VALUES(" +
                "@id," +
                "@title," +
                "@description," +
                "@facetId," +
                "@groupId," +
                "@sortKey," +
                "@flags);";
            _insertItemCommand.Connection = Connection;
            AddParameter(_insertItemCommand, "@id", DbType.String);
            AddParameter(_insertItemCommand, "@title", DbType.String);
            AddParameter(_insertItemCommand, "@description", DbType.String);
            AddParameter(_insertItemCommand, "@facetId", DbType.String);
            AddParameter(_insertItemCommand, "@groupId", DbType.String);
            AddParameter(_insertItemCommand, "@sortKey", DbType.String);
            AddParameter(_insertItemCommand, "@flags", DbType.Int32);

            _deleteItemCommand = GetCommand();
            _deleteItemCommand.CommandText = "DELETE FROM `item` WHERE `id`=@id";
            _deleteItemCommand.Connection = Connection;
            AddParameter(_deleteItemCommand, "@id", DbType.String);

            // pin
            _insertPinCommand = GetCommand();
            _insertPinCommand.CommandText = "INSERT INTO `pin`(" +
                "`itemId`," +
                "`partId`," +
                "`partTypeId`," +
                "`roleId`," +
                "`name`," +
                "`value`," +
                "`timeIndexed`)" +
                "VALUES(" +
                "@itemId," +
                "@partId," +
                "@partTypeId," +
                "@roleId," +
                "@name," +
                "@value," +
                "@timeIndexed);";
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
                        ResourceHelper.LoadResource("MySql.sql"),
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
            _insertPinCommand.Parameters["@partId"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@partTypeId"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@roleId"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@name"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@value"].Value = pin.ItemId;
            _insertPinCommand.Parameters["@timeIndexed"].Value = pin.ItemId;
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
                foreach (DataPin pin in part.GetDataPins())
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
    }
}
