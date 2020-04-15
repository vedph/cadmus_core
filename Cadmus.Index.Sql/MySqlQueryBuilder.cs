using Fusi.Tools.Data;
using System.Globalization;
using System.Text;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// MySql query builder.
    /// </summary>
    /// <seealso cref="SqlQueryBuilderBase" />
    public sealed class MySqlQueryBuilder : SqlQueryBuilderBase
    {
        private const string DB_TYPE = "mysql";

        /// <summary>
        /// Wraps the specified non-keyword token according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// The wrapped token.
        /// </returns>
        protected override string ET(string token) =>
            SqlHelper.EscapeKeyword(token, DB_TYPE);

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
        /// <returns>
        /// The wrapped token.
        /// </returns>
        protected override string ETP(string prefix, string token,
            string suffix = null) =>
            SqlHelper.EscapeKeyword(prefix, DB_TYPE) +
            "." +
            SqlHelper.EscapeKeyword(token, DB_TYPE) +
            (suffix ?? "");

        /// <summary>
        /// SQL-encode the specified text, according to the SQL dialect.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="hasWildcards">if set to <c>true</c> [has wildcards].</param>
        /// <param name="wrapInQuotes">if set to <c>true</c> [wrap in quotes].</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>
        /// Encoded text.
        /// </returns>
        protected override string SQE(string text, bool hasWildcards = false,
            bool wrapInQuotes = false, bool unicode = true) =>
            SqlHelper.SqlEncode(text, hasWildcards, wrapInQuotes, unicode);

        /// <summary>
        /// Appends the regex clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected override void AppendRegexClause(string fieldName, string value,
            StringBuilder sb)
        {
            sb.Append(fieldName)
              .Append(" REGEXP ")
              .AppendLine(SQE(value, false, true));
        }

        /// <summary>
        /// Appends the similar clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="treshold">The treshold.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected override void AppendSimilarClause(string fieldName,
            double treshold, string value, StringBuilder sb)
        {
            // TODO:
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Appends the numeric pair SQL.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="op">The operator.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected override void AppendNumericPairSql(string fieldName,
            string op, string value, StringBuilder sb)
        {
            sb.Append("(\n")
              .Append("  IF (")
              .Append(fieldName)
              .Append(" REGEXP '^[0-9]+$',CAST(")
              .Append(fieldName)
              .Append(" AS SIGNED),NULL)\n")
              .Append(")\n")
              .Append(op)
              .AppendLine(value);
        }

        /// <summary>
        /// Appends the paging instructions.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="sb">The target string builder.</param>
        protected override void AppendPaging(PagingOptions options, StringBuilder sb)
        {
            sb.Append("LIMIT ")
              .AppendLine(
                options.PageSize.ToString(CultureInfo.InvariantCulture))
              .Append("OFFSET ")
              .Append(options.GetSkipCount())
              .AppendLine();
        }
    }
}
