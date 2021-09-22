using Cadmus.Index.Graph;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Fusi.Tools.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Xunit;

namespace Cadmus.Index.MySql.Test
{
    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class MySqlGraphRepositoryTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
        private const string DB_NAME = "cadmus-index-test";
        static private readonly string CS = string.Format(CST, DB_NAME);

        private static IDbConnection GetConnection() => new MySqlConnection(CS);

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

        private static void AddNamespaces(int count, IGraphRepository repository)
        {
            for (int i = 0; i < count; i++)
                repository.AddNamespace("p" + i, $"http://www.ns{i}.org");
        }

        [Fact]
        public void GetNamespaces_Ok()
        {
            Reset();
            IGraphRepository repository = GetRepository();
            AddNamespaces(3, repository);

            DataPage<NamespaceEntry> page = repository.GetNamespaces(
                new NamespaceFilter
                {
                    PageNumber = 1,
                    PageSize = 10,
                });

            Assert.Equal(3, page.Total);
        }
    }
}
