using Cadmus.Core;
using Cadmus.Graph.Adapters;
using System;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// Graph updater. This is a top level helper class, used to update the
/// graph whenever an item or part gets saved.
/// </summary>
public class GraphUpdater
{
    private readonly IGraphRepository _repository;
    private readonly INodeMapper _mapper;
    private readonly ItemGraphSourceAdapter _itemAdapter;
    private readonly PartGraphSourceAdapter _partAdapter;

    /// <summary>
    /// Gets the metadata used by the updater. You can add more metadata
    /// manually, or set the <see cref="MetadataSupplier"/> property so that
    /// it will be invoked before each update. All metadata are automatically
    /// reset after updating.
    /// </summary>
    public IDictionary<string, object> Metadata { get; }

    /// <summary>
    /// Gets or sets the optional metadata supplier to be used by this updater.
    /// </summary>
    public MetadataSupplier? MetadataSupplier { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphUpdater"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <exception cref="ArgumentNullException">repository</exception>
    public GraphUpdater(IGraphRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = new JsonNodeMapper
        {
            IsMappingTracingEnabled = true,
            UidBuilder = repository
        };
        _itemAdapter = new ItemGraphSourceAdapter();
        _partAdapter = new PartGraphSourceAdapter();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds the specified macro.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="macro">The macro.</param>
    public void AddMacro(string key, INodeMappingMacro macro)
        => _mapper.AddMacro(key, macro);

    /// <summary>
    /// Deletes the macro with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void DeleteMacro(string key) => _mapper.DeleteMacro(key);

    /// <summary>
    /// Sets the macros to use in this mapper.
    /// </summary>
    /// <param name="macros">The macros.</param>
    public void SetMacros(IDictionary<string, INodeMappingMacro> macros) =>
        _mapper.SetMacros(macros);

    /// <summary>
    /// Resets the macros to the builtin ones.
    /// </summary>
    public void ResetMacros() => _mapper.ResetMacros();

    private void SetMapperMetadata()
    {
        _mapper.Data.Clear();
        foreach (KeyValuePair<string, object> p in Metadata)
            _mapper.Data.Add(p.Key, p.Value);
    }

    private void Update(object data, RunNodeMappingFilter filter)
    {
        SetMapperMetadata();
        GraphSet set = new();

        foreach (NodeMapping mapping in _repository.FindMappings(filter))
            _mapper.Map(data, mapping, set);

        _repository.UpdateGraph(set);
        Metadata.Clear();
    }

    private GraphUpdaterExplanation Explain(object data,
        RunNodeMappingFilter filter)
    {
        SetMapperMetadata();
        GraphUpdaterExplanation explanation = new()
        {
            Filter = filter
        };
        foreach (NodeMapping mapping in _repository.FindMappings(filter))
        {
            _mapper.Map(data, mapping, explanation.Set);
            if (_mapper.Data.TryGetValue(NodeMapper.APPLIED_MAPPING_LIST,
                out object? o) && o is IList<NodeMapping> applied)
            {
                foreach (NodeMapping a in applied) explanation.Mappings.Add(a);
            }
            else
            {
                explanation.Mappings.Add(mapping);
            }
        }
        // copy adapted metadata to explanation metadata
        foreach (KeyValuePair<string, object> p in Metadata)
            explanation.Metadata.Add(p.Key, p.Value);

        Metadata.Clear();

        return explanation;
    }

    /// <summary>
    /// Updates the graph from the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">item</exception>
    public void Update(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _mapper.Context = new GraphSource(item);
        MetadataSupplier?.Supply(_mapper.Context, Metadata);

        var df = _itemAdapter.Adapt(_mapper.Context, Metadata);
        if (df.Item1 != null) Update(df.Item1, df.Item2);
    }

    /// <summary>
    /// Updates the graph from the specified item's part.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="part">The part.</param>
    /// <exception cref="ArgumentNullException">item or part</exception>
    public void Update(IItem item, IPart part)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(part);

        _mapper.Context = new GraphSource(item, part);
        MetadataSupplier?.Supply(_mapper.Context, Metadata);

        var df = _partAdapter.Adapt(_mapper.Context, Metadata);
        if (df.Item1 != null) Update(df.Item1, df.Item2);
    }

    /// <summary>
    /// Explains the update path for the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>Explanation or null if item not found.</returns>
    /// <exception cref="ArgumentNullException">item</exception>
    public GraphUpdaterExplanation? Explain(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _mapper.Context = new GraphSource(item);
        MetadataSupplier?.Supply(_mapper.Context, Metadata);

        var df = _itemAdapter.Adapt(_mapper.Context, Metadata);
        if (df.Item1 == null) return null;
        return Explain(df.Item1, df.Item2);
    }

    /// <summary>
    /// Explains the update path for the specified part.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="part">The part.</param>
    /// <returns>Explanation or null if item not found.</returns>
    /// <exception cref="ArgumentNullException">item or part</exception>
    public GraphUpdaterExplanation? Explain(IItem item, IPart part)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(part);

        _mapper.Context = new GraphSource(item, part);
        MetadataSupplier?.Supply(_mapper.Context, Metadata);

        var df = _partAdapter.Adapt(_mapper.Context, Metadata);
        if (df.Item1 == null) return null;
        return Explain(df.Item1, df.Item2);
    }
}
