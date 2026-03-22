using Cadmus.Index.Config;
using Xunit;

namespace Cadmus.Index.Sql.Test;

public sealed class ItemIndexFactoryTest
{
    private const string PGSQL_PROFILE =
        "{ " +
        "\"index\": " +
        "{ " +
        "\"reader\": {\"id\": \"item-index-reader.ef-pg\"}, " +
        "\"writer\": {\"id\": \"item-index-writer.ef-pg\"}" +
        "}" +
        "}";

    private readonly StaticItemIndexFactoryProvider _factoryProvider;

    public ItemIndexFactoryTest()
    {
        _factoryProvider = new StaticItemIndexFactoryProvider(
            "Server=localhost;Database={0};Uid=root;Pwd=mysql;");
    }

    [Fact]
    public void GetWriter_PgSql_NotNull()
    {
        ItemIndexFactory factory = _factoryProvider.GetFactory(PGSQL_PROFILE);
        IItemIndexWriter? writer = factory.GetItemIndexWriter();
        Assert.NotNull(writer);
    }

    [Fact]
    public void GetReader_PgSql_NotNull()
    {
        ItemIndexFactory factory = _factoryProvider.GetFactory(PGSQL_PROFILE);
        IItemIndexReader? writer = factory.GetItemIndexReader();
        Assert.NotNull(writer);
    }
}
