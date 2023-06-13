using System;

namespace Cadmus.Index.Ef;

/// <summary>
/// Index pin entity.
/// </summary>
public class EfIndexPin
{
    public int Id { get; set; }
    public string ItemId { get; set; }
    public string PartId { get; set; }
    public string PartTypeId { get; set; }
    public string? PartRoleId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime TimeIndexed { get; set; }

    public EfIndexItem? Item { get; set; }

    public EfIndexPin()
    {
        ItemId = "";
        PartId = "";
        PartTypeId = "";
        Name = "";
        Value = "";
    }

    public override string ToString()
    {
        return $"[{PartTypeId}.{PartRoleId ?? "-"}] {Name}={Value}";
    }
}
