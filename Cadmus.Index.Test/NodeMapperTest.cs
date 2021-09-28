using Cadmus.Index.Graph;
using Cadmus.Index.MySql;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Xunit;

namespace Cadmus.Index.Test
{
    // https://github.com/xunit/xunit/issues/1999
    [CollectionDefinition(nameof(NonParallelResourceCollection),
        DisableParallelization = true)]
    public class NonParallelResourceCollection { }
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class NodeMapperTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
        private const string DB_NAME = "cadmus-index-test";
        static private readonly string CS = string.Format(CST, DB_NAME);

        // private static IDbConnection GetConnection() => new MySqlConnection(CS);

        private static void Reset()
        {
            IDbManager manager = new MySqlDbManager(CST);
            if (manager.Exists(DB_NAME))
            {
                manager.ClearDatabase(DB_NAME);
            }
            else
            {
                manager.CreateDatabase(DB_NAME,
                    MySqlItemIndexWriter.GetMySqlSchema(), null);
            }
        }

        private static IGraphRepository GetRepository()
        {
            MySqlGraphRepository repository =
                new MySqlGraphRepository(new MySqlTokenHelper());
            repository.Configure(new Sql.SqlOptions
            {
                ConnectionString = CS
            });
            return repository;
        }

        #region ParseItemTitle
        [Fact]
        public void ParseItemTitle_Simple_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Null(tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithUid_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [#x:the_uid]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Null(tpu.Item2);
            Assert.Equal("x:the_uid", tpu.Item3);
        }

        [Fact]
        public void ParseItemTitle_WithPrefix_Ok()
        {
            var tpu = NodeMapper.ParseItemTitle("A simple title [@x:artists/]");
            Assert.Equal("A simple title", tpu.Item1);
            Assert.Equal("x:artists/", tpu.Item2);
            Assert.Null(tpu.Item3);
        }
        #endregion

        #region ParsePinName
        [Fact]
        public void ParsePinName_Simple_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("name");

            Assert.Single(comps);
            Assert.Equal("name", comps[0]);
        }

        [Fact]
        public void ParsePinName_Composite1_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha");

            Assert.Equal(2, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
        }

        [Fact]
        public void ParsePinName_Composite2_Ok()
        {
            string[] comps = NodeMapper.ParsePinName("eid@alpha@beta");

            Assert.Equal(3, comps.Length);
            Assert.Equal("eid", comps[0]);
            Assert.Equal("alpha", comps[1]);
            Assert.Equal("beta", comps[2]);
        }
        #endregion

        #region MapItem
        private static void AddItemRules(IGraphRepository repository)
        {
            // TODO
        }

        [Fact]
        public void MapItem_ItemFacetGroupClass_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddItemRules(repository);

            // TODO
        }
        #endregion
    }
}
