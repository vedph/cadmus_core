using System.Collections.Generic;

namespace Cadmus.Core.Config;

internal sealed class DataProfileOptions
{
    public IList<FacetDefinition>? Facets { get; set; }
    public IList<FlagDefinition>? Flags { get; set; }
    public IList<ThesaurusOptions>? Thesauri { get; set; }
}
