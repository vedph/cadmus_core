using System;

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

            // get the old set
            var guidAndItem = set.GetSourceGuidAndType();
            GraphSet oldSet = guidAndItem == null ?
                null : _repository.GetGraphSet(guidAndItem.Item1);

            // compare sets
            CrudGrouper<NodeResult> nodeGrouper = new CrudGrouper<NodeResult>();
            nodeGrouper.Group(set.Nodes, oldSet.Nodes,
                (NodeResult a, NodeResult b) => a.Id == b.Id);

            CrudGrouper<TripleResult> tripleGrouper = new CrudGrouper<TripleResult>();
            tripleGrouper.Group(set.Triples, oldSet.Triples,
                (TripleResult a, TripleResult b) =>
                {
                    return a.SubjectId == b.SubjectId &&
                        a.PredicateId == b.PredicateId &&
                        a.ObjectId == b.ObjectId &&
                        a.Sid == b.Sid;
                });

            try
            {
                // execute updates
                _repository.BeginTransaction();

                // nodes
                foreach (NodeResult node in nodeGrouper.Deleted)
                    _repository.DeleteNode(node.Id);
                foreach (NodeResult node in nodeGrouper.Added)
                    _repository.AddNode(node);
                foreach (NodeResult node in nodeGrouper.Updated)
                    _repository.AddNode(node, node.Sid == null);

                // triples
                foreach (TripleResult triple in tripleGrouper.Deleted)
                    _repository.DeleteTriple(triple.Id);
                foreach (TripleResult triple in tripleGrouper.Added)
                    _repository.AddTriple(triple);
                foreach (TripleResult triple in tripleGrouper.Updated)
                    _repository.AddTriple(triple);

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
