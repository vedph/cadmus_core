using Cadmus.Core;
using Fusi.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes mapper. This maps items or part's pins to graph nodes.
    /// </summary>
    public class NodeMapper
    {
        private readonly IGraphRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMapper"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="System.ArgumentNullException">repository</exception>
        public NodeMapper(IGraphRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
