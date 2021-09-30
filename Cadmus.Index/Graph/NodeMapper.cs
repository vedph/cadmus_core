using Cadmus.Core;
using Fusi.Text.Unicode;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// Nodes mapper. This maps items or part's pins to graph nodes and triples.
    /// </summary>
    public sealed class NodeMapper
    {
        private static readonly UniData _ud = new UniData();

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

            Match m = Regex.Match(title, @"^(?<t>.+)\s+\[(?<k>[\#@])(?<v>[^]]+)\]$");

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

        private NodeResult BuildNode(string sid,
            NodeMapping mapping,
            NodeMappingVariableSet vset)
        {
            NodeResult node = new NodeResult
            {
                SourceType = mapping.SourceType,
                Sid = sid,
                // a node coming from a facet is a class
                IsClass = mapping.SourceType == NodeSourceType.ItemFacet
            };

            // build the node's label following label_template
            node.Label = vset.ResolvePlaceholders(mapping.LabelTemplate).Trim();

            // build the UID prefix
            string prefix = null;
            if (!string.IsNullOrEmpty(mapping.Prefix))
                prefix = vset.ResolvePlaceholders(mapping.Prefix);

            // generate node's UID:
            StringBuilder sb = new StringBuilder();

            // 1: prefix
            if (!string.IsNullOrEmpty(prefix)) sb.Append(prefix);

            // 2: filtered label
            foreach (char c in node.Label)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(char.ToLowerInvariant(_ud.GetSegment(c, true)));
                    continue;
                }
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                    continue;
                }
                if (char.IsWhiteSpace(c)) sb.Append('_');
                if (c == '-' || c == '_') sb.Append(c);
            }
            // ensure the resulting UID is not empty, even though this should
            // never happen
            if (sb.Length == 0) sb.Append('_');

            // 3. append suffix if already present
            node.Uri = _repository.AddUid(sb.ToString(), node.Sid);

            // 4. get a numeric ID for the UID
            node.Id = _repository.AddUri(node.Uri);

            return node;
        }

        private Tuple<TripleResult, NodeResult> BuildTriple(string sid,
            string nodeUid, NodeMapping mapping, NodeMappingVariableSet vset,
            NodeMapperState state)
        {
            // 1: S
            string subjUid;

            // override the default subject if mappings specifies one
            if (!string.IsNullOrEmpty(mapping.TripleS))
            {
                // all the macros here resolve to a UID
                subjUid = vset.ResolveMacro(mapping.TripleS);

                // fail if macro cannot be resolved
                if (subjUid == null)
                {
                    Logger?.LogError("Unable to resolve macro "
                        + mapping.TripleS
                        + " of S mapping " + mapping);
                    return null;
                }
            }
            else
            {
                // else use the node UID as the subject or get it from ancestors if any
                subjUid = nodeUid ?? GetNodeUidFromAncestors(mapping, state);
                if (subjUid == null)
                {
                    Logger?.LogError("Unable to get a subject for triple in mapping "
                        + mapping);
                }
            }

            // 2: P
            string predUid = vset.ResolveMacro(mapping.TripleP);
            // fail if macro cannot be resolved
            if (predUid == null)
            {
                Logger?.LogError("Unable to resolve macro "
                    + mapping.TripleP
                    + " of P mapping " + mapping);
                return null;
            }

            // 3: O (there must be O as we have P)
            if (string.IsNullOrEmpty(mapping.TripleO))
            {
                Logger?.LogError("Missing O specifier in triple mapping "
                    + mapping);
                return null;
            }

            string obj;
            bool objAsUid;
            if (mapping.TripleO[0] == '$')
            {
                // O is a macro to be resolved into a UID/literal
                objAsUid = NodeMappingVariableSet.IsUidMacro(mapping.TripleO);
                obj = vset.ResolveMacro(mapping.TripleO);
                if (obj == null)
                {
                    Logger?.LogError("Unable to resolve macro "
                        + mapping.TripleO
                        + " of O mapping " + mapping);
                    return null;
                }
            }
            else
            {
                // O is a constant UID, as it is
                objAsUid = true;
                obj = mapping.TripleO;
            }

            // prepend O prefix if required
            if (objAsUid && !string.IsNullOrEmpty(mapping.TripleOPrefix))
            {
                string oPrefix = vset.ResolvePlaceholders(mapping.TripleOPrefix);
                obj = oPrefix + obj;
            }

            // build triple
            TripleResult triple;
            if (mapping.IsReversed)
            {
                // reverse implies a non-literal object
                if (!objAsUid)
                {
                    Logger?.LogError("Reversed mapping not applicable " +
                        "to triple with literal: " + mapping + ", SID=" + sid);
                    return null;
                }

                triple = new TripleResult
                {
                    ObjectId = _repository.AddUri(subjUid),
                    ObjectUri = subjUid,
                    PredicateId = _repository.AddUri(predUid),
                    PredicateUri = predUid,
                    SubjectId = _repository.AddUri(obj),
                    SubjectUri = obj,
                    Sid = sid
                };
            }
            else
            {
                triple = new TripleResult
                {
                    SubjectId = _repository.AddUri(subjUid),
                    SubjectUri = subjUid,
                    PredicateId = _repository.AddUri(predUid),
                    PredicateUri = predUid,
                    ObjectId = objAsUid ? _repository.AddUri(obj) : 0,
                    ObjectUri = objAsUid ? obj : null,
                    ObjectLiteral = objAsUid ? null : obj,
                    Sid = sid
                };
            }

            // build O node if required; when building a triple whose O is
            // an object, this object is usually an existing resource, whatever
            // its source. If it is external, it has no SID and is a manually
            // input object; if it comes from other mappings, it will have its
            // own source. So all what we want here is that the O node exists;
            // when it already exists, we will not update it as any of its data
            // is subject to manual editing or is managed by another source.
            NodeResult objNode = null;
            if (objAsUid)
            {
                objNode = _repository.GetNode(triple.ObjectId) ?? new NodeResult
                {
                    Id = _repository.AddUri(obj),
                    Uri = obj,
                    Label = obj,
                    // all the nodes marked with user source (and thus also
                    // having a null SID) will be added to the database only
                    // when they are not already present
                    SourceType = NodeSourceType.User
                };
            }

            return Tuple.Create(triple, objNode);
        }

        private string GetNodeUidFromAncestors(NodeMapping mapping,
            NodeMapperState state)
        {
            if (mapping.ParentId == 0) return null;

            for (int i = state.MappingPath.Count - 1; i > -1; i--)
            {
                int mappingId = state.MappingPath[i];
                if (state.MappedUris.ContainsKey(mappingId))
                    return state.MappedUris[mappingId];
            }
            return null;
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

            // generate node if any
            NodeResult node = null;
            if (!string.IsNullOrEmpty(mapping.LabelTemplate))
            {
                node = BuildNode(state.Sid, mapping, vset);

                // add node to set
                state.AddNode(node, mapping.Id);

                // eventually set the slot value if the mapping requires it.
                // It will be used below when building triples. Until here,
                // the $slot macro can't be resolved, but this is not an issue
                // because it is meaningful only for triples.
                if (!string.IsNullOrEmpty(mapping.Slot))
                {
                    string key = vset.ResolvePlaceholders(mapping.Slot);
                    if (key == null)
                    {
                        Logger?.LogError("Unable to resolve slot macro " +
                            mapping.Slot + " in mapping " + mapping);
                    }
                    else
                    {
                        state.SlotUris[key] = node.Uri;
                        // update slot-dependent variables
                        vset.SetValues(state, true);
                    }
                }
            }

            // if there is a triple, collect SPO from triple_s, triple_p,
            // triple_o (triple_o_prefix) and reversed, then generate it
            // together with its O's node unless it's a literal or already exists
            if (!string.IsNullOrEmpty(mapping.TripleP))
            {
                var to = BuildTriple(state.Sid, node?.Uri, mapping, vset, state);
                if (to != null)
                {
                    if (to.Item2 != null && state.Nodes.All(n => n.Id != to.Item2.Id))
                        state.Nodes.Add(to.Item2);
                    state.Triples.Add(to.Item1);
                }
            }

            // children mappings
            IList<NodeMapping> children = state.Part == null
                ? _repository.FindMappingsFor(state.Item, mapping.Id)
                : _repository.FindMappingsFor(state.Item, state.Part,
                    state.PinName, mapping.Id);

            // process children
            if (children.Count > 0)
            {
                state.MappingPath.Add(mapping.Id);
                foreach (var child in children) ApplyMapping(child, state);
                state.MappingPath.RemoveAt(state.MappingPath.Count - 1);
            }
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
            }
        }

        /// <summary>
        /// Maps the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="set">The set to use, unless you are starting a new set.
        /// </param>
        /// <returns>The generated set of nodes and triples.</returns>
        /// <exception cref="ArgumentNullException">item</exception>
        public GraphSet MapItem(IItem item, GraphSet set = null)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Logger?.LogInformation("Mapping " + item);
            NodeMapperState state = new NodeMapperState
            {
                Item = item
            };
            Map(state);

            if (set == null) return new GraphSet(state.Nodes, state.Triples);
            set.AddNodes(state.Nodes);
            set.AddTriples(state.Triples);
            return set;
        }

        /// <summary>
        /// Maps the specified part's pin(s).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="part">The part.</param>
        /// <param name="pins">Name/value pairs for pins.</param>
        /// <param name="set">The set to use, unless you are starting a new set.
        /// </param>
        /// <returns>The generated set of nodes and triples.</returns>
        /// <exception cref="ArgumentNullException">item, part, pins</exception>
        public GraphSet MapPins(IItem item, IPart part, IList<Tuple<string,string>> pins,
            GraphSet set = null)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (part == null) throw new ArgumentNullException(nameof(part));
            if (pins == null) throw new ArgumentNullException(nameof(pins));

            // we need to map the item first, as this provides context
            // (e.g. to resolve $item and the like)
            NodeMapperState state = new NodeMapperState
            {
                Item = item
            };
            Map(state);

            // reset data from state except for data we want to preserve
            state.Sid = null;
            state.GroupOrdinal = 0;
            state.MappingPath.Clear();
            // nodes and triples from items are already present in the store
            // (whenever user saves, an item gets mapped)
            state.Nodes.Clear();
            state.Triples.Clear();

            // map pins reusing state
            foreach (var pin in pins)
            {
                Logger?.LogInformation($"Mapping {part.Id}/{pin.Item1}=" +
                    (pin.Item2.Length > 80
                     ? pin.Item2.Substring(0, 80) + "..." : pin.Item2));

                state.Part = part;
                state.PinName = pin.Item1;
                state.PinValue = pin.Item2;
                Map(state);
            }

            if (set == null) return new GraphSet(state.Nodes, state.Triples);
            set.AddNodes(state.Nodes);
            set.AddTriples(state.Triples);
            return set;
        }
    }
}
