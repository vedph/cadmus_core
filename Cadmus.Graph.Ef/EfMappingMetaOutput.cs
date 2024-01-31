namespace Cadmus.Graph.Ef;

public class EfMappingMetaOutput
{
    public int Id { get; set; }
    public int MappingId { get; set; }
    public int Ordinal { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }

    public EfMapping? Mapping { get; set; }

    public EfMappingMetaOutput()
    {
        Name = "";
        Value = "";
    }

    public override string ToString()
    {
        return $"#{MappingId}.{Id}: {Name}={(Value.Length > 100 ? Value[..100] : Value)}";
    }
}
