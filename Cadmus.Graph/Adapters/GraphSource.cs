using Cadmus.Core;
using System;

namespace Cadmus.Graph.Adapters;

/// <summary>
/// Source data for graph mapping. This can be either an item, or an
/// item's part, or a thesaurus.
/// </summary>
public class GraphSource
{
    /// <summary>
    /// Gets the item.
    /// </summary>
    public IItem Item { get; }

    /// <summary>
    /// Gets the part.
    /// </summary>
    public IPart? Part { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphSource"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">item</exception>
    public GraphSource(IItem item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphSource"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="part">The part.</param>
    /// <exception cref="ArgumentNullException">item or part</exception>
    public GraphSource(IItem item, IPart part)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Part = part ?? throw new ArgumentNullException(nameof(part));
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        if (Part != null) return Part.ToString()!;
        return Item!.ToString()!;
    }
}
