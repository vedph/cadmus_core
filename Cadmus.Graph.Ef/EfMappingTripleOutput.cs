namespace Cadmus.Graph.Ef;

public class EfMappingTripleOutput
{
    public int Id { get; set; }
    public int MappingId { get; set; }
    public int Ordinal { get; set; }
    public string S { get; set; }
    public string P { get; set; }
    public string? O { get; set; }
    public string? OL { get; set; }

    public EfMapping? Mapping { get; set; }

    public EfMappingTripleOutput()
    {
        S = "";
        P = "";
    }

    public override string ToString()
    {
        return $"{S} {P} {OL ?? O}";
    }
}
