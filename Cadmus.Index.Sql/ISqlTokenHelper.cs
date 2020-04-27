namespace Cadmus.Index.Sql
{
    /// <summary>
    /// Interface for SQL code tokens helper functions.
    /// </summary>
    public interface ISqlTokenHelper
    {
        /// <summary>
        /// Wraps the specified non-keyword token according to the syntax
        /// of the SQL dialect being handled. For instance, in MySql this
        /// wraps a token into backticks, or in SQL Server into square brackets.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The wrapped token.</returns>
        string ET(string token);

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
        string ETP(string prefix, string token, string suffix = null);

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
        string SQE(string text, bool hasWildcards = false,
            bool wrapInQuotes = false, bool unicode = true);
    }
}
