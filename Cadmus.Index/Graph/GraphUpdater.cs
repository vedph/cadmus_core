using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes graph updater. This takes a set of nodes and triples as generated
    /// by a <see cref="NodeMapper"/>, and uses an <see cref="IGraphRepository"/>
    /// to collect the corresponding set from the database. Then it compares
    /// the new and the old sets, and updates the old set accordingly.
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
        /// <param name="set">The new set of nodes and triples.</param>
        /// <exception cref="ArgumentNullException">set</exception>
        public void Update(GraphSet set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));

            try
            {
                _repository.BeginTransaction();

                // get the old set
                var guidAndItem = set.GetSourceGuidAndType();
                GraphSet oldSet = guidAndItem == null ?
                    null : _repository.GetGraphSet(guidAndItem.Item1);
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
