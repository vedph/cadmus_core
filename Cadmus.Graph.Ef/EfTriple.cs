namespace Cadmus.Graph.Ef;

/// <summary>
/// Entity Framework triple entity.
/// </summary>
/// <seealso cref="Triple" />
public class EfTriple : Triple
{
    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    public EfNode? Subject { get; set; }
    /// <summary>
    /// Gets or sets the predicate.
    /// </summary>
    public EfNode? Predicate { get; set; }
    /// <summary>
    /// Gets or sets the object.
    /// </summary>
    public EfNode? Object { get; set; }

    public EfTriple()
    {
    }

    public EfTriple(Triple triple)
    {
        ArgumentNullException.ThrowIfNull(triple);

        Id = triple.Id;
        SubjectId = triple.SubjectId;
        PredicateId = triple.PredicateId;
        ObjectId = triple.ObjectId == 0? null : triple.ObjectId;
        ObjectLiteral = triple.ObjectLiteral;
        ObjectLiteralIx = triple.ObjectLiteralIx;
        LiteralType = triple.LiteralType;
        LiteralLanguage = triple.LiteralLanguage;
        LiteralNumber = triple.LiteralNumber;
        Sid = triple.Sid;
        Tag = triple.Tag;
    }

    public UriTriple ToUriTriple()
    {
        return new UriTriple
        {
            Id = Id,
            SubjectId = SubjectId,
            PredicateId = PredicateId,
            ObjectId = ObjectId == 0? null : ObjectId,
            ObjectLiteral = ObjectLiteral,
            ObjectLiteralIx = ObjectLiteralIx,
            LiteralType = LiteralType,
            LiteralLanguage = LiteralLanguage,
            LiteralNumber = LiteralNumber,
            Sid = Sid,
            Tag = Tag,
            SubjectUri = Subject?.UriEntry?.Uri,
            PredicateUri = Predicate?.UriEntry?.Uri,
            ObjectUri = Object?.UriEntry?.Uri
        };
    }
}
