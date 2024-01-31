using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Graph;

/// <summary>
/// Template tree.
/// </summary>
public sealed class TemplateTree
{
    // these chars correspond to TemplateNodeType enum
    private const string FNCHARS = "-$?@!";

    private readonly List<TemplateNode> _fnNodes;
    public TemplateNode Root { get; }

    private TemplateTree()
    {
        _fnNodes = new();
        Root = new TemplateNode();
    }

    private static void ConsumeLiteral(StringBuilder sb,
        TemplateNode parent)
    {
        if (sb.Length > 0)
        {
            parent.AddChild(new TemplateNode
            {
                Value = sb.ToString()
            });
            sb.Clear();
        }
    }

    /// <summary>
    /// Adds the function node to the list of function nodes, which is
    /// sorted by descending depth and then by insertion order.
    /// </summary>
    /// <param name="node">The node.</param>
    private void AddFnNode(TemplateNode node)
    {
        int i = 0;
        while (i < _fnNodes.Count && node.Depth <= _fnNodes[i].Depth) i++;
        _fnNodes.Insert(i, node);
    }

    /// <summary>
    /// Creates a tree from the specified template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>Tree.</returns>
    /// <exception cref="ArgumentNullException">template</exception>
    /// <exception cref="CadmusGraphException">invalid template</exception>
    public static TemplateTree Create(string template)
    {
        ArgumentNullException.ThrowIfNull(template);

        TemplateTree tree = new();
        TemplateNode node = tree.Root;

        // a literal template is a corner case, be performance-wise
        if (!template.Contains('{'))
        {
            node.AddChild(new TemplateNode
            {
                Value = template
            });
            return tree;
        }

        StringBuilder sb = new();
        int i = 0;
        int fnDepth = 0;

        while (i < template.Length)
        {
            switch (template[i])
            {
                case '\\':
                    // \{ and \} are escapes for { and }
                    if (i + 1 < template.Length &&
                        (template[i + 1] == '{' || template[i + 1] == '}'))
                    {
                        sb.Append(template[i + 1]);
                        i += 2;
                    }
                    break;

                case '{':
                    // if no next char, treat as literal
                    if (i + 1 == template.Length)
                    {
                        sb.Append('{');
                        i++;
                        break;
                    }
                    // else next char is the fn type:
                    int j = FNCHARS.IndexOf(template[i + 1]);
                    if (j == -1)
                    {
                        throw new CadmusGraphException(
                            "Invalid template placeholder type " +
                            $"'{template[i + 1]}' in \"{template}\"");
                    }
                    fnDepth++;
                    // append any pending literal and reset
                    ConsumeLiteral(sb, node);
                    // append fn node and walk down to it
                    TemplateNode fn = new()
                    {
                        Type = (TemplateNodeType)j,
                    };
                    node.AddChild(fn);
                    node = fn;
                    tree.AddFnNode(fn);
                    i += 2;
                    break;

                case '}':
                    // if no prev {, treat as literal
                    if (fnDepth == 0)
                    {
                        sb.Append('}');
                        i++;
                        break;
                    }
                    fnDepth--;
                    ConsumeLiteral(sb, node);
                    node = node.Parent!;
                    i++;
                    break;

                default:
                    sb.Append(template[i++]);
                    break;
            }
        }
        if (fnDepth > 0)
        {
            throw new CadmusGraphException("Invalid template, check braces: " +
                $"\"{template}\"");
        }
        ConsumeLiteral(sb, node);

        return tree;
    }

    /// <summary>
    /// Resolves this template.
    /// </summary>
    /// <param name="fnResolver">The function node resolver function to
    /// use. This gets a function node, and should evaluate all (and only)
    /// its direct children, returning a string result.</param>
    /// <returns>Resolved template.</returns>
    /// <exception cref="ArgumentNullException">fnResolver</exception>
    public string Resolve(Func<TemplateNode, string> fnResolver)
    {
        ArgumentNullException.ThrowIfNull(fnResolver);

        // resolve all the fn nodes from the deepest ones
        foreach (TemplateNode? fn in _fnNodes)
            fn.Value = fnResolver(fn);

        // build the output: for this we can collect only the direct
        // children of the root node, as all the fn nodes have been
        // resolved and thus flattened into the direct children
        StringBuilder sb = new();
        foreach (TemplateNode child in Root.Children!)
            sb.Append(child.Value);

        return sb.ToString();
    }
}
