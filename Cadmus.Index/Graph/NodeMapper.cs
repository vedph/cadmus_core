using Cadmus.Core;
using Fusi.Tools.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes mapper. This maps items or part's pins to graph nodes.
    /// </summary>
    public class NodeMapper
    {
        private readonly IGraphRepository _repository;

        /// <summary>
        /// Gets or sets the optional logger to use.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMapper"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public NodeMapper(IGraphRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Parses the item title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>Tuple with 1=title, 2=prefix, 3=uid.</returns>
        /// <exception cref="ArgumentNullException">title</exception>
        public static Tuple<string,string,string> ParseItemTitle(string title)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));

            Match m = Regex.Match(title, @"^(?<t>)\s*\[(?<k>[\#@])(?<v>[^]]+)\]$");

            if (!m.Success) return Tuple.Create(title, (string)null, (string)null);

            return m.Groups["k"].Value == "@"
                ? Tuple.Create(m.Groups["t"].Value, m.Groups["v"].Value, (string)null)
                : Tuple.Create(m.Groups["t"].Value, (string)null, m.Groups["v"].Value);
        }

        private void ApplyMapping(NodeMapping mapping, IList<Node> nodes,
            IItem item, IPart part = null, string pinName = null,
            string pinValue = null)
        {
            // build the SID
            string sid = SidBuilder.Build(mapping.SourceType,
                part != null? part.Id : item.Id, part?.RoleId,
                pinName, pinValue);

            // TODO
        }

        private void Map(IItem item, IPart part = null, string pinName = null,
            string pinValue = null)
        {
            // get all the matching root mappings
            IList<NodeMapping> mappings = _repository.FindMappingsFor(item);
            Logger?.LogInformation("Rules matched: " + mappings.Count);

            // apply each of them
            List<Node> nodes = new List<Node>();

            foreach (NodeMapping mapping in mappings)
            {
                // if we're targeting an item, and the item has a composite
                // group ID, apply mapping to each group's component,
                // from top to bottom (=left to right)
                if (mapping.SourceType == NodeSourceType.ItemGroup
                    && part == null
                    && item.GroupId?.IndexOf('/') > -1)
                {
                    Logger?.LogInformation("Composite group ID " + item.GroupId);
                    string[] gcc = item.GroupId.Split(new[] { '/' },
                        StringSplitOptions.RemoveEmptyEntries);

                    foreach (string gc in gcc)
                    {
                        Logger?.LogInformation(
                            "Mapping item for group component " + gc);
                        item.GroupId = gc;
                        ApplyMapping(mapping, nodes, item, part, pinName, pinValue);
                    }
                }
                // else just apply the mapping once
                else ApplyMapping(mapping, nodes, item, part, pinName, pinValue);

                // TODO children
            }
        }

        /// <summary>
        /// Maps the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void MapItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Logger?.LogInformation("Mapping " + item);

        }
    }
}
