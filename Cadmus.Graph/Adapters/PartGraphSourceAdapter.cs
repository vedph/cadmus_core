using Cadmus.Core;
using System.Collections.Generic;

namespace Cadmus.Graph.Adapters;

/// <summary>
/// Graph source adapter for <see cref="IPart"/>.
/// </summary>
/// <seealso cref="JsonGraphSourceAdapter" />
/// <seealso cref="IGraphSourceAdapter" />
public sealed class PartGraphSourceAdapter : JsonGraphSourceAdapter,
    IGraphSourceAdapter
{
    public const string M_PART_ID = "part-id";
    public const string M_PART_TYPE_ID = "part-type-id";
    public const string M_PART_ROLE_ID = "part-role-id";

    /// <summary>
    /// Adapt the source to the mapping process, eventually also setting
    /// <paramref name="filter" /> and <paramref name="metadata" />
    /// accordingly.
    /// </summary>
    /// <param name="source">The source. This must be an object implementing
    /// <see cref="IPart"/>.</param>
    /// <param name="filter">The filter to set.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>
    /// Adapted object or null.
    /// </returns>
    protected override object? Adapt(GraphSource source,
        RunNodeMappingFilter filter, IDictionary<string, object> metadata)
    {
        // item
        ItemGraphSourceAdapter.ExtractItemMetadata(source, filter, metadata);

        // part
        IPart? part = source.Part;
        if (part == null) return null;

        // filter
        filter.SourceType = Node.SOURCE_PART;
        filter.PartType = part.TypeId;
        filter.PartRole = part.RoleId;

        // metadata
        metadata[M_PART_ID] = part.Id;
        metadata[ItemGraphSourceAdapter.M_ITEM_ID] = part.ItemId;
        metadata[M_PART_TYPE_ID] = part.TypeId;
        if (part.RoleId != null) metadata[M_PART_ROLE_ID] = part.RoleId;

        return part;
    }
}
