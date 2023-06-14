using Cadmus.Index.Sql.Test;
using Npgsql;
using System.Data;
using Xunit;

namespace Cadmus.Index.Ef.PgSql.Test;

[Collection(nameof(NonParallelResourceCollection))]
public class EfPgSqlItemIndexWriterTest : SqlItemIndexWriterTestBase
{
    private const string CST =
        "Server=localhost;Database={0};User Id=postgres;Password=postgres;" +
        "Include Error Detail=True";
    private const string DB_NAME = "cadmus-index-test";
    static private readonly string CS = string.Format(CST, DB_NAME);

    public EfPgSqlItemIndexWriterTest() : base(CS, false)
    {
    }

    #region Helpers
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
    #endregion

#pragma warning disable S2699 // Tests should include assertions
    #region WriteItem
    [Fact]
    public void WriteItem_NoParts_Ok() => DoWriteItem_NoParts_Ok();

    [Fact]
    public void WriteItem_Parts_Ok() => DoWriteItem_Parts_Ok();

    [Fact]
    public void WriteItem_ExistingWithoutParts_Ok() => DoWriteItem_ExistingWithoutParts_Ok();

    [Fact]
    public void WriteItem_ExistingWithParts_Ok() => DoWriteItem_ExistingWithParts_Ok();
    #endregion

    #region WriteItems
    [Fact]
    public void WriteItems_Ok() => DoWriteItems_Ok();
    #endregion

    #region DeleteItem
    [Fact]
    public void DeleteItem_NotExisting_Nope() => DoDeleteItem_NotExisting_Nope();

    [Fact]
    public void DeleteItem_Existing_Ok() => DoDeleteItem_Existing_Ok();
    #endregion

    #region DeletePart
    [Fact]
    public void DeletePart_NotExisting_Nope() => DoDeletePart_NotExisting_Nope();

    [Fact]
    public void DeletePart_Existing_Ok() => DoDeletePart_Existing_Ok();
    #endregion
#pragma warning restore S2699 // Tests should include assertions
}
