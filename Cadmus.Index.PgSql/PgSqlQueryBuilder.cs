using Cadmus.Index.Sql;
using Fusi.DbManager.PgSql;
using Fusi.Tools.Data;
using System.Globalization;
using System.Text;

namespace Cadmus.Index.PgSql;

/// <summary>
/// PostgreSql query builder.
/// </summary>
/// <seealso cref="SqlQueryBuilderBase" />
public sealed class PgSqlQueryBuilder : SqlQueryBuilderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgSqlQueryBuilder"/>
    /// class.
    /// </summary>
    public PgSqlQueryBuilder() : base(new PgSqlTokenHelper(), false)
    {
    }

    /// <summary>
    /// Appends the regex clause.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="value">The regular expression pattern.</param>
    /// <param name="sb">The string builder to append to.</param>
    protected override void AppendRegexClause(string fieldName, string value,
        StringBuilder sb)
    {
        sb.Append(fieldName)
          .Append(" ~ ")
          .AppendLine(SQE(value, false, true, false));
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
        // https://www.freecodecamp.org/news/fuzzy-string-matching-with-postgresql
        // requires CREATE EXTENSION pg_trgm on the database
        sb.Append("SIMILARITY(")
          .Append(fieldName)
          .Append(", ")
          .Append(SQE(value, false, true, false))
          .Append(") >= ")
          .AppendLine(treshold.ToString(CultureInfo.InvariantCulture));
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
        sb.AppendLine("(")
          .Append("  IF (")
          .Append(fieldName)
          .Append(" ~ '^[0-9]+$',CAST(")
          .Append(fieldName)
          .AppendLine(" AS SIGNED),NULL)")
          .Append(')')
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
        sb.Append("OFFSET ")
          .Append(options.GetSkipCount().ToString(CultureInfo.InvariantCulture))
          .Append(" ROWS FETCH NEXT ")
          .Append(options.PageSize.ToString(CultureInfo.InvariantCulture))
          .AppendLine(" ROWS ONLY");
    }
}
