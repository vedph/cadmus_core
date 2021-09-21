using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Simple helper for building SQL select commands.
    /// </summary>
    public class SqlSelectBuilder
    {
        private readonly Func<DbCommand> _getCommand;
        private readonly List<string> _what;
        private readonly List<string> _from;
        private readonly List<string> _where;
        private readonly List<string> _order;
        private readonly Dictionary<string, DbParameter> _params;
        private DbCommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectBuilder" /> class.
        /// </summary>
        /// <param name="getCommand">The function to get a new instance of a
        /// command.</param>
        /// <exception cref="ArgumentNullException">getCommand</exception>
        public SqlSelectBuilder(Func<DbCommand> getCommand)
        {
            _getCommand = getCommand ?? throw new ArgumentNullException(nameof(getCommand));
            _what = new List<string>();
            _from = new List<string>();
            _where = new List<string>();
            _order = new List<string>();
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
            _what.Add(sql);
            if (endLine) _what.Add(Environment.NewLine);
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
            _from.Add(sql);
            if (endLine) _from.Add(Environment.NewLine);
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
            _where.Add(sql);
            if (endLine) _where.Add(Environment.NewLine);
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
            _order.Add(sql);
            if (endLine) _order.Add(Environment.NewLine);
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

        private static StringBuilder AppendJoin(string delimiter,
            IEnumerable<string> strings,
            StringBuilder sb)
        {
            int n = 0;
            foreach (string s in strings)
            {
                if (++n > 1) sb.Append(delimiter);
                sb.Append(s);
            }
            return sb;
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <returns>List of parameters.</returns>
        public IList<DbParameter> GetParameters() => _params.Values.ToList();

        /// <summary>
        /// Adds the specified parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The optional value.</param>
        public SqlSelectBuilder AddParameter(string name, DbType type, object value = null)
        {
            if (_command == null) _command = _getCommand();
            DbParameter p = _command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            if (value != null) p.Value = value;
            _params[name] = p;
            return this;
        }

        /// <summary>
        /// Adds the specified parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The optional value.</param>
        public SqlSelectBuilder AddParameter(string name, DbType type,
            ParameterDirection direction, object value = null)
        {
            if (_command == null) _command = _getCommand();
            DbParameter p = _command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.Direction = direction;
            if (value != null) p.Value = value;
            _params[name] = p;
            return this;
        }

        /// <summary>
        /// Removes the specified parameter.
        /// </summary>
        /// <param name="name">The parameter's name.</param>
        /// <returns>This builder.</returns>
        public SqlSelectBuilder RemoveParameter(string name)
        {
            if (_params.ContainsKey(name))
                _params.Remove(name);
            return this;
        }

        /// <summary>
        /// Clears the parameters.
        /// </summary>
        /// <returns>This builder.</returns>
        public SqlSelectBuilder ClearParameters()
        {
            _params.Clear();
            return this;
        }

        /// <summary>
        /// Gets the SQL from this builder.
        /// </summary>
        /// <returns>SQL code.</returns>
        public string GetSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            AppendJoin(",", _what, sql);
            AppendLine(sql);

            sql.Append("FROM ");
            AppendJoin("\n", _from, sql);
            AppendLine(sql);

            if (_where.Count > 0)
            {
                sql.Append("WHERE ");
                AppendJoin(" ", _where, sql);
                AppendLine(sql);
            }
            if (_order.Count > 0)
            {
                sql.Append("ORDER BY ");
                AppendJoin(" ", _what, sql);
                AppendLine(sql);
            }

            return sql.ToString();
        }
    }
}
