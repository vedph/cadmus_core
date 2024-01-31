namespace Cadmus.Graph.Ef;

public class EfMappingNodeOutput
{
    public int Id { get; set; }
    public int MappingId { get; set; }
    public int Ordinal { get; set; }
    public string Name { get; set; }
    public string Uid { get; set; }
    public string? Label { get; set; }
    public string? Tag { get; set; }

    public EfMapping? Mapping { get; set; }

    public EfMappingNodeOutput()
    {
        Name = "";
        Uid = "";
    }

    public override string ToString()
    {
        return $"#{MappingId}.{Id}: {Name}";
    }
}
