namespace Cadmus.Graph.Ef;

public class EfUidEntry
{
    public int Id { get; set; }
    public string Sid { get; set; }
    public string Unsuffixed { get; set; }
    public bool HasSuffix { get; set; }

    public EfUidEntry()
    {
        Sid = "";
        Unsuffixed = "";
    }

    public override string ToString()
    {
        return $"#{Id} {Unsuffixed}{(HasSuffix ? "*" : "")}";
    }
}
