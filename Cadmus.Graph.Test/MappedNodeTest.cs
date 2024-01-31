using Xunit;

namespace Cadmus.Graph.Test;

public sealed class MappedNodeTest
{
    [Fact]
    public void Parse_Null_Null()
    {
        MappedNode? node = MappedNode.Parse(null);
        Assert.Null(node);
    }

    [Fact]
    public void Parse_Empty_Null()
    {
        MappedNode? node = MappedNode.Parse("");
        Assert.Null(node);
    }

    [Fact]
    public void Parse_Uid_Ok()
    {
        MappedNode? node = MappedNode.Parse("x:persons/guy");
        Assert.NotNull(node);
        Assert.Equal("x:persons/guy", node!.Uid);
        Assert.Null(node.Label);
        Assert.Null(node.Tag);
    }

    [Fact]
    public void Parse_UidLabel_Ok()
    {
        MappedNode? node = MappedNode.Parse("x:persons/guy [guy]");
        Assert.NotNull(node);
        Assert.Equal("x:persons/guy", node!.Uid);
        Assert.Equal("guy", node.Label);
        Assert.Null(node.Tag);
    }

    [Fact]
    public void Parse_UidTag_Ok()
    {
        MappedNode? node = MappedNode.Parse("x:persons/guy [|tag]");
        Assert.NotNull(node);
        Assert.Equal("x:persons/guy", node!.Uid);
        Assert.Null(node.Label);
        Assert.Equal("tag", node.Tag);
    }

    [Fact]
    public void Parse_UidLabelTag_Ok()
    {
        MappedNode? node = MappedNode.Parse("x:persons/guy [guy|tag]");
        Assert.NotNull(node);
        Assert.Equal("x:persons/guy", node!.Uid);
        Assert.Equal("guy", node.Label);
        Assert.Equal("tag", node.Tag);
    }

    [Fact]
    public void Parse_UidWithSquaresLabelTag_Ok()
    {
        // that's not realistic but ensures that the regex is not confused
        // by the square brackets in the UID
        MappedNode? node = MappedNode.Parse("x:persons/[guy] [guy|tag]");
        Assert.NotNull(node);
        Assert.Equal("x:persons/[guy]", node!.Uid);
        Assert.Equal("guy", node.Label);
        Assert.Equal("tag", node.Tag);
    }

    [Fact]
    public void ToString_Uid_Ok()
    {
        MappedNode node = new()
        {
            Uid = "x:persons/guy"
        };
        string text = node.ToString();
        Assert.Equal(node.Uid, text);
    }

    [Fact]
    public void ToString_UidLabel_Ok()
    {
        MappedNode node = new()
        {
            Uid = "x:persons/guy",
            Label = "guy"
        };
        string text = node.ToString();
        Assert.Equal("x:persons/guy [guy]", text);
    }

    [Fact]
    public void ToString_UidTag_Ok()
    {
        MappedNode node = new()
        {
            Uid = "x:persons/guy",
            Tag = "tag"
        };
        string text = node.ToString();
        Assert.Equal("x:persons/guy [|tag]", text);
    }

    [Fact]
    public void ToString_UidLabelTag_Ok()
    {
        MappedNode node = new()
        {
            Uid = "x:persons/guy",
            Label = "guy",
            Tag = "tag"
        };
        string text = node.ToString();
        Assert.Equal("x:persons/guy [guy|tag]", text);
    }
}
