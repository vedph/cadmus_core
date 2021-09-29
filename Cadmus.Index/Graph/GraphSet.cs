using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A set of graph nodes and triples, derived from one or more mapping
    /// sessions.
    /// </summary>
    public class GraphSet
    {
        /// <summary>
        /// Gets the nodes.
        /// </summary>
        public IList<NodeResult> Nodes { get; }

        /// <summary>
        /// Gets the triples.
        /// </summary>
        public IList<TripleResult> Triples { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphSet"/> class.
        /// </summary>
        public GraphSet()
        {
            Nodes = new List<NodeResult>();
            Triples = new List<TripleResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphSet"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="triples">The triples.</param>
        /// <exception cref="ArgumentNullException">nodes or triples</exception>
        public GraphSet(IList<NodeResult> nodes, IList<TripleResult> triples)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
            Triples = triples ?? throw new ArgumentNullException(nameof(triples));
        }

        /// <summary>
        /// Adds the specified nodes to this set, unless they already exist.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <exception cref="ArgumentNullException">nodes</exception>
        public void AddNodes(IList<NodeResult> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            foreach (NodeResult node in nodes)
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
        public void AddTriples(IList<TripleResult> triples)
        {
            if (triples == null) throw new ArgumentNullException(nameof(triples));

            foreach (TripleResult triple in triples)
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
        public IDictionary<string, IList<NodeResult>> GetNodesByGuid()
        {
            Dictionary<string, IList<NodeResult>> dct =
                new Dictionary<string, IList<NodeResult>>();

            foreach (NodeResult triple in Nodes)
            {
                string key = triple.Sid?.Substring(0, 36) ?? "";

                if (!dct.ContainsKey(key)) dct[key] = new List<NodeResult>();
                dct[key].Add(triple);
            }
            return dct;
        }

        /// <summary>
        /// Gets groups of triples having the same SID's GUID. Usually a set
        /// contains entries from a single GUID source, but this enforces
        /// the update constraints.
        /// </summary>
        /// <returns>Dictionary.</returns>
        public IDictionary<string, IList<TripleResult>> GetTriplesByGuid()
        {
            Dictionary<string, IList<TripleResult>> dct =
                new Dictionary<string, IList<TripleResult>>();

            foreach (TripleResult triple in Triples)
            {
                string key = triple.Sid?.Substring(0, 36) ?? "";

                if (!dct.ContainsKey(key)) dct[key] = new List<TripleResult>();
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
}
