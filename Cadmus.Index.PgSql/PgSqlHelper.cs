using Cadmus.Index.Sql;
using System;

namespace Cadmus.Index.PgSql;

/// <summary>
/// PostgreSql helper.
/// </summary>
/// <seealso cref="ISqlHelper" />
public sealed class PgSqlHelper : ISqlHelper
{
    /// <summary>
    /// Encodes the specified literal text value for SQL.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="hasWildcards">if set to <c>true</c>, the text value
    /// has wildcards.</param>
    /// <param name="wrapInQuotes">if set to <c>true</c>, wrap in quotes
    /// the SQL literal.</param>
    /// <param name="unicode">if set to <c>true</c>, add the Unicode
    /// prefix <c>N</c> before a string literal. This is required in SQL
    /// Server for Unicode strings, while it's harmless in MySql. The
    /// option is meaningful only when <paramref name="wrapInQuotes" /> is
    /// true.</param>
    /// <returns>SQL-encoded value</returns>
    public string SqlEncode(string text, bool hasWildcards = false,
        bool wrapInQuotes = false, bool unicode = true)
        => SqlHelper.SqlEncode(text, hasWildcards, wrapInQuotes, unicode);

    /// <summary>
    /// Encodes the specified date (or date and time) value for SQL.
    /// </summary>
    /// <param name="dt">The value.</param>
    /// <param name="time">if set to <c>true</c>, include the time.</param>
    /// <returns>SQL-encoded value</returns>
    public string SqlEncode(DateTime dt, bool time)
        => SqlHelper.SqlEncode(dt, time);

    /// <summary>
    /// Escapes the specified keyword.
    /// For PgSql we have better not escaping (using ") because this would
    /// make the keyword case-sensitive.
    /// </summary>
    /// <param name="keyword">The keyword.</param>
    /// <returns>Escaped keyword</returns>
    public string EscapeKeyword(string keyword) => keyword;

    /// <summary>
    /// Builds the paging expression with the specified values.
    /// </summary>
    /// <param name="offset">The offset count.</param>
    /// <param name="limit">The limit count.</param>
    /// <returns>SQL code.</returns>
    public string BuildPaging(int offset, int limit)
    {
        return $"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
    }

    /// <summary>
    /// Builds the SQL expression representing a regular expression match
    /// for field <paramref name="name"/> with the specified
    /// <paramref name="pattern"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>SQL code.</returns>
    /// <exception cref="ArgumentNullException">name or pattern</exception>
    public string BuildRegexMatch(string name, string pattern)
    {
        if (name is null) throw new ArgumentNullException(nameof(name));
        if (pattern is null) throw new ArgumentNullException(nameof(pattern));

        return $"{name} ~ {pattern}";
    }
}
