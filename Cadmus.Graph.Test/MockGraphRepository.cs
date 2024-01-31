using Cadmus.Core.Config;
using Fusi.Tools;
using Fusi.Tools.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cadmus.Graph.Test;

internal class MockGraphRepository : RamMappingRepository, IGraphRepository
{
    private readonly RamUidBuilder _uidBuilder;

    public IMemoryCache? Cache { get; set; }

    public MockGraphRepository()
    {
        _uidBuilder = new RamUidBuilder();
    }

    public bool CreateStore(object? payload = null)
    {
        return false;
    }

    public void AddNamespace(string prefix, string uri)
    {
        throw new NotImplementedException();
    }

    public void AddNode(Node node, bool noUpdate = false)
    {
        throw new NotImplementedException();
    }

    public void AddProperty(Property property)
    {
        throw new NotImplementedException();
    }

    public void AddThesaurus(Thesaurus thesaurus, bool includeRoot,
        string? prefix = null)
    {
        throw new NotImplementedException();
    }

    public void AddTriple(Triple triple)
    {
        throw new NotImplementedException();
    }

    public int AddUri(string uri)
    {
        throw new NotImplementedException();
    }

    public string BuildUid(string unsuffixed, string sid)
    {
        return _uidBuilder.BuildUid(unsuffixed, sid);
    }

    public void DeleteGraphSet(string sourceId)
    {
        throw new NotImplementedException();
    }

    public void DeleteNamespaceByPrefix(string prefix)
    {
        throw new NotImplementedException();
    }

    public void DeleteNamespaceByUri(string uri)
    {
        throw new NotImplementedException();
    }

    public void DeleteNode(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteProperty(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteTriple(int id)
    {
        throw new NotImplementedException();
    }

    private static IQueryable<NodeMapping> ApplyNodeMappingFilter(
        RunNodeMappingFilter filter, IQueryable<NodeMapping> mappings)
    {
        // (in our RAM list all the mappings are top-level, so no parent ID
        // filtering is required)

        // source type is always present
        mappings = mappings.Where(m => m.SourceType == filter.SourceType);

        if (!string.IsNullOrEmpty(filter.Facet))
        {
            mappings = mappings.Where(m => m.FacetFilter == null
                || m.FacetFilter == filter.Facet);
        }

        if (!string.IsNullOrEmpty(filter.Group))
        {
            mappings = mappings.Where(m =>
                m.GroupFilter == null ||
                Regex.IsMatch(filter.Group, m.GroupFilter ?? ""));
        }

        if (filter.Flags.HasValue)
        {
            mappings = mappings.Where(m =>
                m.FlagsFilter == null ||
                (m.FlagsFilter & filter.Flags.Value) == filter.Flags.Value);
        }

        if (!string.IsNullOrEmpty(filter.Title))
        {
            mappings = mappings.Where(m =>
                m.TitleFilter == null ||
                Regex.IsMatch(filter.Title, m.TitleFilter ?? ""));
        }

        if (!string.IsNullOrEmpty(filter.PartType))
        {
            mappings = mappings.Where(m =>
                m.PartTypeFilter == null ||
                m.PartTypeFilter == filter.PartType);
        }

        if (!string.IsNullOrEmpty(filter.PartRole))
        {
            mappings = mappings.Where(m =>
                m.PartRoleFilter == null ||
                m.PartRoleFilter == filter.PartRole);
        }

        return mappings;
    }

    public IList<NodeMapping> FindMappings(RunNodeMappingFilter filter)
    {
        return new List<NodeMapping>(
            ApplyNodeMappingFilter(filter, Mappings.AsQueryable()));
    }

    public GraphSet GetGraphSet(string sourceId)
    {
        throw new NotImplementedException();
    }

    public DataPage<UriTriple> GetLinkedLiterals(LinkedLiteralFilter filter)
    {
        throw new NotImplementedException();
    }

    public DataPage<UriNode> GetLinkedNodes(LinkedNodeFilter filter)
    {
        throw new NotImplementedException();
    }

    public DataPage<NamespaceEntry> GetNamespaces(NamespaceFilter filter)
    {
        throw new NotImplementedException();
    }

    public UriNode? GetNode(int id)
    {
        throw new NotImplementedException();
    }

    public UriNode? GetNodeByUri(string uri)
    {
        throw new NotImplementedException();
    }

    public DataPage<UriNode> GetNodes(NodeFilter filter)
    {
        throw new NotImplementedException();
    }

    public IList<UriNode?> GetNodes(IList<int> ids)
    {
        throw new NotImplementedException();
    }

    public DataPage<UriProperty> GetProperties(PropertyFilter filter)
    {
        throw new NotImplementedException();
    }

    public UriProperty? GetProperty(int id)
    {
        throw new NotImplementedException();
    }

    public UriProperty? GetPropertyByUri(string uri)
    {
        throw new NotImplementedException();
    }

    public UriTriple? GetTriple(int id)
    {
        throw new NotImplementedException();
    }

    public DataPage<TripleGroup> GetTripleGroups(TripleFilter filter,
        string sort = "Cu")
    {
        throw new NotImplementedException();
    }

    public DataPage<UriTriple> GetTriples(TripleFilter filter)
    {
        throw new NotImplementedException();
    }

    public void ImportNodes(IEnumerable<UriNode> nodes)
    {
        throw new NotImplementedException();
    }

    public void ImportTriples(IEnumerable<UriTriple> triples)
    {
        throw new NotImplementedException();
    }

    public int LookupId(string uri)
    {
        throw new NotImplementedException();
    }

    public string? LookupNamespace(string prefix)
    {
        throw new NotImplementedException();
    }

    public string? LookupUri(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateGraph(GraphSet set)
    {
        // nope
    }

    public Task UpdateNodeClassesAsync(CancellationToken cancel,
        IProgress<ProgressReport>? progress = null)
    {
        throw new NotImplementedException();
    }
}
