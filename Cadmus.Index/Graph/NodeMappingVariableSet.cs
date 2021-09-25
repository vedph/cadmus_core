﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A set of variables extracted from a node mapping.
    /// </summary>
    /// <remarks>Variables are placeholders (delimited within <c>{}</c> in
    /// values) and macros (whole values prefixed with <c>$</c>). Placeholders
    /// and macros with the same name (without considering the <c>$</c> prefix)
    /// target the same value; i.e. any of the placeholders can be used as a
    /// macro by prefixing it with <c>$</c>.
    /// <para>Currently defined placeholders:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>title</term>
    /// <description>the title (without eventual convention-based suffixes).
    /// </description>
    /// </item>
    /// <item>
    /// <term>title-prefix</term>
    /// <description>the prefix extracted from an item's title.</description>
    /// </item>
    /// <item>
    /// <term>title-uid</term>
    /// <description>the UID extracted from an item's title.</description>
    /// </item>
    /// <item>
    /// <term>facet-id</term>
    /// <description>the facet ID from the item.</description>
    /// </item>
    /// <item>
    /// <term>group-id:N</term>
    /// <description>the group ID from the item. The optional <c>:N</c> refers
    /// to the component (1-N) of a composite group ID. Composite group IDs
    /// have their components separated by slashes.</description>
    /// </item>
    /// <item>
    /// <term>dsc</term>
    /// <description>the item's description.</description>
    /// </item>
    /// <item>
    /// <term>pin-name</term>
    /// <description>the source pin's name, without the EID <c>@...</c> suffix
    /// if any.</description>
    /// </item>
    /// <item>
    /// <term>pin-value</term>
    /// <description>the source pin's value.</description>
    /// </item>
    /// <item>
    /// <term>pin-eid</term>
    /// <description>the source pin's EID suffix if any. The optional <c>:N</c>
    /// refers to the component (1-N) of a composite EID suffix. In these
    /// suffixes components are separated by <c>@</c> characters.</description>
    /// </item>
    /// </list>
    /// <para>Currently defined macros:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>$parent</term>
    /// <description>shortcut for <c>$ancestor:1</c>.</description>
    /// </item>
    /// <item>
    /// <term>$ancestor:N</term>
    /// <description>the UID of the node generated by the ancestor mapping
    /// specified by N: 1=parent, 2=parent of parent, and so forth. If N is
    /// not specified it defaults to 1.</description>
    /// </item>
    /// <item>
    /// <term>$item</term>
    /// <description>the UID of the node generated by the item's mapping.
    /// The item is either the source item, or the item the source part belongs
    /// to.</description>
    /// </item>
    /// <item>
    /// <term>$group:N</term>
    /// <description>the UID of the node generated by the item's group mapping.
    /// For composed group IDs, this is the bottom component's UID; otherwise
    /// use 1=root component, 2=child of 1, etc.</description>
    /// </item>
    /// <item>
    /// <term>$facet</term>
    /// <description>the UID of the node generated by the item's facet ID
    /// mapping.</description>
    /// </item>
    /// <item>
    /// <term>$pin-uid</term>
    /// <description>the pin's value, as an UID. The value is equal to
    /// <c>pin-value</c>, but the directive is different, as this refers to an
    /// object node rather than to a literal value.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class NodeMappingVariableSet
    {
        private static readonly Regex _plhRegex
            = new Regex(@"\{(?<id>(?<n>[^:\}]+)(?::(?<a>[0-9]+))?)\}");
        private static readonly Regex _mcrRegex
            = new Regex(@"\$(?<id>(?<n>[^:]+)(?::(?<a>[0-9]+))?)");

        private readonly Dictionary<string, NodeMappingVariable> _vars;

        /// <summary>
        /// Gets the variables count.
        /// </summary>
        public int Count => _vars.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeMappingVariableSet"/>
        /// class.
        /// </summary>
        public NodeMappingVariableSet()
        {
            _vars = new Dictionary<string, NodeMappingVariable>();
        }

        private void LoadVariableFromMatch(Match m)
        {
            string id = m.Groups["id"].Value;
            if (_vars.ContainsKey(id)) return;

            _vars[id] = new NodeMappingVariable
            {
                Id = id,
                Name = m.Groups["n"].Value,
                Argument = m.Groups["a"].Length > 0
                    ? int.Parse(m.Groups["a"].Value, CultureInfo.InvariantCulture)
                    : 0
            };
        }

        private void LoadPlaceholders(string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            foreach (Match m in _plhRegex.Matches(value))
                LoadVariableFromMatch(m);
        }

        private void LoadMacro(string value)
        {
            if (string.IsNullOrEmpty(value)
                || value.Length < 2
                || !value.StartsWith("$", StringComparison.Ordinal)) return;

            Match m = _mcrRegex.Match(value);
            if (m.Success) LoadVariableFromMatch(m);
        }

        /// <summary>
        /// Adds the specified variable.
        /// </summary>
        /// <param name="v">The variable.</param>
        public void Add(NodeMappingVariable v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            _vars[v.Id] = v;
        }

        /// <summary>
        /// Removes the variable with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Remove(string id) => _vars.Remove(id);

        /// <summary>
        /// Clears this set.
        /// </summary>
        public void Clear() => _vars.Clear();

        /// <summary>
        /// Clears the values for all the variables in this set.
        /// </summary>
        public void ClearValues()
        {
            foreach (NodeMappingVariable v in _vars.Values) v.Value = null;
        }

        /// <summary>
        /// Create a new set of variables by loading placeholders and macros
        /// defined in the specified mapping.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        public static NodeMappingVariableSet LoadFrom(NodeMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));

            NodeMappingVariableSet vars = new NodeMappingVariableSet();

            vars.LoadPlaceholders(mapping.Prefix);
            vars.LoadPlaceholders(mapping.TripleOPrefix);
            vars.LoadPlaceholders(mapping.LabelTemplate);

            vars.LoadMacro(mapping.TripleS);
            vars.LoadMacro(mapping.TripleO);
            vars.LoadMacro(mapping.TripleP);

            return vars;
        }

        /// <summary>
        /// Gets the variable with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Variable or null if not found.</returns>
        /// <exception cref="ArgumentNullException">id</exception>
        public NodeMappingVariable GetVariable(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return _vars.ContainsKey(id) ? _vars[id] : null;
        }

        /// <summary>
        /// Sets the values of all the variables in this set from the specified
        /// data source.
        /// </summary>
        /// <param name="state">The mapper state to use as a data source.</param>
        /// <exception cref="ArgumentNullException">source</exception>
        public void SetValues(NodeMapperState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            // extract title and eventually prefix/uid from title
            Tuple<string, string, string> tpi;
            string[] pinComps;

            foreach (NodeMappingVariable v in _vars.Values)
            {
                switch (v.Name)
                {
                    case "title":
                        tpi = NodeMapper.ParseItemTitle(state.Item.Title);
                        v.Value = tpi.Item1;
                        break;
                    case "title-prefix":
                        tpi = NodeMapper.ParseItemTitle(state.Item.Title);
                        v.Value = tpi.Item2;
                        break;
                    case "title-uid":
                        tpi = NodeMapper.ParseItemTitle(state.Item.Title);
                        v.Value = tpi.Item3;
                        break;
                    case "facet-id":
                        v.Value = state.Item.FacetId;
                        break;
                    case "group-id":
                        // :N = component ordinal (1-N from left to right)
                        if (v.Argument > 0
                            && state.Item.GroupId?.IndexOf('/') > -1)
                        {
                            string[] cc = state.Item.GroupId.Split(new[] { "/" },
                                StringSplitOptions.RemoveEmptyEntries);
                            // no value if N is out of range
                            if (v.Argument <= cc.Length)
                                v.Value = cc[v.Argument - 1];
                        }
                        else v.Value = state.Item.GroupId;
                        break;
                    case "dsc":
                        v.Value = state.Item.Description;
                        break;
                    case "pin-name":
                        // pin's name without EID suffixes
                        if (string.IsNullOrEmpty(state.PinName)) break;
                        pinComps = NodeMapper.ParsePinName(state.PinName);
                        v.Value = pinComps[0];
                        break;
                    case "pin-value":
                    case "pin-uid":
                        v.Value = state.PinValue;
                        break;
                    case "pin-eid":
                        // :N = component's ordinal (1-N from left to right)
                        if (string.IsNullOrEmpty(state.PinName)) break;
                        pinComps = NodeMapper.ParsePinName(state.PinName);
                        if (pinComps.Length == 1 || v.Argument + 1 >= pinComps.Length)
                            break;
                        v.Value = pinComps[v.Argument];
                        break;
                    // macros
                    case "parent":
                    case "ancestor":
                        // :N = ancestor number (1=parent, 2=parent-of-parent...)
                        int upCount = v.Argument == 0 ? 1 : v.Argument,
                            i = state.MappingPath.Count - 1 - upCount;
                        if (i > -1)
                        {
                            int mappingId = state.MappingPath[i];
                            if (state.MappedUris.ContainsKey(mappingId))
                                v.Value = state.MappedUris[mappingId];
                        }
                        break;
                    case "item":
                        if (state.MappedUris.ContainsKey(state.ItemMappingId))
                            v.Value = state.MappedUris[state.ItemMappingId];
                        break;
                    case "group":
                        // :N = group ID (1-N from left to right), 0=non composite
                        // group ID
                        int j = v.Argument > 0 ? v.Argument - 1 : 0;
                        if (j < state.GroupUids.Count)
                            v.Value = state.GroupUids[j];
                        break;
                    case "facet":
                        if (state.MappedUris.ContainsKey(state.FacetMappingId))
                            v.Value = state.MappedUris[state.FacetMappingId];
                        break;
                }
            }
        }

        /// <summary>
        /// Resolves the placeholders in the specified template. All the
        /// placeholders which cannot be resolved are removed from the template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>Template with resolved placeholders.</returns>
        /// <exception cref="ArgumentNullException">template</exception>
        public string ResolvePlaceholders(string template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            return _plhRegex.Replace(template, (Match m) =>
            {
                return _vars.ContainsKey(m.Groups["id"].Value) ?
                    _vars[m.Groups["id"].Value].Value : "";
            });
        }

        /// <summary>
        /// Determines whether the specified macro ($...) returns a UID.
        /// </summary>
        /// <param name="macro">The macro, including its <c>$</c> prefix.</param>
        /// <returns><c>true</c> if the macro returns a UID; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUidMacro(string macro)
        {
            return macro == "$title-uid" || macro == "$parent"
                || macro.StartsWith("$ancestor", StringComparison.Ordinal)
                || macro == "$item"
                || macro == "$facet"
                || macro == "$pin-uid"
                || macro.StartsWith("$group", StringComparison.Ordinal);
        }

        /// <summary>
        /// Resolves the macro for the specified value.
        /// </summary>
        /// <param name="value">The value (a macro starting with <c>$</c>,
        /// or a literal).</param>
        /// <returns>Value or resolved macro or null if macro cannot be resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException">template</exception>
        public string ResolveMacro(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!value.StartsWith("$", StringComparison.Ordinal)
                || value.Length < 2)
            {
                return value;
            }

            string id = value.Substring(1);
            return _vars.ContainsKey(id) ? _vars[id].Value : null;
        }
    }
}
