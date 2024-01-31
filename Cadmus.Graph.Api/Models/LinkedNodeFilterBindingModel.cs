using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Graph.Api.Models;

public class LinkedNodeFilterBindingModel : PagingBindingModel
{
    /// <summary>
    /// Gets or sets any portion of the node's UID to match.
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
    /// Gets or sets the other node identifier, which is the subject node
    /// ID when <see cref="IsObject"/> is true, otherwise the object node ID.
    /// </summary>
    public int OtherNodeId { get; set; }

    /// <summary>
    /// Gets or sets the property identifier in the triple including the
    /// node to match, either as a subject or as an object (according to
    /// <see cref="IsObject"/>).
    /// </summary>
    public int PredicateId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node to match is the
    /// object (true) or the subject (false) of the triple having predicate
    /// <see cref="PredicateId"/>.
    /// </summary>
    public bool IsObject { get; set; }

    /// <summary>
    /// Convert to <see cref="NodeFilter"/>.
    /// </summary>
    /// <returns>Node filter.</returns>
    public LinkedNodeFilter ToLinkedNodeFilter()
    {
        return new LinkedNodeFilter
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
            OtherNodeId = OtherNodeId,
            PredicateId = PredicateId,
            IsObject = IsObject
        };
    }
}
