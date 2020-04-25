using Cadmus.Core.Config;
using Fusi.Tools.Data;
using Xunit;

namespace Cadmus.Index.Sql.Test
{
    public sealed class MySqlQueryBuilderTest
    {
        private const string SQL_PAG_HEAD =
            "SELECT DISTINCT\r\n" +
            "`item`.`id`,`item`.`title`,`item`.`description`,`item`.`facetId`," +
            "`item`.`groupId`,`item`.`sortKey`,`item`.`flags`," +
            "`item`.`timeCreated`,`item`.`creatorId`," +
            "`item`.`timeModified`,`item`.`userId`\r\n" +
            "FROM `item`\r\n" +
            "INNER JOIN `pin`\r\n" +
            "ON `item`.`id`=`pin`.`itemId`\r\n" +
            "WHERE\r\n";
        private const string SQL_TOT_HEAD =
            "SELECT COUNT(DISTINCT `item`.`id`)\r\n" +
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

            var sql = builder.Build($"[{field}=hello]", new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

            string expected = SQL_PAG_HEAD +
                $"`item`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                $"`item`.`{field}`='hello'\r\n";
            Assert.Equal(expected, sql.Item2);
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

            var sql = builder.Build($"[{field}=hello]",
                new PagingOptions
                {
                    PageNumber = 3,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                $"`item`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 40\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                $"`item`.`{field}`='hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Theory]
        [InlineData("partTypeId")]
        [InlineData("roleId")]
        [InlineData("name")]
        [InlineData("value")]
        public void Build_SinglePinField_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build($"[{field}=hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                $"`pin`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                $"`pin`.`{field}`='hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Theory]
        [InlineData("partTypeId")]
        [InlineData("roleId")]
        [InlineData("name")]
        [InlineData("value")]
        public void Build_SinglePinFieldPage3_Ok(string field)
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build($"[{field}=hello]",
                new PagingOptions
                {
                    PageNumber = 3,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                $"`pin`.`{field}`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 40\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                $"`pin`.`{field}`='hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_Equal_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title=hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title`='hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title`='hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_NotEqual_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title<>hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title`<>'hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title`<>'hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_Contains_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title*=hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title` LIKE '%hello%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title` LIKE '%hello%'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_StartsWith_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title^=hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title` LIKE 'hello%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title` LIKE 'hello%'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_EndsWith_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title$=hello]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title` LIKE '%hello'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title` LIKE '%hello'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_Wildcards_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title?=h?ll*]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title` LIKE 'h_ll%'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title` LIKE 'h_ll%'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_Regex_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title~=h\\[ea\\]llo]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "`item`.`title` REGEXP 'h[ea]llo'\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "`item`.`title` REGEXP 'h[ea]llo'\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_Fuzzy_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[title%=hello:90]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "(SELECT SIMILARITY_STRING(`item`.`title`, 'hello')>=90)\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "(SELECT SIMILARITY_STRING(`item`.`title`, 'hello')>=90)\r\n";
            Assert.Equal(expected, sql.Item2);
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

            var sql = builder.Build($"[title{inOp}12]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "(\r\n  IF (`item`.`title` REGEXP '^[0-9]+$'," +
                $"CAST(`item`.`title` AS SIGNED),NULL)\r\n){outOp}12\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "(\r\n  IF (`item`.`title` REGEXP '^[0-9]+$'," +
                $"CAST(`item`.`title` AS SIGNED),NULL)\r\n){outOp}12\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_FlagsAnyOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[flags:review,todo]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "(`item`.`flags` & 3) <> 0\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "(`item`.`flags` & 3) <> 0\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_FlagsAllOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[flags&:review,todo]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "(`item`.`flags` & 3) = 3\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "(`item`.`flags` & 3) = 3\r\n";
            Assert.Equal(expected, sql.Item2);
        }

        [Fact]
        public void Build_FlagsNotAnyOf_Ok()
        {
            MySqlQueryBuilder builder = GetBuilder();

            var sql = builder.Build("[flags!:review,todo]",
                new PagingOptions
                {
                    PageNumber = 1,
                    PageSize = 20
                });

            string expected = SQL_PAG_HEAD +
                "(`item`.`flags` & 3) = 0\r\n" +
                SQL_ORDER +
                "\r\nLIMIT 20\r\nOFFSET 0\r\n";
            Assert.Equal(expected, sql.Item1);

            expected = SQL_TOT_HEAD +
                "(`item`.`flags` & 3) = 0\r\n";
            Assert.Equal(expected, sql.Item2);
        }
    }
}
