using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Graph;

/// <summary>
/// A set of graph nodes and triples, derived from one or more mapping
/// sessions.
/// </summary>
public class GraphSet
{
    /// <summary>
    /// Gets the nodes.
    /// </summary>
    public IList<UriNode> Nodes { get; }

    /// <summary>
    /// Gets the triples.
    /// </summary>
    public IList<UriTriple> Triples { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphSet"/> class.
    /// </summary>
    public GraphSet()
    {
        Nodes = new List<UriNode>();
        Triples = new List<UriTriple>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphSet"/> class.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <param name="triples">The triples.</param>
    /// <exception cref="ArgumentNullException">nodes or triples</exception>
    public GraphSet(IList<UriNode> nodes, IList<UriTriple> triples)
    {
        Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        Triples = triples ?? throw new ArgumentNullException(nameof(triples));
    }

    /// <summary>
    /// Adds the specified nodes to this set, unless they already exist.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    /// <exception cref="ArgumentNullException">nodes</exception>
    public void AddNodes(IList<UriNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);

        foreach (UriNode node in nodes)
        {
            if (Nodes.All(n => n.Id != node.Id))
                Nodes.Add(node);
        }
    }

    /// <summary>
    /// Adds the specified triples to this set, unless they already exist.
    /// </summary>
    /// <param name="triples">The triples.</param>
    /// <exception cref="ArgumentNullException">nodes</exception>
    public void AddTriples(IList<UriTriple> triples)
    {
        ArgumentNullException.ThrowIfNull(triples);

        foreach (UriTriple triple in triples)
        {
            if (Triples.All(t => t.SubjectId != triple.SubjectId ||
                t.PredicateId != triple.PredicateId ||
                t.ObjectId != triple.ObjectId ||
                t.ObjectLiteral != triple.ObjectLiteral))
            {
                Triples.Add(triple);
            }
        }
    }

    /// <summary>
    /// Gets groups of nodes having the same SID's GUID. Usually a set contains
    /// entries from a single GUID source, but some of them may have a null
    /// SID (=the nodes generated as objects of a triple).
    /// </summary>
    /// <returns>Dictionary.</returns>
    public IDictionary<string, IList<UriNode>> GetNodesByGuid()
    {
        Dictionary<string, IList<UriNode>> dct = new();

        foreach (UriNode node in Nodes)
        {
            string key = node.Sid != null? node.Sid[..36] : "";

            if (!dct.ContainsKey(key)) dct[key] = new List<UriNode>();
            dct[key].Add(node);
        }
        return dct;
    }

    /// <summary>
    /// Gets groups of triples having the same SID's GUID. Usually a set
    /// contains entries from a single GUID source, but this enforces
    /// the update constraints.
    /// </summary>
    /// <returns>Dictionary.</returns>
    public IDictionary<string, IList<UriTriple>> GetTriplesByGuid()
    {
        Dictionary<string, IList<UriTriple>> dct = new();

        foreach (UriTriple triple in Triples)
        {
            string key = triple.Sid != null? triple.Sid[..36] : "";

            if (!dct.ContainsKey(key)) dct[key] = new List<UriTriple>();
            dct[key].Add(triple);
        }
        return dct;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"N: {Nodes.Count} | T: {Triples.Count}";
    }
}
