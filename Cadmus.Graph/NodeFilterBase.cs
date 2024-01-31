﻿using Fusi.Tools.Data;
using System.Collections.Generic;

namespace Cadmus.Graph;

public class NodeFilterBase : PagingOptions
{
    /// <summary>
    /// Gets or sets any portion of the node's UID to match.
    /// </summary>
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
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets any portion of the label to match.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets the type of the source.
    /// </summary>
    public int? SourceType { get; set; }

    /// <summary>
    /// Gets or sets the sid.
    /// </summary>
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
}
