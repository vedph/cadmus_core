using Cadmus.Core;
using System.Collections.Generic;

namespace Cadmus.Graph.Adapters;

/// <summary>
/// Graph source adapter for <see cref="IItem"/>'s.
/// </summary>
/// <seealso cref="JsonGraphSourceAdapter" />
/// <seealso cref="IGraphSourceAdapter" />
public sealed class ItemGraphSourceAdapter : JsonGraphSourceAdapter,
    IGraphSourceAdapter
{
    public const string M_ITEM_ID = "item-id";
    public const string M_ITEM_TITLE = "item-title";
    public const string M_ITEM_FACET = "item-facet";
    public const string M_ITEM_GROUP = "item-group";

    internal static void ExtractItemMetadata(GraphSource source,
        RunNodeMappingFilter filter, IDictionary<string, object> metadata)
    {
        IItem? item = source.Item;
        if (item == null) return;

        // filter
        filter.SourceType = Node.SOURCE_ITEM;
        filter.Facet = item.FacetId;
        filter.Group = item.GroupId;
        filter.Flags = item.Flags;
        filter.Title = item.Title;

        // metadata
        metadata[M_ITEM_ID] = item.Id;
        metadata[M_ITEM_TITLE] = item.Title;
        metadata[M_ITEM_FACET] = item.FacetId;

        if (!string.IsNullOrEmpty(item.GroupId))
        {
            metadata[M_ITEM_GROUP] = item.GroupId;
            if (item.GroupId.IndexOf('/') > -1)
            {
                int n = 0;
                foreach (string g in item.GroupId.Split('/'))
                    metadata[$"{M_ITEM_GROUP}@{++n}"] = g;
            }
        }
    }

    /// <summary>
    /// Adapt the source to the mapping process, eventually also setting
    /// <paramref name="filter" /> and <paramref name="metadata" />
    /// accordingly.
    /// </summary>
    /// <param name="source">The source with its item.</param>
    /// <param name="filter">The filter to set.</param>
    /// <param name="metadata">The metadata to set.</param>
    /// <returns>
    /// Adapted object or null.
    /// </returns>
    protected override object? Adapt(GraphSource source,
        RunNodeMappingFilter filter, IDictionary<string, object> metadata)
    {
        ExtractItemMetadata(source, filter, metadata);
        return source.Item;
    }
}
