using Cadmus.Core.Config;
using Fusi.DbManager;
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
        private readonly ISqlTokenHelper _tokenHelper;
        private readonly Regex _wsRegex;
        private readonly Regex _clauseRegex;
        private readonly Regex _simValRegex;
        private readonly Regex _nrRegex;
        private readonly Regex _escRegex;
        private readonly char[] _wildcards;
        private readonly char[] _flagSeparators;
        private readonly Dictionary<string, int> _flags;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryBuilderBase" /> class.
        /// </summary>
        /// <param name="tokenHelper">The SQL token helper to be used.</param>
        protected SqlQueryBuilderBase(ISqlTokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper
                ?? throw new ArgumentNullException(nameof(tokenHelper));

            _wsRegex = new Regex(@"\s+");
            // [nameOPvalue]
            // n=name, o=operator, v=value
            _clauseRegex = new Regex(
                @"\[(?<n>[a-zA-Z]+)(?<o>==|=|\<\>|\*=|\^=|\$=|\?=|~=|%=|!=|\<=|\>=|\<|\>|:|&:|!:)(?<v>[^]]+)\]");
            _simValRegex = new Regex(@":(\d+(?:\.\d+)?)$");
            _nrRegex = new Regex("^[0-9a-fA-F]{1,8}$");
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
        protected string ET(string token) => _tokenHelper.ET(token);

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
        protected string ETP(string prefix, string token,
            string suffix = null) => _tokenHelper.ETP(prefix, token, suffix);

        /// <summary>
        /// Wraps the specified non-keyword tokens and their prefix according to
        /// the syntax of the SQL dialect being handled. For instance, in MySql
        /// this wraps a token into backticks, or in SQL Server into square
        /// brackets.
        /// </summary>
        /// <param name="prefix">The prefix common to all the tokens.</param>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The wrapped tokens.</returns>
        public string ETPS(string prefix, params string[] tokens) =>
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
        protected string SQE(string text, bool hasWildcards = false,
            bool wrapInQuotes = false, bool unicode = true) =>
            _tokenHelper.SQE(text, hasWildcards, wrapInQuotes, unicode);

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
            {
                return int.Parse(flag, NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
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
            string value = UnescapeValue(m.Groups["v"].Value);

            StringBuilder sb = new StringBuilder();
            bool bracket = false;

            // a null name means we are using a pin's name, so we must add
            // 2 clauses: pin.name=name AND pin.value=value
            if (name == null)
            {
                sb.AppendLine("(")
                  .Append(ETP("pin", "name"))
                  .Append('=')
                  .Append(SQE(m.Groups["n"].Value, false, true, false))
                  .AppendLine(" AND");
                name = GetFieldName("value");
                bracket = true;
            }

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
            if (bracket) sb.AppendLine(")");

            return sb.ToString();
        }

        private string BuildWhereSql(string query)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("WHERE");

            // normalize whitespace
            query = _wsRegex.Replace(query, " ").Trim();

            // if no square bracket, assume title as default field
            if (!query.StartsWith("[", StringComparison.Ordinal))
                query = $"[title={query}]";

            // replace \[ or \] with an escape
            query = query.Replace("\\[", "\\005B");
            query = query.Replace("\\]", "\\005D");

            // replace clauses
            string sql = _clauseRegex.Replace(query, BuildClause);

            sb.Append(sql);

            return sb.ToString();
        }

        /// <summary>
        /// Appends the paging instructions.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="sb">The target string builder.</param>
        protected abstract void AppendPaging(PagingOptions options,
            StringBuilder sb);

        private string BuildItemSqlFrom()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("FROM ").AppendLine(ET("item"))
              .Append("INNER JOIN ").AppendLine(ET("pin"))
              .Append("ON ")
              .Append(ETP("item", "id"))
              .Append('=')
              .AppendLine(ETP("pin", "itemId"));
            return sb.ToString();
        }

        private string BuildPinSqlFrom()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("FROM ").AppendLine(ET("pin"))
              .Append("INNER JOIN ").AppendLine(ET("item"))
              .Append("ON ")
              .Append(ETP("pin", "itemId"))
              .Append('=')
              .AppendLine(ETP("item", "id"));
            return sb.ToString();
        }

        /// <summary>
        /// Builds the SQL code corresponding to the specified item query and
        /// paging options. This query returns information about all the items
        /// matching the specified parameters, sorted by item's sort key.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>SQL code for both page and total.</returns>
        /// <exception cref="ArgumentNullException">options or query</exception>
        /// <exception cref="CadmusQueryException">invalid query</exception>
        public Tuple<string, string> BuildForItem(string query, PagingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (query == null) throw new ArgumentNullException(nameof(query));

            StringBuilder sbPage = new StringBuilder();
            StringBuilder sbTotal = new StringBuilder();

            // select distinct item... inner join pin on item.id=pin.itemId
            sbPage.AppendLine("SELECT DISTINCT")
              .AppendLine(ETPS("item",
                "id", "title", "description", "facetId",
                "groupId", "sortKey", "flags",
                "timeCreated", "creatorId", "timeModified", "userId"));

            sbTotal.Append("SELECT COUNT(DISTINCT ")
                   .Append(ETP("item", "id"))
                   .AppendLine(")");

            string fromSql = BuildItemSqlFrom();
            sbPage.Append(fromSql);
            sbTotal.Append(fromSql);

            // where
            string whereSql = BuildWhereSql(query);
            sbPage.Append(whereSql);
            sbTotal.Append(whereSql);

            // order by (for page only)
            sbPage.Append("ORDER BY ")
              .Append(ETP("item", "sortKey"))
              .Append(',')
              .AppendLine(ETP("item", "id"));

            // paging (for page only)
            AppendPaging(options, sbPage);

            return Tuple.Create(sbPage.ToString(), sbTotal.ToString());
        }

        /// <summary>
        /// Builds the SQL code corresponding to the specified pin query and
        /// paging options. This query returns all the pins matching the specified
        /// parameters, sorted by name and value.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="options">The paging options.</param>
        /// <returns>SQL code for both page and total.</returns>
        /// <exception cref="ArgumentNullException">options or query</exception>
        /// <exception cref="CadmusQueryException">invalid query</exception>
        public Tuple<string, string> BuildForPin(string query, PagingOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (query == null) throw new ArgumentNullException(nameof(query));

            StringBuilder sbPage = new StringBuilder();
            StringBuilder sbTotal = new StringBuilder();

            // select distinct item... inner join pin on item.id=pin.itemId
            sbPage.AppendLine("SELECT DISTINCT")
              .AppendLine(ETPS("pin",
                "id", "itemId", "partId", "partTypeId", "roleId", "name", "value"));

            sbTotal.AppendLine("SELECT COUNT(*) FROM (SELECT DISTINCT")
                   .AppendLine(ETPS("pin",
                       "id", "itemId", "partId", "partTypeId", "roleId", "name", "value"));

            string fromSql = BuildPinSqlFrom();
            sbPage.Append(fromSql);
            sbTotal.Append(fromSql);

            // where
            string whereSql = BuildWhereSql(query);
            sbPage.Append(whereSql);
            sbTotal.Append(whereSql);
            sbTotal.Append(") AS tmp");

            // order by (for page only)
            sbPage.Append("ORDER BY ")
              .Append(ETP("pin", "name"))
              .Append(',')
              .Append(ETP("pin", "value"))
              .Append(',')
              .AppendLine(ETP("pin", "id"));

            // paging (for page only)
            AppendPaging(options, sbPage);

            return Tuple.Create(sbPage.ToString(), sbTotal.ToString());
        }
    }
}
