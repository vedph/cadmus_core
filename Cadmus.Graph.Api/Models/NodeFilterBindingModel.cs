using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Graph.Api.Models;

public class NodeFilterBindingModel : PagingBindingModel
{
    /// <summary>
    /// Any portion of the node's UID to match.
    /// </summary>
    [MaxLength(500)]
    public string? Uid { get; set; }

    /// <summary>
    /// Gets or sets the class filter to match.
    /// </summary>
    public bool? IsClass { get; set; }

    /// <summary>
    /// Gets or sets the tag filter to match. If null, no tag filtering
    /// is applied; if empty, only nodes with a null tag are matched;
    /// otherwise, the nodes with the same tag must be matched.
    /// </summary>
    [MaxLength(50)]
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets any portion of the label to match.
    /// </summary>
    [MaxLength(500)]
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the type of the source.
    /// </summary>
    public int? SourceType { get; set; }

    /// <summary>
    /// Gets or sets the sid.
    /// </summary>
    [MaxLength(500)]
    public string? Sid { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Sid"/> represents
    /// the initial portion of the SID being searched, rather than the
    /// full SID.
    /// </summary>
    public bool IsSidPrefix { get; set; }

    /// <summary>
    /// Gets or sets the classes identifiers to match only those nodes
    /// which are inside any of the listed classes.
    /// </summary>
    public List<int>? ClassIds { get; set; }

    /// <summary>
    /// Gets or sets the identifier of a node which is directly linked
    /// to the nodes being searched.
    /// </summary>
    public int LinkedNodeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the role of the node identified by
    /// <see cref="LinkedNodeId"/>: <c>S</c>=subject, <c>O</c>=object,
    /// else no role filtering.
    /// </summary>
    public char LinkedNodeRole { get; set; }

    /// <summary>
    /// Converts this into a <see cref="NodeFilter"/>.
    /// </summary>
    /// <returns>The filter.</returns>
    public NodeFilter ToNodeFilter()
    {
        return new NodeFilter
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            Uid = Uid,
            IsClass = IsClass,
            Tag = Tag,
            Label = Label,
            SourceType = SourceType,
            Sid = Sid,
            IsSidPrefix = IsSidPrefix,
            ClassIds = ClassIds,
            LinkedNodeId = LinkedNodeId,
            LinkedNodeRole = LinkedNodeRole,
        };
    }
}
