namespace Cadmus.Core.Config
{
    internal sealed class DataProfileOptions
    {
        public FacetDefinition[] Facets { get; set; }
        public FlagDefinition[] Flags { get; set; }
        public ThesaurusOptions[] Thesauri { get; set; }
    }
}
