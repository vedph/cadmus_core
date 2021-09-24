using Cadmus.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes mapper. This maps items or part's pins to graph nodes.
    /// </summary>
    public sealed class NodeMapper
    {
        private class NodeMappingInput
        {
            public IList<Node> Nodes { get; set; }
            public IItem Item { get; set; }
            public int GroupOrdinal { get; set; }
            public IPart Part { get; set; }
            public string PinName { get; set; }
            public string PinValue { get; set; }
        }

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

        /// <summary>
        /// Parses the name of the pin for EID suffixes.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An array of strings where 1=name without EID suffix, 2-N=the
        /// EID suffix(es) without their initial <c>@</c> prefix.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public static string[] ParsePinName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.IndexOf('@') == -1) return new[] { name };

            Match m = Regex.Match(name, "^(?<n>[^@]+)(?:@(?<e>[^@]+))*$");
            if (!m.Success) return new[] { name };

            string[] results = new string[1 + m.Groups["e"].Captures.Count];
            results[0] = m.Groups["n"].Value;
            for (int i = 0; i < m.Groups["e"].Captures.Count; i++)
                results[1 + i] = m.Groups["e"].Captures[i].Value;
            return results;
        }

        private void ApplyMapping(NodeMapping mapping, NodeMappingInput input)
        {
            // build the SID
            string sid = SidBuilder.Build(mapping.SourceType,
                input.Part?.Id ?? input.Item.Id,
                input.GroupOrdinal,
                input.Part?.RoleId,
                input.PinName, input.PinValue);

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

                    int n = 0;
                    foreach (string gc in gcc)
                    {
                        Logger?.LogInformation(
                            "Mapping item for group component " + gc);
                        item.GroupId = gc;
                        ApplyMapping(mapping, nodes, item, ++n, part,
                            pinName, pinValue);
                    }
                }
                // else just apply the mapping once
                else ApplyMapping(mapping, nodes, item, 0, part,
                    pinName, pinValue);

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
