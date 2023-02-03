using System.Collections.Generic;

namespace Cadmus.Core.Config;

internal sealed class ThesaurusOptions
{
    public string? Id { get; set; }
    public string? TargetId { get; set; }
    public IList<ThesaurusEntryOptions>? Entries { get; set; }
}

internal sealed class ThesaurusEntryOptions
{
    public string? Id { get; set; }
    public string? Value { get; set; }
}
