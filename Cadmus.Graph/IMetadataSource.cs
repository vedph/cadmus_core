using Cadmus.Core.Storage;
using Cadmus.Graph.Adapters;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// A source of metadata for <see cref="MetadataSupplier"/>.
/// </summary>
public interface IMetadataSource
{
    /// <summary>
    /// Supplies metadata for the specified source.
    /// </summary>
    /// <param name="source">The source item/part for mapping.</param>
    /// <param name="metadata">The metadata dictionary targeted by this supplier.
    /// </param>
    /// <param name="repository">The optional Cadmus repository.</param>
    /// <param name="context">An optional context object, eventually used
    /// by implementors.</param>
    void Supply(GraphSource source, IDictionary<string, object> metadata,
        ICadmusRepository? repository, object? context = null);
}
