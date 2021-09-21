using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Cadmus.Index.Sql.Graph
{
    /// <summary>
    /// Simple helper for building SQL select commands.
    /// </summary>
    public class SqlSelectBuilder
    {
        private readonly StringBuilder _what;
        private readonly StringBuilder _from;
        private readonly StringBuilder _where;
        private readonly StringBuilder _order;
        private readonly Dictionary<string, DbParameter> _params;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectBuilder"/> class.
        /// </summary>
        public SqlSelectBuilder()
        {
            _what = new StringBuilder();
            _from = new StringBuilder();
            _where = new StringBuilder();
            _order = new StringBuilder();
            _params = new Dictionary<string, DbParameter>();
        }

        private static void AppendEndDelimiter(StringBuilder sb, char delimiter,
            bool ignoreWs = false)
        {
            int i = sb.Length - 1;
            if (ignoreWs)
            {
                while (i >= 0 && char.IsWhiteSpace(sb[i])) i--;
                if (i == -1) return;
            }
            if (sb[i] != delimiter) sb.Append(delimiter);
        }

        private static void Add(string sql, char delimiter, bool endLine,
            StringBuilder sb)
        {
            if (string.IsNullOrEmpty(sql)) return;

            AppendEndDelimiter(sb, delimiter, !char.IsWhiteSpace(delimiter));
            sb.Append(sql);
            if (endLine) sb.AppendLine();
        }

        /// <summary>
        /// Adds the specified "what" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        public SqlSelectBuilder AddWhat(string sql, bool endLine = false)
        {
            Add(sql, ',', endLine, _what);
            return this;
        }

        /// <summary>
        /// Clears the "what" content.
        /// </summary>
        public SqlSelectBuilder ClearWhat()
        {
            _what.Clear();
            return this;
        }

        /// <summary>
        /// Adds the specified "from" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        public SqlSelectBuilder AddFrom(string sql, bool endLine = false)
        {
            Add(sql, ' ', endLine, _from);
            return this;
        }

        /// <summary>
        /// Clears the "from" content.
        /// </summary>
        public SqlSelectBuilder ClearFrom()
        {
            _from.Clear();
            return this;
        }

        /// <summary>
        /// Adds the specified "where" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        public SqlSelectBuilder AddWhere(string sql, bool endLine = false)
        {
            Add(sql, ' ', endLine, _where);
            return this;
        }

        /// <summary>
        /// Clears the "where" content.
        /// </summary>
        public SqlSelectBuilder ClearWhere()
        {
            _where.Clear();
            return this;
        }

        /// <summary>
        /// Adds the specified "order by" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        public SqlSelectBuilder AddOrder(string sql, bool endLine = false)
        {
            Add(sql, ',', endLine, _order);
            return this;
        }

        /// <summary>
        /// Clears the "order by" content.
        /// </summary>
        public SqlSelectBuilder ClearOrder()
        {
            _order.Clear();
            return this;
        }

        /// <summary>
        /// Adds the specified parameter to the builder's collection.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">parameter</exception>
        public SqlSelectBuilder AddParameter(DbParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            _params[parameter.ParameterName] = parameter;
            return this;
        }

        /// <summary>
        /// Clears the whole builder.
        /// </summary>
        /// <param name="preserveParams">if set to <c>true</c>, preserve
        /// parameters.</param>
        public SqlSelectBuilder Clear(bool preserveParams = false)
        {
            _what.Clear();
            _from.Clear();
            _where.Clear();
            _order.Clear();
            if (!preserveParams) _params.Clear();
            return this;
        }

        private static void AppendLine(StringBuilder sb)
        {
            if (sb.Length > 0 &&
                sb[sb.Length - 1] != '\n' &&
                sb[sb.Length - 1] == '\r')
            {
                sb.AppendLine();
            }
        }

        /// <summary>
        /// Gets the SQL from this builder.
        /// </summary>
        /// <returns>SQL code.</returns>
        public string GetSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ").Append(_what);
            AppendLine(sql);

            sql.Append("FROM ").Append(_from);
            AppendLine(sql);

            if (_where.Length > 0)
            {
                sql.Append("WHERE ").Append(_where);
                AppendLine(sql);
            }
            if (_order.Length > 0)
            {
                sql.Append("ORDER BY ").Append(_order);
                AppendLine(sql);
            }

            return sql.ToString();
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>List of parameters.</returns>
        public IList<DbParameter> GetParameters() => _params.Values.ToList();
    }
}
