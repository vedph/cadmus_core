using System.Text;
using Xunit;

namespace Cadmus.Graph.Test;

public sealed class TemplateTreeTest
{
    private static string MockResolver(TemplateNode node)
    {
        if (node.Children.Count == 0) return "";

        StringBuilder sb = new();
        foreach (TemplateNode child in node.Children)
            sb.Append(child.Value ?? "");
        return sb.ToString().ToLowerInvariant();
    }

    [Fact]
    public void Create_Ok()
    {
        // -R
        // --A
        // --X
        // ---BC
        // ---X
        // ----D
        // --E
        // --X
        // ---F
        // --G
        const string template = "A{$BC{$D}}E{$F}G";
        TemplateTree tree = TemplateTree.Create(template);

        TemplateNode root = tree.Root;

        // R/ A,X,E,X,G
        Assert.Equal(5, root.ChildrenCount);
        Assert.Equal("A", root.Children[0].Value);
        Assert.Null(root.Children[1].Value);
        Assert.Equal("E", root.Children[2].Value);
        Assert.Null(root.Children[3].Value);
        Assert.Equal("G", root.Children[4].Value);

        // R/X/ BC,X
        TemplateNode node = root.Children[1];
        Assert.Equal(2, node.ChildrenCount);
        Assert.Equal("BC", node.Children[0].Value);
        Assert.Null(node.Children[1].Value);

        // R/X/X/ D
        node = node.Children[1];
        Assert.Equal(1, node.ChildrenCount);
        Assert.Equal("D", node.Children[0].Value);

        // R/X/ F
        node = root.Children[3];
        Assert.Equal(1, node.ChildrenCount);
        Assert.Equal("F", node.Children[0].Value);
    }

    [Fact]
    public void Resolve_Ok()
    {
        const string template = "A{$BC{$D}}E{$F}G";
        TemplateTree tree = TemplateTree.Create(template);

        string result = tree.Resolve(MockResolver);

        Assert.Equal("AbcdEfG", result);
    }
}
