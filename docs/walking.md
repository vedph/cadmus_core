# Walking Graph

- [Walking Graph](#walking-graph)
  - [Example: Step 1](#example-step-1)
  - [Example: Step 2](#example-step-2)
  - [Example: Step 3](#example-step-3)
  - [Example: Step 4](#example-step-4)
  - [Example: Step 5](#example-step-5)
  - [Walker Filters](#walker-filters)
    - [Walking from Node](#walking-from-node)
    - [Walking from Property Group](#walking-from-property-group)

When editing the graph, it is often useful to explore it starting from a specified node.

The idea is that you start focusing on a single node, e.g. a person; and then you can virtually walk across all its links to other nodes, freely choosing one path after another. So, instead of expanding all the edges and nodes from a single node, which would quickly become messy and confusing, we are letting users pick just want they want to see.

For instance, say you start from the node representing Petrarch; you can see a list of links connected to it. As these might quickly grow, we cannot display all of them at once: it would be both confusing and slow. Rather, we adopt a filtering and paging strategy, consisting of three sets of links:

- "outbound" links: links to nodes, i.e. triples where our chosen origin node is the subject of a triple having a non-literal node as its object.
- "literal" links (always outbound by definition): links to literals, i.e. triples where our chosen origin node is the subject of a triple having a _literal_ node as its object.
- "inbound" links, i.e. triples where our chosen origin node is the _object_. On the other end (the subject end) we will necessarily have a non literal node.

Each of these sets has its filters and paging options, so that users can further refine the visualization by removing unnecessary links and nodes.

To avoid polluting the visualization and provide a visual anchor for filtering and paging, we don't directly represent links on the node; rather, we represent groups of links, each with a number equal to the count of all the links of a specific type.

## Example: Step 1

For instance, say we have these triples connected to the origin node representing Petrarch:

```turtle
x:guys/francesco_petrarca rdfs:label "Petrarca"@ita
x:guys/francesco_petrarca rdfs:label "Petrarch"@eng
x:guys/francesco_petrarca rdfs:label "Petrarcha"@lat

x:events/birth crm:P98_brought_into_life x:guys/francesco_petrarca
x:events/death crm:P93_took_out_of_existence x:guys/francesco_petrarca
```

Of course, in turn these nodes are connected to other nodes in the graph; but at start we limit ourselves to the triples involving our origin node, either as a subject or as an object.

This first step is shown in Figure 1:

![walking graph - 1](img/graph-walk-0.png)

*Figure 1 - Walking from origin node*

With relation to the origin node, the first 3 triples are outbound, while the last 2 triples are inbound:

- the outbound triples all share the same predicate (`rdfs:label`), so we just have a single shape out of the origin node, with its count=3. Eventually, we might also want to draw these shapes proportionally to their counts.
- the inbound triples have each one its own predicate; so we have two shapes connected to the origin node, with their count=1.

>Note: getting a page of groups of triples connected to an origin node is done via repository `GetTripleGroups`, which receives the paging, filtering, and sorting parameters, and returns the requested page with triple predicates and their totals.

So, we start with a minimalist visualization, where the origin node is linked to shapes representing groups of triples linked to that node, with their count. The user can now start walking in any direction.

## Example: Step 2

Say that now the user picks the `rdfs:label` shape linked to the origin node. In this case, this expands into 3 literals, representing 3 labels attached to Petrarch in different languages. The result is represented in Figure 2.

![walking graph - 2](img/graph-walk-1.png)

*Figure 2 - Walking from the rdfs:label group*

>Note: this is done via repository `GetLinkedNodes` and `GetLinkedLiterals`.

Once we have nodes, the walking process can cyclically resume from them, by focusing on each desired node in turn.

## Example: Step 3

Going on, the user now picks the `crm:P98_brought_into_life` property group, connected to the origin node as an inbound link. It projects a further node, which is the `petrarch_birth` event (Figure 3).

![walking graph - 3](img/graph-walk-2.png)

*Figure 3 - Walking from the crm:P98_brought_into_life group*

## Example: Step 4

Again, the user picks the newly added `petrarch_birth` node. This projects another property group, corresponding to predicate `a` (Figure 4).

![walking graph - 4](img/graph-walk-3.png)

*Figure 4 - Walking from the petrarch_birth node*

## Example: Step 5

Finally, the user picks this newly added `a` group, which in turn projects a `crm:E67_birth` node, which is the object of the triple. In other terms, this means that the `petrarch_birth` event is classified as a birth event (Figure 5).

![walking graph - 5](img/graph-walk-4.png)

*Figure 5 - Walking from the a group of petrarch_birth node*

The example might continue indefinitely, visualizing a graph built by progressive additions, yet compact and readable thanks to paging and filtering. We are thus walking the graph piece by piece, driven only by our choices. This helps users focus on their paths, without being confused by the huge amount of other connections. Each property group shape is the handle used to further refine filtering for its projections, and for navigating across their pages.

So, in the end we just have 3 types of shapes in this graph:

1. shapes representing non-literal nodes (N). These project groups of predicates from an origin node.
2. shapes representing property groups (P), i.e. groups of links sharing the same predicate, and the same node as one of the terms of the triple, either the subject ("outbound links") or the object ("inbound links"). These project nodes from a predicate (in turn connected to another node, being either its subject or its object).
3. shapes representing literals (L). These are terminals and do not project anything.

## Walker Filters

While walking, the nodes in the graph work also has handles to control paging, filtering, and sorting of triples and nodes.

There are two scenarios in walking, according to the current origin: from node, or from a properties group.

### Walking from Node

When walking from a node, the node can project property groups, either outbound (the node being the subject) or inbound (the node being the object):

1. **outbound**: required filter is _subject ID_ = origin node ID.
2. **inbound**: required filter is _object ID_ = origin node ID.

The filter model (`TripleFilter`) includes these properties:

- `PageNumber`
- `PageSize`
- `SubjectId`: equal to the origin node for outbound links.
- `PredicateIds`: a whitelist of predicate IDs. At least 1 of these must be matched.
- `NotPredicateIds`: a blacklist of predicate IDs. None of these must be matched.
- `HasLiteralObject`: true to match only triples having a literal object; false for the inverse; null to match both.
- `ObjectId`: the ID of the triple's object to match.
- `Sid`: the SID to match.
- `IsSidPrefix`: true to treat the SID filter as a prefix.
- `Tag`: the tag to match (empty to match null tags, null to ignore tags in matching).
- `LiteralPattern`: the pattern of the triple's literal object value to match.
- `LiteralType`: the type of the triple's literal object to match.
- `LiteralLanguage`: the language of the triple's literal object to match.
- `MinLiteralNumber`: the minimum numeric value for a numeric literal object.
- `MaxLiteralNumber`: the maximum numeric value for a numeric literal object.

### Walking from Property Group

When walking from a property group, the group can project nodes of three types:

1. **outbound** non-literals: required filters are _predicate ID_ = group's property ID; _subject ID_ = the ID of the subject node linked to this property group. The result is a page of nodes.
2. **outbound** literals: required filters are _predicate ID_ = group's property ID; _subject ID_ = the ID of the subject node linked to this property group. The result is a page of triples.
3. **inbound**: required filters are _predicate ID_ = group's property ID; _object ID_ = the ID of the object node linked to this property group (literal=false is implied by the fact that the nodes being fetched are subjects). The result is a page of nodes.

The filter model is different:

(1) linked node (`LinkedNodeFilter`):

- `PageNumber`
- `PageSize`
- `OtherNodeId`: the other node identifier, which is the subject node ID when `IsObject` is true, otherwise the object node ID.
- `PredicateId`: the predicate ID to match.
- `IsObject`: true if the node to match is the object of the triple; false if it's the subject.
- `Uid`: any portion of the node ID to match.
- `IsClass`: true to match nodes representing classes; false to match nodes not representing classes; null to match both.
- `Tag`: the tag to match (empty to match null tags, null to ignore tags in matching).
- `Label`: any portion of the label to match.
- `SourceType`: the source type to match.
- `Sid`: the SID to match.
- `IsSidPrefix`: true to treat the SID filter as a prefix.
- `ClassIds`: the classes IDs to match. When specified, the node must derive either directly or indirectly from any of the classes.

(2) linked literal (`LinkedLiteralFilter`):

- `PageNumber`
- `PageSize`
- `SubjectId`: the subject ID in the triple including the literal to match.
- `PredicateId`: the property identifier in the triple including the  literal to match.
- `LiteralPattern`: the pattern of the triple's literal object value to match.
- `LiteralType`: the type of the triple's literal object to match.
- `LiteralLanguage`: the language of the triple's literal object to match.
- `MinLiteralNumber`: the minimum numeric value for a numeric literal object.
- `MaxLiteralNumber`: the maximum numeric value for a numeric literal object.
