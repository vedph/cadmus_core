using Cadmus.Core.Storage;
using Cadmus.Graph.Adapters;
using System;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// Extensible class for supplying additional metadata to a <see cref="GraphUpdater"/>.
/// You can add extension methods to this class to allow for additional metadata
/// supplied for a specific <see cref="GraphSource"/>.
/// </summary>
public class MetadataSupplier
{
    private readonly List<IMetadataSource> _sources;
    private ICadmusRepository? _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataSupplier"/> class.
    /// </summary>
    public MetadataSupplier()
    {
        _sources = new List<IMetadataSource>();
    }

    /// <summary>
    /// Sets the cadmus repository.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <returns>The supplier.</returns>
    public MetadataSupplier SetCadmusRepository(ICadmusRepository? repository)
    {
        _repository = repository;
        return this;
    }

    /// <summary>
    /// Adds the specified metadata source to this supplier.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The supplier.</returns>
    /// <exception cref="ArgumentNullException">source</exception>
    public MetadataSupplier AddMetadataSource(IMetadataSource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        _sources.Add(source);
        return this;
    }

    /// <summary>
    /// Supplies metadata for the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="metadata">The target metadata to update.</param>
    /// <param name="context">The optional context object.</param>
    /// <exception cref="ArgumentNullException">source or metadata</exception>
    public void Supply(GraphSource source, IDictionary<string, object> metadata,
        object? context = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(metadata);

        foreach (IMetadataSource ms in _sources)
            ms.Supply(source, metadata, _repository, context);
    }
}
