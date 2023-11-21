using Cadmus.Core;
using System;
using System.Collections.Generic;

namespace Cadmus.Index.Ef;

/// <summary>
/// Index item entity.
/// </summary>
public class EfIndexItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FacetId { get; set; }
    public string? GroupId { get; set; }
    public string SortKey { get; set; }
    public int Flags { get; set; }
    public DateTime TimeCreated { get; set; }
    public string? CreatorId { get; set; }
    public DateTime TimeModified { get; set; }
    public string? UserId { get; set; }

    public List<EfIndexPin>? Pins { get; set; }

    public EfIndexItem()
    {
        Id = "";
        Title = "";
        Description = "";
        FacetId = "";
        SortKey = "";
    }

    public EfIndexItem(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        Title = item.Title;
        Description = item.Description;
        FacetId = item.FacetId;
        GroupId = item.GroupId;
        SortKey = item.SortKey;
        Flags = item.Flags;
        TimeCreated = item.TimeCreated;
        CreatorId = item.CreatorId;
        TimeModified = item.TimeModified;
        UserId = item.UserId;
    }

    public override string ToString()
    {
        return $"{Id}: {Title} ({FacetId})";
    }
}
