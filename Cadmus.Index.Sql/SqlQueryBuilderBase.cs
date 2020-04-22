using Cadmus.Core.Config;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Base class for SQL query builders.
    /// </summary>
    public abstract class SqlQueryBuilderBase : ISqlQueryBuilder
    {
        private readonly Regex _wsRegex;
        private readonly Regex _clauseRegex;
        private readonly Regex _simValRegex;
        private readonly Regex _nrRegex;
        private readonly Regex _escRegex;
        private readonly char[] _wildcards;
        private readonly char[] _flagSeparators;
        private readonly Dictionary<string, int> _flags;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryBuilderBase"/> class.
        /// </summary>
        protected SqlQueryBuilderBase()
        {
            _wsRegex = new Regex(@"\s+");
            // [nameOPvalue]
            // n=name, o=operator, v=value
            _clauseRegex = new Regex(
                @"\[(?<n>[a-zA-Z]+)(?<o>==|=|\<\>|\*=|\^=|\$=|\?=|~=|%=|!=|\<=|\>=|\<|\>|:|&:|!:)(?<v>[^]]+)\]");
            _simValRegex = new Regex(@":(\d+(?:\.\d+)?)$");
            _nrRegex = new Regex(@"^\d+$");
            _escRegex = new Regex(@"\\([0-9a-fA-F]{4})");
            _wildcards = new[] { '*', '?' };
            _flagSeparators = new[] { ',' };
            _flags = new Dictionary<string, int>();
        }

        /// <summary>
        /// Sets the flag definitions to be used for clauses involving flags.
        /// </summary>
        /// <param name="definitions">The definitions.</param>
        public void SetFlagDefinitions(IList<FlagDefinition> definitions)
        {
            _flags.Clear();
            if (definitions?.Count > 0)
            {
                foreach (FlagDefinition def in definitions)
                    _flags[def.Label.ToLowerInvariant()] = def.Id;
            }
        }

        /// <summary>
        /// Wraps the specified non-keyword token according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The wrapped token.</returns>
        protected abstract string ET(string token);

        /// <summary>
        /// Wraps the specified non-keyword tokens according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The wrapped tokens separated by comma.</returns>
        protected string ETS(params string[] tokens) =>
            string.Join(",", from k in tokens select ET(k));

        /// <summary>
        /// Wraps the specified non-keyword token and its prefix according to
        /// the syntax of the SQL dialect being handled. For instance, in MySql
        /// this wraps a token into backticks, or in SQL Server into square
        /// brackets.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="token">The token.</param>
        /// <param name="suffix">An optional suffix to be appended at the end
        /// of the result.</param>
        /// <returns>The wrapped token.</returns>
        protected abstract string ETP(string prefix, string token,
            string suffix = null);

        /// <summary>
        /// Wraps the specified non-keyword tokens and their prefix according to
        /// the syntax of the SQL dialect being handled. For instance, in MySql
        /// this wraps a token into backticks, or in SQL Server into square
        /// brackets.
        /// </summary>
        /// <param name="prefix">The prefix common to all the tokens.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The wrapped tokens.</returns>
        private string EKPS(string prefix, params string[] tokens) =>
            string.Join(",", from k in tokens select ETP(prefix, k));

        /// <summary>
        /// SQL-encode the specified text, according to the SQL dialect.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hasWildcards">if set to <c>true</c> [has wildcards].
        /// </param>
        /// <param name="wrapInQuotes">if set to <c>true</c> [wrap in quotes].
        /// </param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>Encoded text.</returns>
        protected abstract string SQE(string text, bool hasWildcards = false,
            bool wrapInQuotes = false, bool unicode = true);

        private string GetFieldName(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "title":
                case "t":
                    return ETP("item", "title");
                case "description":
                case "dsc":
                    return ETP("item", "description");
                case "facet":
                case "facetid":
                    return ETP("item", "facetId");
                case "group":
                case "groupid":
                    return ETP("item", "groupId");
                case "sortkey":
                    return ETP("item", "sortKey");
                case "flags":
                    return ETP("item", "flags");
                case "parttypeid":
                case "parttype":
                case "type":
                    return ETP("pin", "partTypeId");
                case "roleid":
                case "role":
                    return ETP("pin", "roleId");
                case "name":
                case "n":
                    return ETP("pin", "name");
                case "value":
                case "v":
                    return ETP("pin", "value");
                default:
                    return null;
            }
        }

        private Tuple<double,string> ParseTresholdAndValue(string value)
        {
            string v = value;
            Match m = _simValRegex.Match(value);
            if (!m.Success || !double.TryParse(m.Groups[1].Value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double treshold))
            {
                treshold = 0.9;
            }
            if (m.Success) v = value.Substring(0, m.Index);

            return Tuple.Create(treshold, v);
        }

        /// <summary>
        /// Appends the regex clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected abstract void AppendRegexClause(string fieldName, string value,
            StringBuilder sb);

        /// <summary>
        /// Appends the similar clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="treshold">The treshold.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected abstract void AppendSimilarClause(
            string fieldName,
            double treshold,
            string value,
            StringBuilder sb);

        /// <summary>
        /// Appends the numeric pair SQL.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="op">The operator.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected abstract void AppendNumericPairSql(string fieldName, string op,
            string value, StringBuilder sb);

        private int ParseFlag(string flag)
        {
            if (_nrRegex.IsMatch(flag))
                return int.Parse(flag, CultureInfo.InvariantCulture);
            return _flags.ContainsKey(flag) ? _flags[flag] : 0;
        }

        private void AppendFlagsSql(string name, string op, string value,
            StringBuilder sb)
        {
            // (name
            sb.Append('(').Append(name);

            // parse all the values and OR them into n
            int[] values =
                (from s in value.ToLowerInvariant().Split(
                _flagSeparators, StringSplitOptions.RemoveEmptyEntries)
                 select ParseFlag(s)).ToArray();
            int n = 0;
            foreach (int v in values) n |= v;

            switch (op)
            {
                case "&:":
                    // & n = n
                    sb.Append(" & ").Append(n).Append(") = ").Append(n).AppendLine();
                    break;
                case "!:":
                    // & n = 0
                    sb.Append(" & ").Append(n).AppendLine(") = 0");
                    break;
                default:
                    // & n <> 0
                    sb.Append(" & ").Append(n).AppendLine(") <> 0");
                    break;
            }
        }

        private string UnescapeValue(string value)
        {
            if (value.IndexOf('\\') == -1) return value;
            return _escRegex.Replace(value, m =>
            {
                return new string(
                    (char)int.Parse(
                        m.Groups[1].Value,
                        NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture), 1);
            });
        }

        private string BuildClause(Match m)
        {
            string name = GetFieldName(m.Groups["n"].Value);
            if (name == null) return "";
            string value = UnescapeValue(m.Groups["v"].Value);

            StringBuilder sb = new StringBuilder();

            switch (m.Groups["o"].Value)
            {
                case "=":   // equal
                    // name='value' (encoded)
                    sb.Append(name)
                      .Append('=')
                      .AppendLine(SQE(value, false, true, false));
                    break;
                case "<>":  // not equal
                    // name<>'value' (encoded)
                    sb.Append(name)
                      .Append("<>")
                      .AppendLine(SQE(value, false, true, false));
                    break;
                case "*=":  // contains
                    // name LIKE '%value%' (encoded)
                    sb.Append(name)
                       .Append(" LIKE '%")
                       .Append(SQE(value, false, false, false))
                       .AppendLine("%'");
                    break;
                case "^=":  // starts-with
                    // name LIKE 'value%' (encoded)
                    sb.Append(name)
                      .Append(" LIKE '")
                      .Append(SQE(value, false, false, false))
                      .AppendLine("%'");
                    break;
                case "$=":  // ends-with
                    // name LIKE '%value' (encoded)
                    sb.Append(name)
                      .Append(" LIKE '%")
                      .Append(SQE(value, false, false, false))
                      .AppendLine("'");
                    break;
                case "?=":  // wildcards (?=1 char, *=0-N chars)
                    // if value has no wildcards, fallback to equals
                    if (value.IndexOfAny(_wildcards) == -1)
                        goto case "=";
                    // translate wildcards: * => %, ? => _
                    string wild = value.Replace('*', '%').Replace('?', '_');
                    // name LIKE 'value' (encoded except for wildcards)
                    sb.Append(name)
                      .Append(" LIKE ")
                      .AppendLine(SQE(wild, true, true, false));
                    break;
                case "~=":  // regex
                    AppendRegexClause(name, value, sb);
                    break;
                case "%=":  // fuzzy
                    var tv = ParseTresholdAndValue(value);
                    AppendSimilarClause(name, tv.Item1, tv.Item2, sb);
                    break;
                // numeric
                case "==":  // equal
                    AppendNumericPairSql(name, "=", value, sb);
                    break;
                case "!=":  // not equal
                    AppendNumericPairSql(name, "<>", value, sb);
                    break;
                case "<":   // less-than
                    AppendNumericPairSql(name, "<", value, sb);
                    break;
                case ">":   // greater-than
                    AppendNumericPairSql(name, ">", value, sb);
                    break;
                case "<=":  // less-than or equal
                    AppendNumericPairSql(name, "<=", value, sb);
                    break;
                case ">=":  // greater-than or equal
                    AppendNumericPairSql(name, ">=", value, sb);
                    break;
                // flags
                case ":":   // has any flags of
                    AppendFlagsSql(name, ":", value, sb);
                    break;
                case "&:":  // has all flags of
                    AppendFlagsSql(name, "&:", value, sb);
                    break;
                case "!:":  // has not any flags of
                    AppendFlagsSql(name, "!:", value, sb);
                    break;
            }

            return sb.ToString();
        }

        private void AppendWhere(string query, StringBuilder sb)
        {
            sb.AppendLine("WHERE");

            // normalize whitespace
            query = _wsRegex.Replace(query, " ").Trim();

            // replace \[ or \] with an escape
            query = query.Replace("\\[", "\\005B");
            query = query.Replace("\\]", "\\005D");

            // replace clauses
            string sql = _clauseRegex.Replace(query, BuildClause);

            sb.Append(sql);
        }

        /// <summary>
        /// Appends the paging instructions.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="sb">The target string builder.</param>
        protected abstract void AppendPaging(PagingOptions options,
            StringBuilder sb);

        /// <summary>
        /// Builds the SQL code corresponding to the specified query and
        /// paging options.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="query">The query.</param>
        /// <returns>SQL code.</returns>
        /// <exception cref="ArgumentNullException">options or query</exception>
        public string Build(PagingOptions options, string query)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (query == null) throw new ArgumentNullException(nameof(query));

            StringBuilder sb = new StringBuilder();

            // select distinct item... inner join pin on item.id=pin.itemId
            sb.AppendLine("SELECT DISTINCT")
              .AppendLine(EKPS("item",
                "id", "title", "description", "facetId",
                "groupId", "sortKey", "flags"))
              .Append("FROM ").AppendLine(ET("item"))
              .Append("INNER JOIN ").AppendLine(ET("pin"))
              .Append("ON ")
              .Append(ETP("item", "id"))
              .Append('=')
              .AppendLine(ETP("pin", "itemId"));

            // where
            AppendWhere(query, sb);

            // order by
            sb.Append("ORDER BY ")
              .Append(ETP("item", "sortKey"))
              .Append(',')
              .AppendLine(ETP("item", "id"));

            // paging
            AppendPaging(options, sb);

            return sb.ToString();
        }
    }
}
