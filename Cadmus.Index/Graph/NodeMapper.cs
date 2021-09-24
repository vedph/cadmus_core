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

        /// <summary>
        /// Applies the specified mapping and all its descendants.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="state">The state.</param>
        private void ApplyMapping(NodeMapping mapping, NodeMapperState state)
        {
            Logger?.LogInformation($"Mapping #{mapping.Id}: {mapping.Name}");

            // build the SID for this mapping
            state.Sid = SidBuilder.Build(mapping.SourceType,
                state.Part?.Id ?? state.Item.Id,
                state.GroupOrdinal,
                state.Part?.RoleId,
                state.PinName, state.PinValue);
            Logger?.LogInformation("SID " + state.Sid);

            // calculate variables
            NodeMappingVariableSet vset = NodeMappingVariableSet.LoadFrom(mapping);
            vset.SetValues(state);

            // generate node
            Node node = new Node();

            // build the node's label following label_template
            if (!string.IsNullOrEmpty(mapping.LabelTemplate))
                node.Label = vset.ResolvePlaceholders(mapping.LabelTemplate);

            // build the UID prefixes from prefix and triple_o_prefix
            string prefix = null;
            if (!string.IsNullOrEmpty(mapping.Prefix))
                prefix = vset.ResolvePlaceholders(mapping.Prefix);

            string oPrefix = null;
            if (!string.IsNullOrEmpty(mapping.TripleOPrefix))
                oPrefix = vset.ResolvePlaceholders(mapping.TripleOPrefix);

            // generate node's UID using prefix, filtered label,
            // and an eventual suffix
            // TODO

            // add node to set
            state.Nodes.Add(node);

            // if there is a triple, collect SPO from triple_s, triple_p,
            // triple_o (triple_o_prefix) and reversed, then generate it
            // together with its O's node unless it's a literal or already exists

            // children mappings
            IList<NodeMapping> children = state.Part == null
                ? _repository.FindMappingsFor(state.Item, mapping.Id)
                : _repository.FindMappingsFor(state.Item, state.Part,
                    state.PinName, mapping.Id);
            // update state
            state.MappingPath.Add(mapping.Id);

            foreach (var child in children) ApplyMapping(child, state);

            state.MappingPath.RemoveAt(state.MappingPath.Count - 1);
        }

        /// <summary>
        /// The entry point for applying all the matching mappings to a source.
        /// </summary>
        /// <param name="state">The initial state.</param>
        private void Map(NodeMapperState state)
        {
            // get all the matching root mappings
            IList<NodeMapping> mappings = state.Part == null
                ? _repository.FindMappingsFor(state.Item)
                : _repository.FindMappingsFor(state.Item, state.Part, state.PinName);
            Logger?.LogInformation("Rules matched: " + mappings.Count);

            // apply each of them
            foreach (NodeMapping mapping in mappings)
            {
                // if we're targeting an item, and the item has a composite
                // group ID, apply mapping to each group's component,
                // from top to bottom (=left to right)
                if (mapping.SourceType == NodeSourceType.ItemGroup
                    && state.Part == null
                    && state.Item.GroupId?.IndexOf('/') > -1)
                {
                    Logger?.LogInformation("Composite group ID " +
                        state.Item.GroupId);
                    string[] gcc = state.Item.GroupId.Split(new[] { '/' },
                        StringSplitOptions.RemoveEmptyEntries);

                    state.GroupOrdinal = 0;
                    foreach (string gc in gcc)
                    {
                        Logger?.LogInformation(
                            "Mapping item for group component " + gc);
                        state.Item.GroupId = gc;
                        state.GroupOrdinal++;
                        ApplyMapping(mapping, state);
                    }
                }
                // else just apply the mapping once
                else ApplyMapping(mapping, state);

                // children mappings
                IList<NodeMapping> children = state.Part == null
                    ? _repository.FindMappingsFor(state.Item, mapping.Id)
                    : _repository.FindMappingsFor(state.Item, state.Part,
                        state.PinName, mapping.Id);
                // update state
                state.GroupOrdinal = 0;
                state.MappingPath.Add(mapping.Id);

                foreach (var child in children)
                {
                    ApplyMapping(child, state);
                }
                state.MappingPath.RemoveAt(state.MappingPath.Count - 1);
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
            NodeMapperState state = new NodeMapperState
            {
                Item = item
            };
            Map(state);
        }

        /// <summary>
        /// Maps the specified part's pin.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part.</param>
        /// <param name="pinName">Name of the pin.</param>
        /// <param name="pinValue">The pin value.</param>
        /// <exception cref="ArgumentNullException">nameof(item)</exception>
        public void MapPin(IItem item, IPart part, string pinName, string pinValue)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Logger?.LogInformation($"Mapping {part.Id}/{pinName}=" +
                (pinValue.Length > 80? pinValue.Substring(0, 80) + "..." : pinValue));

            NodeMapperState state = new NodeMapperState
            {
                Item = item,
                Part = part,
                PinName = pinName,
                PinValue = pinValue
            };
            Map(state);
        }
    }
}
