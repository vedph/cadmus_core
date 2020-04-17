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
                $"`item`.`{field}`='hello'\r\n" +
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
                $"`item`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 40\r\n";
            Assert.Equal(expected, sql);
        }

        [Theory]
        [InlineData("partTypeId")]
        [InlineData("roleId")]
        [InlineData("name")]
        [InlineData("value")]
        public void Build_SinglePinField_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, $"[{field}=hello]");

            string expected = SQL_HEAD +
                $"`pin`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Theory]
        [InlineData("partTypeId")]
        [InlineData("roleId")]
        [InlineData("name")]
        [InlineData("value")]
        public void Build_SinglePinFieldPage3_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 3,
                PageSize = 20
            }, $"[{field}=hello]");

            string expected = SQL_HEAD +
                $"`pin`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 40\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_Equal_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title=hello]");

            const string expected = SQL_HEAD +
                "`item`.`title`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_NotEqual_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title<>hello]");

            const string expected = SQL_HEAD +
                "`item`.`title`<>'hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_Contains_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title*=hello]");

            const string expected = SQL_HEAD +
                "`item`.`title` LIKE '%hello%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_StartsWith_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title^=hello]");

            const string expected = SQL_HEAD +
                "`item`.`title` LIKE 'hello%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_EndsWith_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title$=hello]");

            const string expected = SQL_HEAD +
                "`item`.`title` LIKE '%hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_Wildcards_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title?=h?ll*]");

            const string expected = SQL_HEAD +
                "`item`.`title` LIKE 'h_ll%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_Regex_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title~=h\\[ea\\]llo]");

            const string expected = SQL_HEAD +
                "`item`.`title` REGEXP 'h[ea]llo'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_Fuzzy_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[title%=hello:90]");

            const string expected = SQL_HEAD +
                "(SELECT SIMILARITY_STRING(`item`.`title`, 'hello')>=90)\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Theory]
        [InlineData("==", "=")]
        [InlineData("!=", "<>")]
        [InlineData("<", "<")]
        [InlineData(">", ">")]
        [InlineData("<=", "<=")]
        [InlineData(">=", ">=")]
        public void Build_NumericEqual_Ok(string inOp, string outOp)
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, $"[title{inOp}12]");

            string expected = SQL_HEAD +
                "(\r\n  IF (`item`.`title` REGEXP '^[0-9]+$'," +
                $"CAST(`item`.`title` AS SIGNED),NULL)\r\n){outOp}12\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_FlagsAnyOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[flags:review,todo]");

            const string expected = SQL_HEAD +
                "(`item`.`flags` & 3) <> 0\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_FlagsAllOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[flags&:review,todo]");

            const string expected = SQL_HEAD +
                "(`item`.`flags` & 3) = 3\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }

        [Fact]
        public void Build_FlagsNotAnyOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            string sql = builder.Build(new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            }, "[flags!:review,todo]");

            const string expected = SQL_HEAD +
                "(`item`.`flags` & 3) = 0\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql);
        }
    }
}
