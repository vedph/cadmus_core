namespace Cadmus.Graph.Ef;

public class EfNode : Node
{
    public EfUriEntry? UriEntry { get; set; }
    public EfProperty? Property { get; set; }
    public List<EfNodeClass>? Classes { get; set; }
    public List<EfTriple>? SubjectTriples { get; set; }
    public List<EfTriple>? PredicateTriples { get; set; }
    public List<EfTriple>? ObjectTriples { get; set; }

    public EfNode()
    {
    }

    public EfNode(Node node)
    {
        Id = node.Id;
        IsClass = node.IsClass;
        Tag = node.Tag;
        Label = node.Label;
        SourceType = node.SourceType;
        Sid = node.Sid;
    }

    public UriNode ToUriNode(string? uri = null)
    {
        return new UriNode
        {
            Id = Id,
            IsClass = IsClass,
            Tag = Tag,
            Label = Label,
            SourceType = SourceType,
            Sid = Sid,
            Uri = uri ?? UriEntry?.Uri
        };
    }
}
