using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes graph updater.
    /// </summary>
    public sealed class GraphUpdater
    {
        private readonly IGraphRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphUpdater"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public GraphUpdater(IGraphRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Updates the graph with the specified nodes and triples.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="triples">The triples.</param>
        /// <exception cref="ArgumentNullException">nodes or triples</exception>
        public void Update(IList<Node> nodes, IList<Triple> triples)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (triples == null)
                throw new ArgumentNullException(nameof(triples));

            try
            {
                _repository.BeginTransaction();
                // TODO
                _repository.CommitTransaction();
            }
            catch (Exception)
            {
                _repository.RollbackTransaction();
                throw;
            }
        }
    }
}
