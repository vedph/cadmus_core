using Fusi.Tools.Data;
using System.Globalization;
using System.Text;

namespace Cadmus.Index.Sql
{
    /// <summary>
    /// SQL Server query builder.
    /// </summary>
    /// <seealso cref="SqlQueryBuilderBase" />
    public sealed class MsSqlQueryBuilder : SqlQueryBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlQueryBuilder"/>
        /// class.
        /// </summary>
        public MsSqlQueryBuilder(): base(new MsSqlTokenHelper())
        {
        }

        /// <summary>
        /// Appends the regex clause.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected override void AppendRegexClause(string fieldName, string value,
            StringBuilder sb)
        {
            sb.Append("dbo.RegexIsMatch(")
              .Append(fieldName)
              .Append(", ")
              .Append(SQE(value, false, true))
              .AppendLine(", NULL)=1");
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
            sb.Append("dbo.Similarity(")
                .Append(fieldName).Append(", ")
                .Append(SQE(value, false, true))
                .Append(")>=")
                .AppendLine(treshold.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Appends the numeric pair SQL.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="op">The operator.</param>
        /// <param name="value">The value.</param>
        /// <param name="sb">The string builder to append to.</param>
        protected override void AppendNumericPairSql(string fieldName, string op,
            string value, StringBuilder sb)
        {
            sb.Append("IIF(ISNUMERIC(")
              .Append(fieldName)
              .Append(")=1, CAST(")
              .Append(fieldName)
              .Append(" AS INT), NULL)")
              .Append(op)
              .AppendLine(value);
        }

        /// <summary>
        /// Appends the paging instructions.
        /// </summary>
        /// <param name="options">The paging options.</param>
        /// <param name="sb">The target string builder.</param>
        protected override void AppendPaging(PagingOptions options,
            StringBuilder sb)
        {
            sb.Append("OFFSET ")
                .Append(options.GetSkipCount())
                .AppendLine(" ROWS")
                .Append("FETCH NEXT ")
                .Append(options.PageSize)
                .AppendLine(" ROWS ONLY");
        }
    }
}
