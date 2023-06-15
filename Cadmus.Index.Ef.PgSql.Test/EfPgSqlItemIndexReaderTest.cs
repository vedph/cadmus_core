using Cadmus.Index.Sql.Test;
using Npgsql;
using System.Data;
using Xunit;

namespace Cadmus.Index.Ef.PgSql.Test;

public sealed class EfPgSqlItemIndexReaderTest : SqlItemIndexReaderTestBase
{
    private const string CST =
        "Server=localhost;Database={0};User Id=postgres;Password=postgres;" +
        "Include Error Detail=True";
    private const string DB_NAME = "cadmus-index-test";
    static private readonly string CS = string.Format(CST, DB_NAME);

    public EfPgSqlItemIndexReaderTest() : base(CS, false)
    {
    }

    protected override IDbConnection GetConnection() => new NpgsqlConnection(CS);

    protected override IItemIndexWriter GetWriter()
    {
        EfPgSqlItemIndexWriter writer = new();
        writer.Configure(new EfIndexRepositoryOptions
        {
            ConnectionString = CS
        });
        return writer;
    }

    protected override IItemIndexReader GetReader()
    {
        EfPgSqlItemIndexReader reader = new();
        reader.Configure(new EfIndexRepositoryOptions
        {
            ConnectionString = CS
        });
        return reader;
    }

#pragma warning disable S2699 // Tests should include assertions
    [Fact]
    public void SearchItems_ByTitleNoOp_Ok() => DoSearchItems_ByTitleNoOp_Ok();

    [Fact]
    public void SearchItems_ByTitleContains_Ok()
        => DoSearchItems_ByTitleContains_Ok();

    [Fact]
    public void SearchItems_ByTitleRegex_Ok()
        => DoSearchItems_ByTitleRegex_Ok();

    [Fact]
    public void SearchItems_ByTitleFuzzy_Ok()
    => DoSearchItems_ByTitleFuzzy_Ok();

    [Fact]
    public void SearchItems_ByDscContains_Ok()
        => DoSearchItems_ByDscContains_Ok();
#pragma warning restore  // Tests should include assertions
}
