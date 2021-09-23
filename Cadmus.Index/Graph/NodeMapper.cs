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

        /// <summary>
        /// Gets all the root mappings which match the specified item or
        /// item's part's pin.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part. Null when mapping an item.</param>
        /// <param name="pinName">Name of the pin. Null when mapping an item.</param>
        /// <returns>Matching root mappings.</returns>
        public IList<NodeMapping> GetRootMappings(IItem item, IPart part = null,
            string pinName = null)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            NodeMappingFilter filter = new NodeMappingFilter();

            if (part == null)
            {
                // item

            }
            else
            {
                // TODO pin
            }

            DataPage<NodeMapping> page = _repository.GetNodeMappings(filter);

            return page.Items.OrderBy(i => i.SourceType)
                .ThenBy(i => i.Ordinal)
                .ThenBy(i => i.PartType)
                .ThenBy(i => i.PartRole)
                .ThenBy(i => i.PinName)
                .ThenBy(i => i.Name)
                .ToList();
        }
    }
}
