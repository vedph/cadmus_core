using Cadmus.Index.Config;
using Xunit;

namespace Cadmus.Index.Sql.Test
{
    public sealed class ItemIndexFactoryTest
    {
        private const string MYSQL_PROFILE =
            "{ " +
            "\"index\": " +
            "{ " +
            "\"reader\": {\"id\": \"item-index-reader.mysql\"}, " +
            "\"writer\": {\"id\": \"item-index-writer.mysql\"}" +
            "}" +
            "}";
        private const string MSSQL_PROFILE =
            "{ " +
            "\"index\": " +
            "{ " +
            "\"reader\": {\"id\": \"item-index-reader.mysql\"}, " +
            "\"writer\": {\"id\": \"item-index-writer.mysql\"}" +
            "}" +
            "}";

        private readonly IItemIndexFactoryProvider _factoryProvider;

        public ItemIndexFactoryTest()
        {
            _factoryProvider = new StaticItemIndexFactoryProvider(
                "Server=localhost;Database={0};Uid=root;Pwd=mysql;");
        }

        [Fact]
        public void GetWriter_MySql_NotNull()
        {
            ItemIndexFactory factory = _factoryProvider.GetFactory(MYSQL_PROFILE);
            IItemIndexWriter? writer = factory.GetItemIndexWriter();
            Assert.NotNull(writer);
        }

        [Fact]
        public void GetWriter_MsSql_NotNull()
        {
            ItemIndexFactory factory = _factoryProvider.GetFactory(MSSQL_PROFILE);
            IItemIndexWriter? writer = factory.GetItemIndexWriter();
            Assert.NotNull(writer);
        }

        [Fact]
        public void GetReader_MsSql_NotNull()
        {
            ItemIndexFactory factory = _factoryProvider.GetFactory(MSSQL_PROFILE);
            IItemIndexReader? writer = factory.GetItemIndexReader();
            Assert.NotNull(writer);
        }
    }
}
