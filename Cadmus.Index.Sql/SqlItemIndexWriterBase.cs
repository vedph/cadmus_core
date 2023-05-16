using Cadmus.Core;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Fusi.DbManager;
using System.Collections.Generic;
using System.Threading;
using Fusi.Tools;

namespace Cadmus.Index.Sql;

/// <summary>
/// Base class for <see cref="IItemIndexWriter"/> implementors.
/// </summary>
public abstract class SqlItemIndexWriterBase : SqlRepositoryBase
{
    private bool _exists;

    /// <summary>
    /// Gets or sets the initialize context. For SQL-based writers, this
    /// is a string with SQL code to execute after the database gets
    /// created.
    /// </summary>
    public object? InitContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlItemIndexWriterBase" />
    /// class.
    /// </summary>
    /// <param name="tokenHelper">The SQL token helper to be used.</param>
    /// <exception cref="ArgumentNullException">resScriptName</exception>
    protected SqlItemIndexWriterBase(ISqlTokenHelper tokenHelper)
        : base(tokenHelper)
    {
    }

    /// <summary>
    /// Called when <see cref="SqlRepositoryBase.ConnectionString" /> has
    /// changed. This resets the exists flag for the current database.
    /// </summary>
    /// <param name="cs">The cs.</param>
    protected override void OnConnectionStringChanged(string cs)
    {
        _exists = false;
    }

    /// <summary>
    /// Gets the database manager.
    /// </summary>
    /// <returns>Database manager.</returns>
    protected abstract IDbManager GetDbManager();

    /// <summary>
    /// Gets the schema SQL used to populate a created database.
    /// </summary>
    /// <returns>SQL code.</returns>
    protected abstract string GetSchemaSql();

    private DbCommand GetInsertItemCommand(bool upsert, DbTransaction? tr = null)
    {
        DbCommand cmd = GetCommand();
        cmd.Transaction = tr;
        cmd.CommandText =
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
            "@userId)";

        if (upsert)
        {
            cmd.CommandText += "\nON DUPLICATE KEY UPDATE " +
            $"{ET("title")}=@title, {ET("description")}=@description, " +
            $"{ET("facetId")}=@facetId, {ET("groupId")}=@groupId, " +
            $"{ET("sortKey")}=@sortKey, {ET("flags")}=@flags, " +
            $"{ET("timeCreated")}=@timeCreated, {ET("creatorId")}=@creatorId, " +
            $"{ET("timeModified")}=@timeModified, {ET("userId")}=@userId;";
        }

        AddParameter(cmd, "@id", DbType.String);
        AddParameter(cmd, "@title", DbType.String);
        AddParameter(cmd, "@description", DbType.String);
        AddParameter(cmd, "@facetId", DbType.String);
        AddParameter(cmd, "@groupId", DbType.String);
        AddParameter(cmd, "@sortKey", DbType.String);
        AddParameter(cmd, "@flags", DbType.Int32);
        AddParameter(cmd, "@timeCreated", DbType.DateTime);
        AddParameter(cmd, "@creatorId", DbType.String);
        AddParameter(cmd, "@timeModified", DbType.DateTime);
        AddParameter(cmd, "@userId", DbType.String);
        return cmd;
    }

    private DbCommand GetDeleteItemCommand(DbTransaction? tr = null)
    {
        // delete item (and its pins by FK constraints)
        DbCommand cmd = GetCommand();
        cmd.Transaction = tr;
        cmd.CommandText = "DELETE FROM `item` WHERE `id`=@id";
        AddParameter(cmd, "@id", DbType.String);
        return cmd;
    }

    private DbCommand GetInsertPinCommand(DbTransaction? tr = null)
    {
        DbCommand cmd = GetCommand();
        cmd.Transaction = tr;
        cmd.CommandText =
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
        AddParameter(cmd, "@itemId", DbType.String);
        AddParameter(cmd, "@partId", DbType.String);
        AddParameter(cmd, "@partTypeId", DbType.String);
        AddParameter(cmd, "@roleId", DbType.String);
        AddParameter(cmd, "@name", DbType.String);
        AddParameter(cmd, "@value", DbType.String);
        AddParameter(cmd, "@timeIndexed", DbType.DateTime);
        return cmd;
    }

    private DbCommand GetDeletePartPinsCommand(DbTransaction? tr = null)
    {
        // delete part pins
        DbCommand cmd = GetCommand();
        cmd.Transaction = tr;
        cmd.CommandText = "DELETE FROM `pin` WHERE `partId`=@partId;";
        AddParameter(cmd, "@partId", DbType.String);
        return cmd;
    }

    /// <summary>
    /// Creates the index if not exists.
    /// </summary>
    /// <exception cref="InvalidOperationException">Null database name in SQL
    /// item index writer</exception>
    public Task CreateIndex()
    {
        // ensure the database exists
        if (!_exists)
        {
            Connection?.Close();
            Connection = null;

            IDbManager manager = GetDbManager();
            string? name = GetDbName() ?? throw new InvalidOperationException(
                "Null database name in SQL item index writer");
            if (!manager.Exists(name))
            {
                manager.CreateDatabase(name,
                    GetSchemaSql(),
                    InitContext as string);
                _exists = true;
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ensures that the database exists and the connection is open.
    /// </summary>
    protected override void EnsureConnected()
    {
        // ensure the database exists
        if (!_exists)
        {
            Connection?.Close();
            Connection = null;

            IDbManager manager = GetDbManager();
            string? name = GetDbName();
            if (name == null)
            {
                throw new InvalidOperationException(
                    "Null database name in SQL item index writer");
            }
            if (!manager.Exists(name))
            {
                manager.CreateDatabase(name,
                    GetSchemaSql(),
                    InitContext as string);
                _exists = true;
            }
        }

        base.EnsureConnected();
    }

    private static void InsertItem(IndexItem item, DbCommand cmd)
    {
        cmd.Parameters["@id"].Value = item.Id;
        cmd.Parameters["@title"].Value = item.Title;
        cmd.Parameters["@description"].Value = item.Description;
        cmd.Parameters["@facetId"].Value = item.FacetId;
        cmd.Parameters["@groupId"].Value = item.GroupId;
        cmd.Parameters["@sortKey"].Value = item.SortKey;
        cmd.Parameters["@flags"].Value = item.Flags;
        cmd.Parameters["@timeCreated"].Value = item.TimeCreated;
        cmd.Parameters["@creatorId"].Value = item.CreatorId;
        cmd.Parameters["@timeModified"].Value = item.TimeModified;
        cmd.Parameters["@userId"].Value = item.UserId;
        cmd.ExecuteNonQuery();
    }

    private static void InsertPin(IndexPin pin, DbCommand cmd)
    {
        cmd.Parameters["@itemId"].Value = pin.ItemId;
        cmd.Parameters["@partId"].Value = pin.PartId;
        cmd.Parameters["@partTypeId"].Value = pin.PartTypeId;
        cmd.Parameters["@roleId"].Value = pin.RoleId;
        cmd.Parameters["@name"].Value = pin.Name;
        cmd.Parameters["@value"].Value = pin.Value ?? "";
        cmd.Parameters["@timeIndexed"].Value = pin.TimeIndexed;
        cmd.ExecuteNonQuery();
    }

    private static void DeleteItem(string id, DbCommand cmd)
    {
        cmd.Parameters["@id"].Value = id;
        cmd.ExecuteNonQuery();
    }

    private static void DeletePartPins(string partId, DbCommand cmd)
    {
        cmd.Parameters["@partId"].Value = partId;
        cmd.ExecuteNonQuery();
    }

    private static IndexPin GetIndexPin(DataPin pin, string partTypeId,
        DateTime now)
    {
        return new IndexPin
        {
            ItemId = pin.ItemId!,
            PartId = pin.PartId!,
            RoleId = pin.RoleId,
            PartTypeId = partTypeId,
            Name = pin.Name!,
            Value = pin.Value!,
            TimeIndexed = now
        };
    }

    /// <summary>
    /// Bulk-writes the specified items, assuming that they do not exist.
    /// This can be used to populate an empty index with higher performance.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <param name="progress">The optional progress reporter.</param>
    /// <exception cref="ArgumentNullException">items</exception>
    public Task WriteItems(IEnumerable<IItem> items,
        CancellationToken cancel, IProgress<ProgressReport>? progress = null)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        EnsureConnected();
        ProgressReport? report = progress != null ? new ProgressReport() : null;
        DbCommand cmd = GetInsertItemCommand(false);

        foreach (IItem item in items)
        {
            InsertItem(new IndexItem(item), cmd);

            // parts
            if (item.Parts?.Count > 0)
            {
                DbCommand insPinCmd = GetInsertPinCommand();
                DateTime now = DateTime.UtcNow;

                foreach (IPart part in item.Parts)
                {
                    foreach (DataPin pin in part.GetDataPins(item))
                    {
                        InsertPin(GetIndexPin(pin, part.TypeId, now),
                            insPinCmd);
                    }
                }
            }

            // progress
            if (progress != null && ++report!.Count % 10 == 0)
                progress.Report(report);

            if (cancel.IsCancellationRequested) break;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Writes the specified item to the index.
    /// If the index does not exist, it is created.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">item</exception>
    public Task WriteItem(IItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        EnsureConnected();

        using (DbTransaction tr = Connection!.BeginTransaction())
        {
            try
            {
                // upsert the item (so that its pins are not removed -
                // pins from a removed part are deleted when the part is removed)
                DbCommand cmd = GetInsertItemCommand(true, tr);
                InsertItem(new IndexItem(item), cmd);

                // write its parts pins
                if (item.Parts?.Count > 0)
                {
                    DbCommand delPinsCommand = GetDeletePartPinsCommand(tr);
                    DbCommand insPinCmd = GetInsertPinCommand(tr);
                    DateTime now = DateTime.UtcNow;

                    foreach (IPart part in item.Parts)
                    {
                        // delete all the part's pins
                        DeletePartPins(part.Id, delPinsCommand);

                        // insert all the new part's pins
                        foreach (DataPin pin in part.GetDataPins(item))
                        {
                            InsertPin(GetIndexPin(pin, part.TypeId, now),
                                insPinCmd);
                        }
                    }
                }
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the item with the specified identifier with all its pins.
    /// </summary>
    /// <param name="itemId">The item identifier.</param>
    /// <exception cref="ArgumentNullException">itemId</exception>
    public Task DeleteItem(string itemId)
    {
        if (itemId == null) throw new ArgumentNullException(nameof(itemId));

        EnsureConnected();

        DbCommand cmd = GetDeleteItemCommand();
        DeleteItem(itemId, cmd);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Writes the specified part's pins to the index.
    /// </summary>
    /// <param name="item">The item the part belongs to.</param>
    /// <param name="part">The part.</param>
    /// <exception cref="ArgumentNullException">Null part.</exception>
    public Task WritePart(IItem item, IPart part)
    {
        if (part == null) throw new ArgumentNullException(nameof(part));

        EnsureConnected();

        using (DbTransaction tr = Connection!.BeginTransaction())
        {
            try
            {
                // delete all the part's pins
                DbCommand cmd = GetDeletePartPinsCommand(tr);
                DeletePartPins(part.Id, cmd);

                // insert all the new part's pins
                DateTime now = DateTime.UtcNow;
                DbCommand pinCmd = GetInsertPinCommand(tr);

                foreach (DataPin pin in part.GetDataPins(item))
                    InsertPin(GetIndexPin(pin, part.TypeId, now), pinCmd);

                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
                throw;
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the pins of the part with the specified ID from the index.
    /// </summary>
    /// <param name="partId">The part identifier.</param>
    /// <exception cref="ArgumentNullException">partId</exception>
    public Task DeletePart(string partId)
    {
        if (partId == null)
            throw new ArgumentNullException(nameof(partId));

        EnsureConnected();
        DbCommand cmd = GetDeletePartPinsCommand();
        DeletePartPins(partId, cmd);

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
