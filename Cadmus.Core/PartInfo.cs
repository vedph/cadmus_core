﻿using System;

namespace Cadmus.Core;

/// <summary>
/// Summary information about any type of part.
/// </summary>
public class PartInfo : IHasVersion
{
    /// <summary>
    /// Gets or sets the part identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the item the parts belongs to.
    /// </summary>
    public string? ItemId { get; set; }

    /// <summary>
    /// Gets or sets the part's type identifier.
    /// </summary>
    public string? TypeId { get; set; }

    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    /// <remarks>Parts of the same type in the same item can have different
    /// roles. For instance, two date parts may refer to the date of the original
    /// text and to that of its later copy. In this case, a role ID helps
    /// selecting the desired part from an item.</remarks>
    public string? RoleId { get; set; }

    /// <summary>
    /// Gets or sets the thesaurus scope. This is an arbitrary string
    /// representing the suffix to be appended to a thesaurus ID (just
    /// before the <c>@</c> introducing its language ID) when loading
    /// thesauri. The suffix is appended with a leading dot, and should
    /// include only letters A-Z or a-z, digits, dash, and underscore.
    /// </summary>
    /// <remarks>This scope is used for those parts which require to
    /// load different thesauri according to runtime data. For instance,
    /// an apparatus part might require to load a different set of witnesses
    /// according to the work it refers to. Thus, an editor could append
    /// the part's thesaurus scope to its required thesaurus ID before
    /// loading it. For instance, say the part wants to load a thesaurus
    /// with ID <c>witnesses@en</c>: if the part has its
    /// <see cref="ThesaurusScope"/> equal to <c>lucr</c>, the editor will
    /// rather load <c>witnesses.lucr@en</c>.
    /// </remarks>
    public string? ThesaurusScope { get; set; }

    /// <summary>
    /// Creation date and time (UTC).
    /// </summary>
    public DateTime TimeCreated { get; set; }

    /// <summary>
    /// ID of the user who created the resource.
    /// </summary>
    public string CreatorId { get; set; }

    /// <summary>
    /// Last saved date and time (UTC).
    /// </summary>
    public DateTime TimeModified { get; set; }

    /// <summary>
    /// ID of the user who last saved the resource.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartInfo"/> class.
    /// </summary>
    public PartInfo()
    {
        CreatorId = UserId = "";
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{ItemId}.{Id} {TypeId}" +
            (RoleId != null? $" ({RoleId})" : "" );
    }
}
