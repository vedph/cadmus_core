namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Sql Server SQL token helper.
    /// </summary>
    /// <seealso cref="ISqlTokenHelper" />
    public sealed class MsSqlTokenHelper : ISqlTokenHelper
    {
        internal const string DB_TYPE = "mssql";

        /// <summary>
        /// Wraps the specified non-keyword token according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// The wrapped token.
        /// </returns>
        public string ET(string token) =>
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
        public string ETP(string prefix, string token,
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
        public string SQE(string text, bool hasWildcards = false,
            bool wrapInQuotes = false, bool unicode = true) =>
            SqlHelper.SqlEncode(text, hasWildcards, wrapInQuotes, unicode);
    }
}
