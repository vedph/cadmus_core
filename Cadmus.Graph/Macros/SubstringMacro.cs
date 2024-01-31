using Cadmus.Graph.Adapters;
using Fusi.Tools.Configuration;
using System;

namespace Cadmus.Graph.Macros;

/// <summary>
/// Substring macro. This returns a substring of the received string.
/// Tag: <c>node-mapping-macro.substring</c>.
/// </summary>
/// <seealso cref="INodeMappingMacro" />
[Tag("node-mapping-macro.substring")]
public sealed class SubstringMacro : INodeMappingMacro
{
    /// <summary>
    /// Run the macro function.
    /// </summary>
    /// <param name="context">The data context of the macro function.</param>
    /// <param name="args">The arguments: 0=the string to substring,
    /// 1=the start index, 2=the length (optional).</param>
    /// <returns>Result or null.</returns>
    /// <exception cref="ArgumentNullException">template</exception>
    public string? Run(GraphSource? context, string[]? args)
    {
        if (args == null || args.Length == 0) return null;

        string s = args[0];
        if (s == null) return null;

        if (args.Length > 2)
        {
            int start = int.Parse(args[1]);
            int length = int.Parse(args[2]);
            return s.Substring(start, length);
        }
        else if (args.Length > 1)
        {
            int start = int.Parse(args[1]);
            return s[start..];
        }
        else return s;
    }
}
