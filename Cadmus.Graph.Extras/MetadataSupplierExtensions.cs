namespace Cadmus.Graph.Extras;

/// <summary>
/// Extensions to <see cref="MetadataSupplier"/>.
/// </summary>
public static class MetadataSupplierExtensions
{
    /// <summary>
    /// Adds the <c>item-eid</c> metadatum supplier.
    /// </summary>
    /// <param name="supplier">The supplier to extend.</param>
    public static MetadataSupplier AddItemEid(this MetadataSupplier supplier)
    {
        supplier.AddMetadataSource(new ItemEidMetadataSource());
        return supplier;
    }
}
