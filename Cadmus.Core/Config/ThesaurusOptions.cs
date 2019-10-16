namespace Cadmus.Core.Config
{
    internal sealed class ThesaurusOptions
    {
        public string Id { get; set; }
        public ThesaurusEntryOptions[] Entries { get; set; }
    }

    internal sealed class ThesaurusEntryOptions
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}
