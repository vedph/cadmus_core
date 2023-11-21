using Cadmus.Core;
using Cadmus.Index.Sql;
using Fusi.Tools.Configuration;
using Fusi.Tools.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Index.Ef;

/// <summary>
/// Base class for Entity Framework-based item index readers.
/// </summary>
/// <seealso cref="IItemIndexReader" />
public abstract class EfItemIndexReader : IItemIndexReader,
    IConfigurable<EfIndexRepositoryOptions>
{
    private ISqlQueryBuilder? _queryBuilder;

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfItemIndexReader"/> class.
    /// </summary>
    protected EfItemIndexReader()
    {
        ConnectionString = "";
    }

    /// <summary>
    /// Configures this repository with the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">options</exception>
    public void Configure(EfIndexRepositoryOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ConnectionString = options.ConnectionString;
    }

    /// <summary>
    /// Gets the SQL query builder.
    /// </summary>
    /// <returns>SQL query builder.</returns>
    protected abstract ISqlQueryBuilder GetQueryBuilder();

    /// <summary>
    /// Gets a new DB context configured for <see cref="ConnectionString"/>.
    /// </summary>
    /// <returns>DB context.</returns>
    protected abstract CadmusIndexDbContext GetContext();

    /// <summary>
    /// Searches for items in the index with the specified query.
    /// </summary>
    /// <param name="query">The query text.</param>
    /// <param name="options">The paging options.</param>
    /// <returns>Page of results.</returns>
    /// <exception cref="ArgumentNullException">query or options</exception>
    public DataPage<ItemInfo> SearchItems(string query, PagingOptions options)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(options);

        _queryBuilder ??= GetQueryBuilder();
        if (_queryBuilder == null)
        {
            return new DataPage<ItemInfo>(options.PageNumber, options.PageSize,
                0, Array.Empty<ItemInfo>());
        }

        var pageAndTot = _queryBuilder.BuildForItem(query, options);

        using CadmusIndexDbContext context = GetContext();

        int total = context.Set<EfIntWrapper>()
            .FromSqlRaw(pageAndTot.Item2)
            .AsEnumerable()
            .First()
            .Value;
        if (total == 0)
        {
            return new DataPage<ItemInfo>(options.PageNumber, options.PageSize,
                0, Array.Empty<ItemInfo>());
        }

        List<ItemInfo> items = context.Set<ItemInfo>()
            .FromSqlRaw(pageAndTot.Item1).ToList();

        return new DataPage<ItemInfo>(options.PageNumber, options.PageSize,
            total, items);
    }

    /// <summary>
    /// Searches for pins in the index with the specified query.
    /// </summary>
    /// <param name="query">The query text.</param>
    /// <param name="options">The paging options.</param>
    /// <returns>Page of results.</returns>
    /// <exception cref="ArgumentNullException">query or options</exception>
    public DataPage<DataPinInfo> SearchPins(string query, PagingOptions options)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(options);

        _queryBuilder ??= GetQueryBuilder();
        if (_queryBuilder == null)
        {
            return new DataPage<DataPinInfo>(options.PageNumber, options.PageSize,
                0, Array.Empty<DataPinInfo>());
        }

        var pageAndTot = _queryBuilder.BuildForPin(query, options);

        using CadmusIndexDbContext context = GetContext();

        int total = context.Set<EfIntWrapper>()
            .FromSqlRaw(pageAndTot.Item2)
            .AsEnumerable()
            .First()
            .Value;
        if (total == 0)
        {
            return new DataPage<DataPinInfo>(options.PageNumber, options.PageSize,
                0, Array.Empty<DataPinInfo>());
        }

        List<EfIndexPin> pins = context.Set<EfIndexPin>()
            .FromSqlRaw(pageAndTot.Item1).ToList();

        return new DataPage<DataPinInfo>(options.PageNumber, options.PageSize,
            total, pins.ConvertAll(e => e.ToDataPinInfo()));
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
