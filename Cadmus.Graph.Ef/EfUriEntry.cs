namespace Cadmus.Graph.Ef;

public class EfUriEntry
{
    public int Id { get; set; }
    public string Uri { get; set; }

    public EfNode? Node { get; set; }

    public EfUriEntry()
    {
        Uri = "";
    }

    public override string ToString()
    {
        return $"#{Id} {Uri}";
    }
}
