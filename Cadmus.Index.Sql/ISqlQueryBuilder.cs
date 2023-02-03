using Cadmus.Core.Config;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;

namespace Cadmus.Index.Sql;

/// <summary>
/// SQL query builder interface.
/// </summary>
public interface ISqlQueryBuilder
{
    /// <summary>
    /// Sets the flag definitions to be used for clauses involving flags.
    /// </summary>
    /// <param name="definitions">The definitions.</param>
    void SetFlagDefinitions(IList<FlagDefinition> definitions);

    /// <summary>
    /// Builds the SQL code corresponding to the specified item query and
    /// paging options. This query returns information about all the items
    /// matching the specified parameters, sorted by item's sort key.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="options">The paging options.</param>
    /// <returns>SQL code for both page and total.</returns>
    Tuple<string, string> BuildForItem(string query, PagingOptions options);

    /// <summary>
    /// Builds the SQL code corresponding to the specified pin query and
    /// paging options. This query returns all the pins matching the specified
    /// parameters, sorted by name and value.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="options">The paging options.</param>
    /// <returns>SQL code for both page and total.</returns>
    Tuple<string, string> BuildForPin(string query, PagingOptions options);
}
