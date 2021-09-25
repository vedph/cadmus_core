﻿using System.Text;

namespace Cadmus.Index.Graph
{
    /// <summary>
    /// The mapping for a node.
    /// </summary>
    public class NodeMapping
    {
        /// <summary>
        /// The source types letters (one for each value of
        /// <see cref="NodeSourceType"/>).
        /// </summary>
        public const string SOURCE_TYPES = "UIFGP";

        /// <summary>
        /// Gets or sets the mapping identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent mapping identifier, or 0 when this mapping
        /// has no parent.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the type of the source.
        /// </summary>
        public NodeSourceType SourceType { get; set; }

        /// <summary>
        /// Gets or sets a user friendly name for this mapping.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordinal. This is usually 0, but can be set to
        /// any number to force some mappings to be executed before or after
        /// some other mappings in the context of the same depth level
        /// in the mappings hierarchy.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// Gets or sets the item's facet filter (a facet ID).
        /// </summary>
        public string FacetFilter { get; set; }

        /// <summary>
        /// Gets or sets the item's group filter (a regular expression).
        /// </summary>
        public string GroupFilter { get; set; }

        /// <summary>
        /// Gets or sets the flags filter (a number greater than 0; all the bits
        /// in that number must be matched).
        /// </summary>
        public int FlagsFilter { get; set; }

        /// <summary>
        /// Gets or sets the title filter (a regular expression).
        /// </summary>
        public string TitleFilter { get; set; }

        /// <summary>
        /// Gets or sets the type ID of the part.
        /// </summary>
        public string PartType { get; set; }

        /// <summary>
        /// Gets or sets the part role ID.
        /// </summary>
        public string PartRole { get; set; }

        /// <summary>
        /// Gets or sets the name of the pin. By convention, the name may end
        /// with 1 or more <c>@*</c> suffixes, each representing a scope (EID)
        /// for the pin. For instance, for a scoped pin <c>color@angel-1v</c>
        /// (=<c>color</c> pin assigned to an entry identified as <c>angel-1v</c>),
        /// the pin name to match is <c>color@*</c>, where <c>*</c>=any
        /// characters different from <c>@</c>.
        /// </summary>
        public string PinName { get; set; }

        /// <summary>
        /// Gets or sets the prefix. The optional prefix to prepend to the
        /// target UID. It can have placeholders <c>{label-prefix}</c> (when
        /// the prefix is extracted from label according to UID generation
        /// conventions), <c>{facet-id}</c> and <c>{group-id}</c>, all meaningful
        /// only when dealing with items.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the label template. This optional template can have
        /// placeholders <c>{title}</c> (=item's title, after processing
        /// conventions), <c>{group-id}</c> (=group ID), <c>{facet-id}</c>
        /// (=facet ID), <c>{pin-name}</c> (=source pin's name, without the EID
        /// <c>@...</c> suffix if any), <c>{pin-eid}</c> (=source pin's EID
        /// suffix; this can be followed by <c>:N</c> where N is a 1-based,
        /// positive or negative integer, representing the EID component to pick
        /// from the whole EID suffix when this is composite), <c>{pin-value}</c>
        /// (=the pin's value). This is used to generate a label for the target
        /// node (in turn, the label is used as a source for building the UID).
        /// It is null when the mapping just produces links.
        /// </summary>
        public string LabelTemplate { get; set; }

        /// <summary>
        /// Gets or sets the triple subject. Used to specify a subject different
        /// from the parent mapping's node (which is the default): values are
        /// the same of <see cref="TripleO"/>.
        /// </summary>
        public string TripleS { get; set; }

        /// <summary>
        /// Gets or sets the triple's predicate. This is null when no triple
        /// is required (just creating a target node), or the UID of a predicate,
        /// or macro <c>$pin-name</c>.
        /// </summary>
        public string TripleP { get; set; }

        /// <summary>
        /// Gets or sets the triple's object. This is null when no triple is
        /// required, or the UID of an object, or a macro among: <c>$parent</c>
        /// =the UID of the node generated by the parent mapping (equivalent to
        /// <c>$ancestor:1</c>); <c>$ancestor:N</c>=the UID of the node generated
        /// by the ancestor mapping specified by N, like 1=parent, 2=parent of
        /// parent, etc.; <c>$item</c>=the item's UID; <c>$group</c>=the group's
        /// UID (for a composed group ID this is the bottom component's UID;
        /// else use <c>$group:N</c> where 1=root component, 2=child of 1, etc;
        /// <c>$facet</c>=the facet's UID; <c>$label</c>=the literal value got
        /// from processing the item's label; <c>$dsc</c>=the literal value got
        /// from item's description; <c>$pin-value</c>=the literal value got from
        /// the source pin; <c>$pin-uid</c>=the UID got from the source pin value,
        /// when this is an EID. Both <c>$pin-value</c> and <c>$pin-uid</c>
        /// draw their value from the pin's value, but this value is used either
        /// as a literal or as an UID.
        /// </summary>
        public string TripleO { get; set; }

        /// <summary>
        /// Gets or sets the optional triple's object prefix, to prepend to
        /// the newly generated node with O role in the triple. This can be used
        /// when nodes are implicitly created when setting their relation with
        /// the subject. Value and processing is the same as <see cref="Prefix"/>.
        /// </summary>
        public string TripleOPrefix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the triple for this mapping
        /// is "reversed", i.e. when S and O (non-literal) as defined by
        /// this mapping should be swapped, thus reversing the generated triple.
        /// </summary>
        public bool IsReversed { get; set; }

        /// <summary>
        /// Gets or sets an optional human-readable description of this mapping.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name);

            sb.Append(": [").Append(SOURCE_TYPES[(int)SourceType]).Append("] ");

            sb.Append('#').Append(Id);
            if (ParentId > 0) sb.Append("<-").Append(ParentId);

            if (!string.IsNullOrEmpty(FacetFilter)) sb.Append(" ff");
            if (!string.IsNullOrEmpty(GroupFilter)) sb.Append(" gf");
            if (FlagsFilter != 0) sb.Append(" lf");
            if (!string.IsNullOrEmpty(TitleFilter)) sb.Append(" tf");

            if (!string.IsNullOrEmpty(PartType)) sb.Append(' ').Append(PartType);
            if (!string.IsNullOrEmpty(PartRole)) sb.Append(':').Append(PartRole);
            if (!string.IsNullOrEmpty(PinName)) sb.Append('/').Append(PartRole);

            if (!string.IsNullOrEmpty(TripleP)) sb.Append(" T");
            if (IsReversed) sb.Append('^');

            return sb.ToString();
        }
    }
}
