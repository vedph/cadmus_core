using Cadmus.Index.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Cadmus.Index.Sql.Graph
{
    /// <summary>
    /// Base class for SQL-based graph repositories.
    /// </summary>
    public abstract class SqlGraphRepositoryBase : SqlRepositoryBase
    {
        /// <summary>
        /// Gets the currently active transaction if any.
        /// </summary>
        protected IDbTransaction Transaction { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlGraphRepositoryBase"/>
        /// class.
        /// </summary>
        /// <param name="tokenHelper">The token helper.</param>
        protected SqlGraphRepositoryBase(ISqlTokenHelper tokenHelper) :
            base(tokenHelper)
        {
        }

        /// <summary>
        /// Commits a write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        public void CommitTransaction(object context)
        {
            Transaction?.Commit();
            Transaction = null;
        }

        /// <summary>
        /// Rollbacks the write transaction.
        /// </summary>
        /// <param name="context">A generic context object.</param>
        public void RollbackTransaction(object context)
        {
            Transaction?.Rollback();
            Transaction = null;
        }

        /// <summary>
        /// Adds the specified SID lookup entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public void AddSid(SidEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            // TODO
        }
    }
}
