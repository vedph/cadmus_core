namespace Cadmus.Graph.Ef;

public sealed class EfMappingLink
{
    public int ParentId { get; set; }
    public int ChildId { get; set; }
    public EfMapping? Parent { get; set; }
    public EfMapping? Child { get; set; }

    public override string ToString()
    {
        return $"#{ParentId}-{ChildId}";
    }
}
