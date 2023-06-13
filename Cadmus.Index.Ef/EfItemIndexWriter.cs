using Cadmus.Core;
using Fusi.DbManager;
using Fusi.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Index.Ef;

/// <summary>
/// Base class for Entity Framework-based item index writers.
/// </summary>
/// <seealso cref="IItemIndexWriter" />
public abstract class EfItemIndexWriter : IItemIndexWriter
{
    private bool _exists;
    private string _cs;

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString
    {
        get => _cs; private set
        {
            _cs = value;
            _exists = false;
        }
    }

    /// <summary>
    /// Gets or sets the initialize context. For SQL-based writers, this
    /// is a string with SQL code to execute after the database gets
    /// created.
    /// </summary>
    public object? InitContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfItemIndexWriter"/> class.
    /// </summary>
    protected EfItemIndexWriter()
    {
        _cs = "";
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

    /// <summary>
    /// Gets the name of the database from the connection string.
    /// </summary>
    /// <returns>Database name or null.</returns>
    protected abstract string? GetDbName();

    /// <summary>
    /// Configures this repository with the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">options</exception>
    public void Configure(EfIndexRepositoryOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        ConnectionString = options.ConnectionString;
    }

    /// <summary>
    /// Gets a new DB context configured for <see cref="ConnectionString"/>.
    /// </summary>
    /// <returns>DB context.</returns>
    protected abstract CadmusIndexDbContext GetContext();

        /// <summary>
    /// Creates the index. If the index already exists, nothing is done.
    /// </summary>
    /// <exception cref="InvalidOperationException">Null database name in SQL
    /// item index writer</exception>
    public Task CreateIndex()
    {
        // ensure the database exists
        if (!_exists)
        {
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
    /// Bulk-writes the specified items, assuming that they do not exist.
    /// This can be used to populate an empty index with higher performance.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="cancel">The cancellation token.</param>
    /// <param name="progress">The optional progress reporter.</param>
    /// <exception cref="ArgumentNullException">items</exception>
    public Task WriteItems(IEnumerable<IItem> items, CancellationToken cancel,
        IProgress<ProgressReport>? progress = null)
    {
        if (items == null) throw new ArgumentNullException(nameof(items));

        CreateIndex();

        using CadmusIndexDbContext context = GetContext();
        context.AddRange(items);
        context.SaveChanges();

        return Task.CompletedTask;
    }

    private static EfIndexPin GetIndexPin(DataPin pin, string partTypeId,
        DateTime now) => new()
        {
            ItemId = pin.ItemId!,
            PartId = pin.PartId!,
            PartRoleId = pin.RoleId,
            PartTypeId = partTypeId,
            Name = pin.Name!,
            Value = pin.Value!,
            TimeIndexed = now
        };

    /// <summary>
    /// Writes the specified item to the index.
    /// If the index does not exist, it is created.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">item</exception>
    public Task WriteItem(IItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        CreateIndex();

        using CadmusIndexDbContext context = GetContext();

        EfIndexItem? old = context.Items.Find(item.Id);
        if (old != null)
        {
            // update old item
            old.Title = item.Title;
            old.Description = item.Description;
            old.FacetId = item.FacetId;
            old.GroupId = item.GroupId;
            old.SortKey = item.SortKey;
            old.Flags = item.Flags;
            old.TimeCreated = item.TimeCreated;
            old.CreatorId = item.CreatorId;
            old.TimeModified = item.TimeModified;
            old.UserId = item.UserId;
        }
        else context.Items.Add(new EfIndexItem(item));

        if (item.Parts?.Count > 0)
        {
            DateTime now = DateTime.UtcNow;

            foreach (IPart part in item.Parts)
            {
                // delete all the part's pins
                context.Pins.RemoveRange(
                    context.Pins.Where(p => p.PartTypeId == part.Id));

                // insert all the new part's pins
                context.Pins.AddRange(part.GetDataPins(item)
                    .Select(p => GetIndexPin(p, part.TypeId, now)));
            }
        }

        context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the item with the specified identifier with all its pins.
    /// </summary>
    /// <param name="itemId">The item identifier.</param>
    /// <exception cref="ArgumentNullException">itemId</exception>
    public Task DeleteItem(string itemId)
    {
        if (itemId is null) throw new ArgumentNullException(nameof(itemId));

        CreateIndex();
        using CadmusIndexDbContext context = GetContext();
        EfIndexItem? item = context.Items.Find(itemId);
        if (item != null)
        {
            context.Items.Remove(item);
            context.SaveChanges();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Writes the specified part's pins to the index.
    /// </summary>
    /// <param name="item">The item the part belongs to.</param>
    /// <param name="part">The part.</param>
    /// <exception cref="ArgumentNullException">Item or part.</exception>
    public Task WritePart(IItem item, IPart part)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        if (part is null) throw new ArgumentNullException(nameof(part));

        CreateIndex();
        using CadmusIndexDbContext context = GetContext();

        // delete all the part's pins
        context.Pins.RemoveRange(
            context.Pins.Where(p => p.PartTypeId == part.Id));

        // insert all the new part's pins
        DateTime now = DateTime.UtcNow;
        context.Pins.AddRange(part.GetDataPins(item)
            .Select(p => GetIndexPin(p, part.TypeId, now)));

        context.SaveChanges();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the pins of the part with the specified ID from the index.
    /// </summary>
    /// <param name="partId">The part identifier.</param>
    /// <exception cref="ArgumentNullException">partId</exception>
    public Task DeletePart(string partId)
    {
        CreateIndex();

        using CadmusIndexDbContext context = GetContext();
        context.Pins.RemoveRange(
            context.Pins.Where(p => p.PartTypeId == partId));
        context.SaveChanges();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears the whole index.
    /// </summary>
    /// <exception cref="InvalidOperationException">Null database name in SQL
    /// item index writer</exception>
    public Task Clear()
    {
        IDbManager manager = GetDbManager();
        manager.ClearDatabase(GetDbName() ?? throw new InvalidOperationException(
                       "Null database name in SQL item index writer"));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the connection to the target database. This does nothing,
    /// since no connection is kept open.
    /// </summary>
    public void Close()
    {
        // nope
    }
}
