using Fusi.Tools.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using DevLab.JmesPath;
using System.Collections.Generic;

namespace Cadmus.Graph;

/// <summary>
/// JSON-based node mapper.
/// <para>Tag: <c>node-mapper.json</c>.</para>
/// </summary>
/// <seealso cref="NodeMapper" />
/// <seealso cref="INodeMapper" />
[Tag("node-mapper.json")]
public sealed class JsonNodeMapper : NodeMapper, INodeMapper
{
    private readonly JmesPath _jmes;
    private JsonDocument? _doc;
    private int _sourceType;
    private string? _lastSid;
    private string? _lastSidSource;

    public JsonNodeMapper()
    {
        _jmes = new();
    }

    protected override string ResolveDataExpression(string expression)
    {
        if (_doc == null) return "";

        // corner case: "." just means current value.
        // Also, any value kind not being an object or an array is just
        // a primitive, so we end up returning the current value.
        if (expression == "." ||
            (_doc.RootElement.ValueKind != JsonValueKind.Object
            && _doc.RootElement.ValueKind != JsonValueKind.Array))
        {
            return _doc.RootElement.ToString();
        }

        // an array might include a single primitive, so return it
        if (_doc.RootElement.ValueKind == JsonValueKind.Array
            && _doc.RootElement.GetArrayLength() == 1
            && _doc.RootElement[0].ValueKind != JsonValueKind.Object
            && _doc.RootElement[0].ValueKind != JsonValueKind.Array)
        {
            return _doc.RootElement[0].ToString();
        }

        // else evaluate expression from current object/array
        string json = JsonSerializer.Serialize(_doc.RootElement);
        string? result = _jmes.Transform(json, expression);
        if (string.IsNullOrEmpty(result)) return "";

        return JsonDocument.Parse(result).RootElement.ToString() ?? "";
    }

    private void AddNodes(string sid, NodeMapping mapping, GraphSet target)
    {
        foreach (var p in mapping.Output!.Nodes)
        {
            string uri = UidBuilder.BuildUid(ResolveTemplate(p.Value.Uid!, true),
                sid);
            UriNode node = new()
            {
                Uri = uri,
                SourceType = _sourceType,
                Sid = sid,
                Label = string.IsNullOrEmpty(p.Value.Label) ?
                    uri : ResolveTemplate(p.Value.Label, false)
            };
            ContextNodes[p.Key] = node;
            target.Nodes.Add(node);
        }
    }

    private void AddTriples(string sid, NodeMapping mapping, GraphSet target)
    {
        int n = 0;
        foreach (MappedTriple tripleSource in mapping.Output!.Triples)
        {
            n++;
            if (string.IsNullOrEmpty(tripleSource.S))
            {
                throw new CadmusGraphException(
                    $"Undefined triple subject at mapping #{n}: {mapping}");
            }
            if (string.IsNullOrEmpty(tripleSource.P))
            {
                throw new CadmusGraphException(
                    $"Undefined triple predicate at mapping #{n}: {mapping}");
            }
            if (string.IsNullOrEmpty(tripleSource.O) && tripleSource.OL == null)
            {
                throw new CadmusGraphException(
                    $"Undefined triple object at mapping #{n}: {mapping}");
            }

            UriTriple triple = new()
            {
                Sid = sid,
                SubjectUri = ResolveTemplate(tripleSource.S!, true),
                // P=a becomes rdf:type
                PredicateUri = ResolveTemplate(tripleSource.P == "a"
                    ? "rdf:type": tripleSource.P!, true),
                ObjectUri = tripleSource.O != null
                    ? ResolveTemplate(tripleSource.O!, true) : null,
                ObjectLiteral = tripleSource.OL != null
                    ? ResolveTemplate(tripleSource.OL, false)
                    : null
            };
            LiteralHelper.AdjustLiteral(triple);
            target.Triples.Add(triple);
        }
    }

    private void BuildOutput(string? sid, NodeMapping mapping, GraphSet target)
    {
        if (mapping.Output == null) return;

        // metadata
        if (mapping.Output.HasMetadata)
        {
            foreach (var p in mapping.Output.Metadata)
                Data[p.Key] = ResolveTemplate(p.Value!, false);
        }

        if (!string.IsNullOrEmpty(sid))
        {
            // nodes
            if (mapping.Output.HasNodes) AddNodes(sid, mapping, target);
            // triples
            if (mapping.Output.HasTriples) AddTriples(sid, mapping, target);
        }
    }

    private void ApplyMapping(string? sid, string json, NodeMapping mapping,
        GraphSet target, int itemIndex = -1)
    {
        Logger?.LogDebug("Mapping {mapping}", mapping);

        if (IsMappingTracingEnabled)
        {
            IList<NodeMapping>? traced = null;
            if (Data.TryGetValue(APPLIED_MAPPING_LIST, out object? mappings))
                traced = mappings as IList<NodeMapping>;
            if (traced == null)
            {
                traced = [];
                Data[APPLIED_MAPPING_LIST] = traced;
            }
            // avoid sequences of duplicates (which happen in arrays)
            if (traced.Count == 0 || traced[^1] != mapping)
                traced.Add(mapping);
        }

        // if we're dealing with an array's item, we do not want to compute
        // the mapping's expression, but just use the received json
        // representing the item itself.
        string? result;
        if (itemIndex == -1)
        {
            try
            {
                result = mapping.Source == "."
                    ? json
                    : _jmes.Transform(json, mapping.Source);
                // provide index anyway, because some mappings might need it,
                // even if effectively used only for array items later
                Data["index"] = -1;
            }
            catch (Exception ex)
            {
                throw new AggregateException(
                    $"Eval error \"{ex.Message}\" at mapping {mapping}", ex);
            }
        }
        else result = json;

        // scalar pattern filter
        if (!mapping.IsScalarMatch(result)) return;

        // get the result into the current document
        _doc = JsonDocument.Parse(result);

        // generate SID if specified in mapping
        if (mapping.Sid != null)
        {
            sid = ResolveTemplate(mapping.Sid!, false);
            if (!string.IsNullOrEmpty(sid))
            {
                _lastSid = sid;
                _lastSidSource = mapping.Sid;
            }
        }
        else if (itemIndex > -1 && _lastSidSource?.IndexOf(
            "$index", StringComparison.Ordinal) > -1)
        {
            // corner case: inherited SID with an array item: resolve it
            // again if including $index
            sid = ResolveTemplate(mapping.Sid!, false);
            if (!string.IsNullOrEmpty(sid)) _lastSid = sid;
        }

        // process document with result, according to its root type
        switch (_doc.RootElement.ValueKind)
        {
            // null or undefined does not trigger output
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                break;
            case JsonValueKind.Object:
                if (mapping.Output != null)
                {
                    if (string.IsNullOrEmpty(sid))
                    {
                        if (_lastSid == null && !mapping.Output.HasNoGraph)
                        {
                            throw new CadmusGraphException(
                                $"Undefined SID for mapping {mapping}");
                        }
                        sid = _lastSid;
                    }
                    BuildOutput(sid, mapping, target);
                }
                break;
            // an array does not trigger output, but applies its mapping
            // to each of its items
            case JsonValueKind.Array:
                // for each array's item, apply mapping and its children
                int index = 0;
                foreach (JsonElement item in _doc.RootElement.EnumerateArray())
                {
                    Data["index"] = index;
                    ApplyMapping(sid, item.GetRawText(),
                        mapping, target, index++);
                }
                Data.Remove("index");
                break;
            // else it's a terminal, build output
            default:
                // set current leaf variable
                Data["."] = _doc.RootElement.ToString();
                if (mapping.Output != null)
                {
                    if (string.IsNullOrEmpty(sid))
                    {
                        if (_lastSid == null && !mapping.Output.HasNoGraph)
                        {
                            throw new CadmusGraphException(
                                $"Undefined SID for mapping {mapping}");
                        }
                        sid = _lastSid;
                    }
                    BuildOutput(sid, mapping, target);
                }
                break;
        }
        _doc = null;

        // process this mapping's children recursively
        if (mapping.HasChildren)
        {
            foreach (NodeMapping child in mapping.Children!)
                ApplyMapping(sid, result, child, target);
        }
    }

    /// <summary>
    /// Map the specified source into the <paramref name="target"/> graphset.
    /// </summary>
    /// <param name="source">The source object, here a JSON string.</param>
    /// <param name="mapping">The mapping to apply.</param>
    /// <param name="target">The target graphset.</param>
    /// <exception cref="ArgumentNullException">mapping or target</exception>
    public void Map(object source, NodeMapping mapping, GraphSet target)
    {
        ArgumentNullException.ThrowIfNull(mapping);
        ArgumentNullException.ThrowIfNull(target);

        // reset state
        _sourceType = 0;
        _lastSid = null;
        _doc = null;

        // source is JSON
        string? json = source as string;
        if (string.IsNullOrEmpty(json)) return;

        // set source type once for all the descendants
        _sourceType = mapping.SourceType;

        ApplyMapping(null, json, mapping, target);
    }
}
