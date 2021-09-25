using System;
using System.Collections.Generic;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A set of graph nodes and triples.
    /// </summary>
    public class GraphSet
    {
        /// <summary>
        /// Gets the nodes generated in the current mapping session.
        /// </summary>
        public IList<Node> Nodes { get; }

        /// <summary>
        /// Gets the triples generated in the current mapping session.
        /// </summary>
        public IList<Triple> Triples { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphSet"/> class.
        /// </summary>
        public GraphSet()
        {
            Nodes = new List<Node>();
            Triples = new List<Triple>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphSet"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="triples">The triples.</param>
        /// <exception cref="ArgumentNullException">nodes or triples</exception>
        public GraphSet(IList<Node> nodes, IList<Triple> triples)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
            Triples = triples ?? throw new ArgumentNullException(nameof(triples));
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
