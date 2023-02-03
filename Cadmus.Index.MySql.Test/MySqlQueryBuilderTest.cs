using Cadmus.Core.Config;
using Fusi.Tools.Data;
using Xunit;

namespace Cadmus.Index.MySql.Test;

public sealed class MySqlQueryBuilderTest
{
    private const string SQL_ITEM_PAG_HEAD =
        "SELECT DISTINCT\r\n" +
        "`item`.`id`,`item`.`title`,`item`.`description`,`item`.`facetId`," +
        "`item`.`groupId`,`item`.`sortKey`,`item`.`flags`," +
        "`item`.`timeCreated`,`item`.`creatorId`," +
        "`item`.`timeModified`,`item`.`userId`\r\n" +
        "FROM `item`\r\n" +
        "INNER JOIN `pin`\r\n" +
        "ON `item`.`id`=`pin`.`itemId`\r\n" +
        "WHERE\r\n";
    private const string SQL_ITEM_TOT_HEAD =
        "SELECT COUNT(DISTINCT `item`.`id`)\r\n" +
        "FROM `item`\r\n" +
        "INNER JOIN `pin`\r\n" +
        "ON `item`.`id`=`pin`.`itemId`\r\n" +
        "WHERE\r\n";
    private const string SQL_ITEM_ORDER = "ORDER BY `item`.`sortKey`,`item`.`id`";

    private const string SQL_PIN_PAG_HEAD =
        "SELECT DISTINCT\r\n" +
        "`pin`.`id`,`pin`.`itemId`,`pin`.`partId`,`pin`.`partTypeId`,`pin`.`roleId`," +
        "`pin`.`name`,`pin`.`value`\r\n" +
        "FROM `pin`\r\n" +
        "INNER JOIN `item`\r\n" +
        "ON `pin`.`itemId`=`item`.`id`\r\n" +
        "WHERE\r\n";
    private const string SQL_PIN_TOT_HEAD =
        "SELECT COUNT(*) FROM (SELECT DISTINCT\r\n" +
        "`pin`.`id`,`pin`.`itemId`,`pin`.`partId`," +
        "`pin`.`partTypeId`,`pin`.`roleId`,`pin`.`name`,`pin`.`value`\r\n" +
        "FROM `pin`\r\n" +
        "INNER JOIN `item`\r\n" +
        "ON `pin`.`itemId`=`item`.`id`\r\n" +
        "WHERE\r\n";
    private const string SQL_PIN_ORDER =
        "ORDER BY `pin`.`name`,`pin`.`value`,`pin`.`id`";

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

    [Fact]
    public void BuildForItem_NoSquares_Title()
    {
        MySqlQueryBuilder builder = GetBuilder();
        var sql = builder.BuildForItem("hello", new PagingOptions
        {
            PageNumber = 1,
            PageSize = 20
        });

        string clause = "`item`.`title`='hello'\r\n";

        string expected = SQL_ITEM_PAG_HEAD +
            clause +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD + clause;
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_CustomField_Name()
    {
        MySqlQueryBuilder builder = GetBuilder();
        var sql = builder.BuildForItem("[category=geography]", new PagingOptions
        {
            PageNumber = 1,
            PageSize = 20
        });

        const string clause =
            "(\r\n`pin`.`name`='category' AND\r\n`pin`.`value`='geography'\r\n)\r\n";

        string expected = SQL_ITEM_PAG_HEAD +
            clause +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD + clause;
        Assert.Equal(expected, sql.Item2);
    }

    [Theory]
    [InlineData("title")]
    [InlineData("description")]
    [InlineData("facetId")]
    [InlineData("groupId")]
    [InlineData("sortKey")]
    public void BuildForItem_SingleItemField_Ok(string field)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem($"[{field}=hello]", new PagingOptions
        {
            PageNumber = 1,
            PageSize = 20
        });

        string expected = SQL_ITEM_PAG_HEAD +
            $"`item`.`{field}`='hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            $"`item`.`{field}`='hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Theory]
    [InlineData("title")]
    [InlineData("description")]
    [InlineData("facetId")]
    [InlineData("groupId")]
    [InlineData("sortKey")]
    public void BuildForPin_SingleItemField_Ok(string field)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForPin($"[{field}=hello]", new PagingOptions
        {
            PageNumber = 1,
            PageSize = 20
        });

        string expected = SQL_PIN_PAG_HEAD +
            $"`item`.`{field}`='hello'\r\n" +
            SQL_PIN_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_PIN_TOT_HEAD +
            $"`item`.`{field}`='hello'\r\n) AS tmp";
        Assert.Equal(expected, sql.Item2);
    }

    [Theory]
    [InlineData("title")]
    [InlineData("description")]
    [InlineData("facetId")]
    [InlineData("groupId")]
    [InlineData("sortKey")]
    public void BuildForItem_SingleItemFieldPage3_Ok(string field)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem($"[{field}=hello]",
            new PagingOptions
            {
                PageNumber = 3,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            $"`item`.`{field}`='hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 40\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            $"`item`.`{field}`='hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Theory]
    [InlineData("partTypeId")]
    [InlineData("roleId")]
    [InlineData("name")]
    [InlineData("value")]
    public void BuildForItem_SinglePinField_Ok(string field)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem($"[{field}=hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            $"`pin`.`{field}`='hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            $"`pin`.`{field}`='hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Theory]
    [InlineData("partTypeId")]
    [InlineData("roleId")]
    [InlineData("name")]
    [InlineData("value")]
    public void BuildForItem_SinglePinFieldPage3_Ok(string field)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem($"[{field}=hello]",
            new PagingOptions
            {
                PageNumber = 3,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            $"`pin`.`{field}`='hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 40\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            $"`pin`.`{field}`='hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_Equal_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title=hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title`='hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title`='hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_NotEqual_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title<>hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title`<>'hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title`<>'hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_Contains_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title*=hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title` LIKE '%hello%'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title` LIKE '%hello%'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_StartsWith_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title^=hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title` LIKE 'hello%'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title` LIKE 'hello%'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_EndsWith_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title$=hello]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title` LIKE '%hello'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title` LIKE '%hello'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_Wildcards_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title?=h?ll*]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title` LIKE 'h_ll%'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title` LIKE 'h_ll%'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_Regex_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title~=h\\[ea\\]llo]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "`item`.`title` REGEXP 'h[ea]llo'\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "`item`.`title` REGEXP 'h[ea]llo'\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_Fuzzy_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[title%=hello:90]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "(SELECT SIMILARITY_STRING(`item`.`title`, 'hello')>=90)\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
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
    public void BuildForItem_NumericEqual_Ok(string inOp, string outOp)
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem($"[title{inOp}12]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "(\r\n  IF (`item`.`title` REGEXP '^[0-9]+$'," +
            $"CAST(`item`.`title` AS SIGNED),NULL)\r\n){outOp}12\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "(\r\n  IF (`item`.`title` REGEXP '^[0-9]+$'," +
            $"CAST(`item`.`title` AS SIGNED),NULL)\r\n){outOp}12\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_FlagsAnyOf_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[flags:review,todo]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "(`item`.`flags` & 3) <> 0\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "(`item`.`flags` & 3) <> 0\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_FlagsAllOf_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[flags&:review,todo]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "(`item`.`flags` & 3) = 3\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "(`item`.`flags` & 3) = 3\r\n";
        Assert.Equal(expected, sql.Item2);
    }

    [Fact]
    public void BuildForItem_FlagsNotAnyOf_Ok()
    {
        MySqlQueryBuilder builder = GetBuilder();

        var sql = builder.BuildForItem("[flags!:review,todo]",
            new PagingOptions
            {
                PageNumber = 1,
                PageSize = 20
            });

        string expected = SQL_ITEM_PAG_HEAD +
            "(`item`.`flags` & 3) = 0\r\n" +
            SQL_ITEM_ORDER +
            "\r\nLIMIT 20\r\nOFFSET 0\r\n";
        Assert.Equal(expected, sql.Item1);

        expected = SQL_ITEM_TOT_HEAD +
            "(`item`.`flags` & 3) = 0\r\n";
        Assert.Equal(expected, sql.Item2);
    }
}
