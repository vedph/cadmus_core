using Cadmus.Core.Config;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Sql
{
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
        /// Builds the SQL code corresponding to the specified query and
        /// paging options.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="query">The query.</param>
        /// <returns>SQL code.</returns>
        string Build(PagingOptions options, string query);
    }
}
