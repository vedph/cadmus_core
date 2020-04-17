using Cadmus.Core.Config;
using Fusi.Tools.Data;
using Xunit;

namespace Cadmus.Index.Sql.Test
{
    public sealed class MySqlQueryBuilderTest
    {
        private const string SQL_HEAD =
            "SELECT DISTINCT\r\n" +
            "`item`.`id`,`item`.`title`,`item`.`description`,`item`.`facetId`," +
            "`item`.`groupId`,`item`.`sortKey`,`item`.`flags`\r\n" +
            "FROM `item`\r\n" +
            "INNER JOIN `pin`\r\n" +
            "ON `item`.`id`=`pin`.`itemId`\r\n" +
            "WHERE\r\n";
        private const string SQL_ORDER = "ORDER BY `item`.`sortKey`,`item`.`id`";

        private MySqlQueryBuilder GetBuilder()
        {
            MySqlQueryBuilder builder = new MySqlQueryBuilder();
            builder.SetFlagDefinitions(new[]
            {
                new FlagDefinition
                {
                    Id = 1,
                    Label = "review"
                },
                new FlagDefinition
                {
                    Id = 2,
                    Label = "todo"
                }
            });
            return builder;
        }

        [Theory]
        [InlineData("title")]
        [InlineData("description")]
        [InlineData("facetId")]
        [InlineData("groupId")]
        [InlineData("sortKey")]
        public void Build_SingleItemField_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, $"[{field}=hello]");

            string expected = SQL_HEAD +
                $"`item`.`{field}`=N'hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Theory]
        [InlineData("title")]
        [InlineData("description")]
        [InlineData("facetId")]
        [InlineData("groupId")]
        [InlineData("sortKey")]
        public void Build_SingleItemFieldPage3_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 3,
                PageSize = 20
            }, $"[{field}=hello]");

            string expected = SQL_HEAD +
                $"`item`.`{field}`=N'hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 40\r\n";
            Assert.Equal(expected, sql);
        }

    }
}
