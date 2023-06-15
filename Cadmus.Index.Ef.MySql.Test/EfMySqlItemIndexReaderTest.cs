using Cadmus.Index.Sql.Test;
using MySql.Data.MySqlClient;
using System.Data;
using Xunit;

namespace Cadmus.Index.Ef.MySql.Test;

[Collection(nameof(NonParallelResourceCollection))]
public sealed class EfMySqlItemIndexReaderTest : SqlItemIndexReaderTestBase
{
    private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
    private const string DB_NAME = "cadmus-index-test";
    static private readonly string CS = string.Format(CST, DB_NAME);

    public EfMySqlItemIndexReaderTest() : base(CS, false)
    {
    }

    protected override IDbConnection GetConnection() => new MySqlConnection(CS);

    protected override IItemIndexWriter GetWriter()
    {
        EfMySqlItemIndexWriter writer = new();
        writer.Configure(new EfIndexRepositoryOptions
        {
            ConnectionString = CS
        });
        return writer;
    }

    protected override IItemIndexReader GetReader()
    {
        EfMySqlItemIndexReader reader = new();
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
        => DoSearchItems_ByTitleFuzzy_Ok(0.9f, 3);

    [Fact]
    public void SearchItems_ByDscContains_Ok()
        => DoSearchItems_ByDscContains_Ok();
#pragma warning restore  // Tests should include assertions
}
