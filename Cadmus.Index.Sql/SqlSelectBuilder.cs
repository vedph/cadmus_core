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
    public sealed class SqlSelectBuilder
    {
        #region SqlBuilderSlot
        /// <summary>
        /// A state slot for the builder.
        /// </summary>
        private class SqlBuilderSlot
        {
            public Lazy<List<string>> What { get; }
            public Lazy<List<string>> From { get; }
            public Lazy<List<string>> Where { get; }
            public Lazy<List<string>> Order { get; }
            public Lazy<List<string>> Limit { get; }
            public Lazy<Dictionary<string, DbParameter>> Parameters { get; }

            public SqlBuilderSlot()
            {
                What = new Lazy<List<string>>();
                From = new Lazy<List<string>>();
                Where = new Lazy<List<string>>();
                Order = new Lazy<List<string>>();
                Limit = new Lazy<List<string>>();
                Parameters = new Lazy<Dictionary<string, DbParameter>>();
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (What.IsValueCreated) sb.Append('S');
                if (From.IsValueCreated) sb.Append('F');
                if (Where.IsValueCreated) sb.Append('W');
                if (Order.IsValueCreated) sb.Append('O');
                if (Limit.IsValueCreated) sb.Append('L');
                return sb.ToString();
            }
        }
        #endregion

        private readonly Dictionary<string, SqlBuilderSlot> _slots;
        private readonly Func<DbCommand> _getCommand;
        private DbCommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSelectBuilder" /> class.
        /// </summary>
        /// <param name="getCommand">The function to get a new instance of a
        /// command.</param>
        /// <exception cref="ArgumentNullException">getCommand</exception>
        public SqlSelectBuilder(Func<DbCommand> getCommand)
        {
            _getCommand = getCommand ??
                throw new ArgumentNullException(nameof(getCommand));
            _slots = new Dictionary<string, SqlBuilderSlot>();
        }

        /// <summary>
        /// Ensures that the specified slots exist. This grants that the
        /// <c>*</c> slot ID works as intended on all the slots, even when you
        /// did not yet use some of them.
        /// </summary>
        /// <param name="slotIds">The slot ids.</param>
        /// <returns>The builder.</returns>
        public SqlSelectBuilder EnsureSlots(params string[] slotIds)
        {
            foreach (string id in slotIds)
            {
                string sid = id ?? "";
                if (!_slots.ContainsKey(sid)) _slots[sid] = new SqlBuilderSlot();
            }
            return this;
        }

        /// <summary>
        /// Removes all slots.
        /// </summary>
        /// <returns>The builder.</returns>
        public SqlSelectBuilder RemoveAllSlots()
        {
            _slots.Clear();
            return this;
        }

        private static void Add(string sql, bool endLine, List<string> target)
        {
            target.Add(sql);
            if (endLine) target.Add(Environment.NewLine);
        }

        /// <summary>
        /// Adds the specified "what" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        /// <param name="slotId">The slot ID to write to, or null to use the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddWhat(string sql, bool endLine = false,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddWhat(sql, endLine, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            Add(sql, endLine, _slots[slotId].What.Value);
            return this;
        }

        /// <summary>
        /// Clears the "what" content.
        /// </summary>
        /// <param name="slotId">The slot ID to clear, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder ClearWhat(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId))
                _slots[slotId].What.Value.Clear();

            return this;
        }

        /// <summary>
        /// Adds the specified "from" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        /// <param name="slotId">The slot ID to write to, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder AddFrom(string sql, bool endLine = false,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddFrom(sql, endLine, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            Add(sql, endLine, _slots[slotId].From.Value);
            return this;
        }

        /// <summary>
        /// Clears the "from" content.
        /// </summary>
        /// <param name="slotId">The slot ID to clear, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder ClearFrom(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId))
                _slots[slotId].From.Value.Clear();

            return this;
        }

        /// <summary>
        /// Adds the specified "where" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        /// <param name="slotId">The slot ID to write to, or null to use the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddWhere(string sql, bool endLine = false,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddWhere(sql, endLine, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            Add(sql, endLine, _slots[slotId].Where.Value);
            return this;
        }

        /// <summary>
        /// Clears the "where" content.
        /// </summary>
        /// <param name="slotId">The slot ID to clear, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder ClearWhere(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId))
                _slots[slotId].Where.Value .Clear();

            return this;
        }

        /// <summary>
        /// Adds the specified "order by" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        /// <param name="slotId">The slot ID to write to, or null to use the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddOrder(string sql, bool endLine = false,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddOrder(sql, endLine, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            Add(sql, endLine, _slots[slotId].Order.Value);
            return this;
        }

        /// <summary>
        /// Clears the "order by" content.
        /// </summary>
        /// <param name="slotId">The slot ID to clear, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder ClearOrder(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId))
                _slots[slotId].Order.Value.Clear();

            return this;
        }

        /// <summary>
        /// Adds the specified "limit" content.
        /// </summary>
        /// <param name="sql">The SQL code to add.</param>
        /// <param name="endLine">if set to <c>true</c>, append a line end
        /// after adding SQL.</param>
        /// <param name="slotId">The slot ID to write to, or null to use the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddLimit(string sql, bool endLine = false,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddLimit(sql, endLine, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            Add(sql, endLine, _slots[slotId].Limit.Value);
            return this;
        }

        /// <summary>
        /// Clears the "limit" content.
        /// </summary>
        /// <param name="slotId">The slot ID to clear, or null to use the
        /// default slot.</param>
        public SqlSelectBuilder ClearLimit(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId))
                _slots[slotId].Limit.Value.Clear();

            return this;
        }

        /// <summary>
        /// Clears a whole slot.
        /// </summary>
        /// <param name="slotId">The target slot ID.</param>
        public SqlSelectBuilder Clear(string slotId = null)
        {
            _slots.Remove(slotId ?? "");
            return this;
        }

        private static void AppendLine(StringBuilder sb)
        {
            if (sb.Length > 0 &&
                sb[sb.Length - 1] != '\n' &&
                sb[sb.Length - 1] != '\r')
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
        /// <param name="slotId">The ID of the slot to get parameters from.</param>
        /// <returns>List of parameters.</returns>
        public IList<DbParameter> GetParameters(string slotId = null)
        {
            if (slotId == null) slotId = "";

            return _slots.ContainsKey(slotId)
                ? (IList<DbParameter>)_slots[slotId].Parameters.Value.Values.ToList()
                : Array.Empty<DbParameter>();
        }

        /// <summary>
        /// Adds the specified parameter to the builder's collection.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="slotId">The target slot ID, or null to target the
        /// default slot, or <c>*</c> to target all the slots.</param>
        /// <exception cref="ArgumentNullException">parameter</exception>
        public SqlSelectBuilder AddParameter(DbParameter parameter,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys) AddParameter(parameter, id);
                return this;
            }
            if (slotId == null) slotId = "";

            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (!_slots.ContainsKey(slotId))
                _slots[slotId] = new SqlBuilderSlot();

            _slots[slotId].Parameters.Value[parameter.ParameterName] = parameter;

            return this;
        }

        /// <summary>
        /// Adds the specified parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="value">The optional value.</param>
        /// <param name="slotId">The target slot ID, or null to target the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddParameter(string name, DbType type,
            object value = null, string slotId = null)
        {
            if (_command == null) _command = _getCommand();
            DbParameter p = _command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            if (value != null) p.Value = value;
            return AddParameter(p, slotId);
        }

        /// <summary>
        /// Adds the specified parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The optional value.</param>
        /// <param name="slotId">The target slot ID, or null to target the
        /// default slot, or <c>*</c> to target all the slots.</param>
        public SqlSelectBuilder AddParameter(string name, DbType type,
            ParameterDirection direction, object value = null,
            string slotId = null)
        {
            if (_command == null) _command = _getCommand();
            DbParameter p = _command.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.Direction = direction;
            if (value != null) p.Value = value;
            return AddParameter(p, slotId);
        }

        /// <summary>
        /// Removes the specified parameter.
        /// </summary>
        /// <param name="name">The parameter's name.</param>
        /// <param name="slotId">The target slot ID, or null to target the
        /// default slot, or <c>*</c> to target all the slots.</param>
        /// <returns>This builder.</returns>
        public SqlSelectBuilder RemoveParameter(string name,
            string slotId = null)
        {
            if (slotId == "*")
            {
                foreach (string id in _slots.Keys)
                    RemoveParameter(name, id);
            }
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId) &&
                _slots[slotId].Parameters.IsValueCreated)
            {
                _slots[slotId].Parameters.Value.Remove(name);
            }

            return this;
        }

        /// <summary>
        /// Clears all the parameters in the specified slot.
        /// </summary>
        /// <param name="slotId">The slot ID, or null to target the default
        /// slot.</param>
        /// <returns>This builder.</returns>
        public SqlSelectBuilder ClearParameters(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (_slots.ContainsKey(slotId) &&
                _slots[slotId].Parameters.IsValueCreated)
            {
                _slots[slotId].Parameters.Value.Clear();
            }

            return this;
        }

        /// <summary>
        /// Adds the parameters from the specified slot to
        /// <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="slotId">The slot identifier.</param>
        /// <exception cref="ArgumentNullException">command</exception>
        public void AddParametersTo(DbCommand command, string slotId = null)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId) ||
                !_slots[slotId].Parameters.IsValueCreated) return;

            command.Parameters.AddRange(
                _slots[slotId].Parameters.Value.Values.ToArray());
        }

        /// <summary>
        /// Builds the SQL for the specified slot.
        /// </summary>
        /// <returns>SQL code.</returns>
        /// <exception cref="InvalidOperationException">slot ID not found</exception>
        public string Build(string slotId = null)
        {
            if (slotId == null) slotId = "";

            if (!_slots.ContainsKey(slotId))
            {
                throw new InvalidOperationException(
                    $"Slot ID {slotId} not found in SQL select statement builder");
            }

            SqlBuilderSlot slot = _slots[slotId];

            StringBuilder sql = new StringBuilder();

            if (slot.What.IsValueCreated)
            {
                sql.Append("SELECT ");
                AppendJoin(", ", slot.What.Value, sql);
            }

            if (slot.From.IsValueCreated)
            {
                AppendLine(sql);
                sql.Append("FROM ");
                AppendJoin("\n", slot.From.Value, sql);
            }

            if (slot.Where.IsValueCreated)
            {
                AppendLine(sql);
                sql.Append("WHERE ");
                AppendJoin(" ", slot.Where.Value, sql);
            }
            if (slot.Order.IsValueCreated)
            {
                AppendLine(sql);
                sql.Append("ORDER BY ");
                AppendJoin(" ", slot.Order.Value, sql);
            }
            if (slot.Limit.IsValueCreated)
            {
                AppendLine(sql);
                AppendJoin(" ", slot.Limit.Value, sql);
            }

            return sql.ToString();
        }
    }
}
