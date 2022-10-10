# Cadmus

Cadmus data layer and business layer.

Please see the conceptual documentation under [cadmus_doc](https://github.com/vedph/cadmus_doc).

## History

### 4.0.5

- 2022-10-10: updated packages.

### 4.0.4

- 2022-08-21:
  - updated packages.
  - added `FlagMatching` to item/history item filters. This does not affect the previous behavior, which remains the default one; yet, now it is possible to match the flags value with these modes:
    - all the bits set (the default, as before);
    - any of the bits set;
    - all the bits clear;
    - any of the bits clear.
Consequently, the `IHasFlags` interface has been added, so that both filters can be handled in the same way for what concerns flags.

### 4.0.3

- 2022-08-07: updated packages (`Fusi.Config`).

### 4.0.2

- 2022-08-06: updated packages.

### 4.0.1

- 2022-05-31: replaced legacy pin-based graph with new [Cadmus.Graph](https://github.com/vedph/cadmus-graph).

### 3.0.1

- 2022-05-18: upgraded dependencies to NET 6.

### 3.0.0

- 2022-04-29: upgraded to .NET 6.

### 2.x.x

- 2022-02-14: added resiliency to ItemIndexer. Now a failing indexing operation is logged (via the new Logger property, when set) and indexing then continues.
- 2022-01-02: moved parts-related libraries into external solutions. For general parts, this implies relocating their namespaces as libraries were renamed from `Cadmus.Parts` to `Cadmus.General.Parts`, and from `Cadmus.Seed.Parts` to `Cadmus.Seed.General.Parts`.
- 2021-11-20: changed graph node filter class IDs type.
- 2021-11-08: changed pins for categories and keywords parts so that an unfiltered copy is emitted.
- 2021-11-06: made `Thesaurus` serializable.
- 2021-11-03: added `AddThesaurus` to `IGraphRepository` to allow importing thesaurus-based taxonomies as classes in the graph.
- 2021-10-30: added `IGraphPresetReader` and its JSON implementation to read preset graph nodes and node mappings from an external source. This will be used in configuration via Cadmus tool.
- 2021-10-24: added `InitContext` to `IItemIndexWriter` to allow for lazy graph database initialization.
- 2021-10-15: refactored parts with `DocReferences` to use the new model from bricks.
- 2021-10-08: added CLI core library to let the CLI tool use plugins for repository and part seeder factories.
- 2021-10-06: first backend implementation of the semantic graph subsystem.
