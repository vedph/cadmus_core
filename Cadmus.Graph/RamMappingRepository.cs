using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace Cadmus.Graph;

/// <summary>
/// RAM-based mappings repository.
/// </summary>
/// <seealso cref="IMappingRepository" />
public class RamMappingRepository : IMappingRepository
{
    private int _nextId;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Gets all the mappings in this repository.
    /// </summary>
    public List<NodeMapping> Mappings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RamMappingRepository"/> class.
    /// </summary>
    public RamMappingRepository()
    {
        Mappings = new();
        _jsonOptions = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };
        _jsonOptions.Converters.Add(new NodeMappingOutputJsonConverter());
    }

    private int GetNextId() => Interlocked.Increment(ref _nextId);

    private static IQueryable<NodeMapping> ApplyNodeMappingFilter(
        NodeMappingFilter filter, IQueryable<NodeMapping> mappings)
    {
        if (filter.ParentId != null)
            mappings = mappings.Where(m => m.ParentId == filter.ParentId);

        if (filter.SourceType.HasValue)
            mappings = mappings.Where(m => m.SourceType == filter.SourceType.Value);

        if (!string.IsNullOrEmpty(filter.Name))
        {
            mappings = mappings.Where(m => m.Name != null &&
                m.Name.Contains(filter.Name));
        }

        if (!string.IsNullOrEmpty(filter.Facet))
        {
            mappings = mappings.Where(m => m.FacetFilter != null &&
                m.FacetFilter == filter.Facet);
        }

        if (!string.IsNullOrEmpty(filter.Group))
        {
            Regex r = new(filter.Group, RegexOptions.IgnoreCase);
            mappings = mappings.Where(m => r.IsMatch(m.GroupFilter ?? ""));
        }

        if (filter.Flags.HasValue)
        {
            mappings = mappings.Where(m => m.FlagsFilter != null &&
                (m.FlagsFilter.Value & filter.Flags.Value) == filter.Flags.Value);
        }

        if (!string.IsNullOrEmpty(filter.Title))
        {
            mappings = mappings.Where(m => m.TitleFilter != null &&
                m.TitleFilter.Contains(filter.Title));
        }

        if (!string.IsNullOrEmpty(filter.PartType))
        {
            mappings = mappings.Where(m => m.PartTypeFilter == filter.PartType);
        }

        if (!string.IsNullOrEmpty(filter.PartRole))
        {
            mappings = mappings.Where(m => m.PartRoleFilter == filter.PartRole);
        }

        return mappings;
    }

    /// <summary>
    /// Gets the specified page of node mappings.
    /// </summary>
    /// <param name="filter">The filter. Set page size=0 to get all
    /// the mappings at once.</param>
    /// <param name="descendants">True to load the descendants of each mapping.
    /// </param>
    /// <returns>The page.</returns>
    /// <exception cref="ArgumentNullException">filter</exception>
    public DataPage<NodeMapping> GetMappings(NodeMappingFilter filter,
        bool descendants)
    {
        IQueryable<NodeMapping> mappings =
            ApplyNodeMappingFilter(filter, Mappings.AsQueryable());

        int total = mappings.Count();
        if (total == 0)
        {
            return new DataPage<NodeMapping>(
                filter.PageNumber, filter.PageSize, 0,
                Array.Empty<NodeMapping>());
        }

        mappings = mappings.OrderBy(m => m.Name).ThenBy(m => m.Id)
            .Skip(filter.PageSize == 0? 0 : filter.GetSkipCount())
            .Take(filter.PageSize == 0? total : filter.PageSize);

        return new DataPage<NodeMapping>(filter.PageNumber,
            filter.PageSize, total, mappings.ToList());
    }

    /// <summary>
    /// Gets the node mapping with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The mapping or null if not found.</returns>
    public NodeMapping? GetMapping(int id) => Mappings.Find(m => m.Id == id);

    /// <summary>
    /// Deletes the mapping with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteMapping(int id)
    {
        NodeMapping? mapping = GetMapping(id);
        if (mapping != null) Mappings.Remove(mapping);
    }

    /// <summary>
    /// Adds or updates the specified mapping. When the mapping ID is 0, it is
    /// assumed that it's a new mapping, and it will get a new ID; otherwise,
    /// it will replace another mapping with the same ID.
    /// </summary>
    /// <param name="mapping">The mapping.</param>
    /// <returns>The mapping ID.</returns>
    public int AddMapping(NodeMapping mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);

        if (mapping.Id != 0) DeleteMapping(mapping.Id);
        else mapping.Id = GetNextId();
        Mappings.Add(mapping);
        return mapping.Id;
    }

    /// <summary>
    /// Adds the mapping by name. If a mapping with the same name already
    /// exists, it will be updated. Names are not case sensitive.
    /// </summary>
    /// <param name="mapping">The mapping.</param>
    /// <returns>
    /// The ID of the mapping.
    /// </returns>
    /// <exception cref="ArgumentNullException">mapping</exception>
    /// <exception cref="InvalidOperationException">Adding mappings by name
    /// requires ID=0</exception>
    public int AddMappingByName(NodeMapping mapping)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        if (mapping.Id != 0)
            throw new InvalidOperationException("Adding mappings by name requires ID=0");

        NodeMapping? old = Mappings.Find(m => m.Name?.Equals(mapping.Name,
            StringComparison.OrdinalIgnoreCase) == true);

        if (old != null) Mappings.Remove(old);
        else mapping.Id = GetNextId();

        Mappings.Add(mapping);
        return mapping.Id;
    }

    /// <summary>
    /// Imports mappings from the specified JSON code representing a mappings
    /// document.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns>The number of root mappings imported.</returns>
    /// <exception cref="ArgumentNullException">json</exception>
    /// <exception cref="InvalidDataException">Invalid JSON mappings document
    /// </exception>
    public int Import(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        NodeMappingDocument? doc =
            JsonSerializer.Deserialize<NodeMappingDocument>(json,
            _jsonOptions)
            ?? throw new InvalidDataException("Invalid JSON mappings document");

        int n = 0;
        foreach (NodeMapping mapping in doc.GetMappings())
        {
            AddMapping(mapping);
            n++;
        }
        return n;
    }

    /// <summary>
    /// Exports mappings to JSON code representing a mappings document.
    /// </summary>
    /// <returns>JSON.</returns>
    public string Export()
    {
        NodeMappingDocument doc = new();
        doc.DocumentMappings.AddRange(Mappings);
        return JsonSerializer.Serialize(doc, _jsonOptions);
    }
}
