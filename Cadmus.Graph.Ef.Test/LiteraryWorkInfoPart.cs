using Cadmus.Core;
using Cadmus.Refs.Bricks;
using Fusi.Tools.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadmus.Graph.Ef.Test;

/// <summary>
/// Essential information about a literary work.
/// <para>Tag: <c>it.vedph.itinera.literary-work-info</c>.</para>
/// </summary>
[Tag("it.vedph.itinera.literary-work-info")]
public sealed class LiteraryWorkInfoPart : PartBase
{
    /// <summary>
    /// Gets or sets the language(s) used in the work.
    /// </summary>
    public List<string> Languages { get; set; }

    /// <summary>
    /// Gets or sets the genre the work belongs to.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Gets or sets the metre(s) used in the work.
    /// </summary>
    public List<string> Metres { get; set; }

    /// <summary>
    /// Gets or sets the strophic structure(s) used in the work.
    /// </summary>
    public List<string> Strophes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this work is lost.
    /// </summary>
    public bool IsLost { get; set; }

    /// <summary>
    /// Gets or sets the author ID(s).
    /// </summary>
    public List<AssertedCompositeId> AuthorIds { get; set; }

    /// <summary>
    /// Gets or sets the work's title(s).
    /// </summary>
    public List<AssertedTitle> Titles { get; set; }

    /// <summary>
    /// Gets or sets an optional note.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteraryWorkInfoPart"/>
    /// class.
    /// </summary>
    public LiteraryWorkInfoPart()
    {
        AuthorIds = new List<AssertedCompositeId>();
        Languages = new List<string>();
        Metres = new List<string>();
        Strophes = new List<string>();
        Titles = new List<AssertedTitle>();
    }

    /// <summary>
    /// Get all the key=value pairs (pins) exposed by the implementor.
    /// </summary>
    /// <param name="item">The optional item. The item with its parts
    /// can optionally be passed to this method for those parts requiring
    /// to access further data.</param>
    /// <returns>The pins.</returns>
    public override IEnumerable<DataPin> GetDataPins(IItem? item = null)
    {
        DataPinBuilder builder = new(new StandardDataPinTextFilter());

        builder.AddValues("language", Languages);
        builder.AddValue("genre", Genre);
        builder.AddValues("metre", Metres);

        if (AuthorIds?.Count > 0)
        {
            builder.AddValues("author", AuthorIds
                .Where(id => id.Target?.Gid != null).Select(a => a.Target!.Gid));
        }

        if (Titles?.Count > 0)
        {
            builder.AddValues("title", Titles.Select(t => t.Value!),
                filter: true, filterOptions: true);
        }

        return builder.Build(this);
    }

    /// <summary>
    /// Gets the definitions of data pins used by the implementor.
    /// </summary>
    /// <returns>Data pins definitions.</returns>
    public override IList<DataPinDefinition> GetDataPinDefinitions()
    {
        return new List<DataPinDefinition>(new[]
        {
             new DataPinDefinition(DataPinValueType.String,
                "language",
                "The list of work's languages.",
                "M"),
             new DataPinDefinition(DataPinValueType.String,
                "genre",
                "The list of work's genre."),
             new DataPinDefinition(DataPinValueType.String,
                "metre",
                "The list of work's metres.",
                "M"),
             new DataPinDefinition(DataPinValueType.String,
                "author",
                "The work's author ID(s).",
                "M"),
             new DataPinDefinition(DataPinValueType.String,
                "title",
                "The list of work's titles.",
                "Mf")
        });
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append("[LiteraryWorkInfo]");

        if (AuthorIds?.Count > 0)
        {
            sb.AppendJoin(", ",
                AuthorIds.Select(id => id.Target?.Gid ?? "")).Append(" - ");
        }

        if (Titles?.Count > 0) sb.Append(Titles[0].Value);

        return sb.ToString();
    }
}
