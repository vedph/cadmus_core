using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Graph;

/// <summary>
/// A node used to represent literals and functions in a graph entity template.
/// </summary>
public sealed class TemplateNode
{
    private List<TemplateNode>? _children;

    /// <summary>
    /// The optional parent of this node. Null if this is the root node.
    /// </summary>
    public TemplateNode? Parent { get; private set; }

    /// <summary>
    /// The optional children of this node.
    /// </summary>
    public IList<TemplateNode> Children
    {
        get => _children ??= new List<TemplateNode>();
    }

    /// <summary>
    /// Gets the children nodes count.
    /// </summary>
    public int ChildrenCount => _children?.Count ?? 0;

    /// <summary>
    /// Gets the depth level (1=root).
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    /// The type of this node.
    /// </summary>
    public TemplateNodeType Type { get; set; }

    /// <summary>
    /// The literal value of this node. This is null if the node is a
    /// function node and has not yet been resolved.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateNode"/> class.
    /// </summary>
    public TemplateNode()
    {
        Depth = 1;
    }

    /// <summary>
    /// Adds <paramref name="node"/> as the last child of this node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <exception cref="ArgumentNullException">node</exception>
    public void AddChild(TemplateNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        node.Parent = this;
        node.Depth = Depth + 1;
        Children.Add(node);
    }

    /// <summary>
    /// Replaces this node with <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node replacing this node.</param>
    /// <exception cref="ArgumentNullException">node</exception>
    /// <exception cref="InvalidOperationException">ReplaceWith cannot be
    /// used for root node</exception>
    public void ReplaceWith(TemplateNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (Parent == null)
        {
            throw new InvalidOperationException(
                "ReplaceWith cannot be used for root node");
        }
        int i = Parent.Children!.IndexOf(this);
        Parent.Children[i] = node;
        node.Parent = Parent;
        node.Depth = Depth;
        Parent = null;
    }

    /// <summary>
    /// Gets the sibling number of this node (1-N).
    /// </summary>
    /// <returns>The sibling number.</returns>
    public int GetSiblingNumber()
    {
        return Parent == null ? 1 : Parent.Children.IndexOf(this) + 1;
    }

    /// <summary>
    /// Visit this node and all its descendant nodes.
    /// </summary>
    /// <param name="visitor">The function to be executed for each visited
    /// node. Returns true to continue, false to stop visiting.</param>
    /// <exception cref="ArgumentNullException">visitor</exception>
    public void Visit(Func<TemplateNode, bool> visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        if (!visitor(this)) return;
        if (ChildrenCount > 0)
        {
            foreach (TemplateNode child in Children) child.Visit(visitor);
        }
    }

    /// <summary>
    /// Dump this node and its descendants.
    /// </summary>
    /// <returns>Text with dump.</returns>
    public string Dump()
    {
        StringBuilder sb = new();
        Visit(new Func<TemplateNode, bool>((node) =>
        {
            sb.Append(' ', node.Depth - 1)
              .Append(node.ChildrenCount > 0 ? "+ " : "- ");
            sb.Append('[').Append(node.Depth).Append('.')
                .Append(node.GetSiblingNumber()).Append("] ");
            sb.Append(node).AppendLine();
            return true;
        }));
        return sb.ToString();
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"[{Enum.GetName(Type)}] \"{Value}\" ({Children?.Count ?? 0})";
    }
}

/// <summary>
/// The type of <see cref="TemplateNode"/>.
/// </summary>
public enum TemplateNodeType
{
    /// <summary>Literal.</summary>
    Literal = 0,
    /// <summary>Metadatum.</summary>
    Metadatum,
    /// <summary>Context node.</summary>
    Node,
    /// <summary>Source expression.</summary>
    Expression,
    /// <summary>Macro.</summary>
    Macro
}
