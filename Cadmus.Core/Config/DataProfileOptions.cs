namespace Cadmus.Core.Config
{
    internal sealed class DataProfileOptions
    {
        public FacetDefinition[] Facets { get; set; }
        public FlagDefinition[] Flags { get; set; }
        public ThesaurusOptions[] Thesauri { get; set; }
        public DataPinFilter GraphPinFilter { get; set; }
        public DataPinFilter NonGraphPinFilter { get; set; }
    }
}
