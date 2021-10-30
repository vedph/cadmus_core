# Cadmus

Cadmus data layer and business layer.

Please see the conceptual documentation under [cadmus_doc](https://github.com/vedph/cadmus_doc).

## History

- 2021-10-30: added `IGraphPresetReader` and its JSON implementation to read preset graph nodes and node mappings from an external source. This will be used in configuration via Cadmus tool.
- 2021-10-24: added `InitContext` to `IItemIndexWriter` to allow for lazy graph database initialization.
- 2021-10-15: refactored parts with `DocReferences` to use the new model from bricks.
- 2021-10-08: added CLI core library to let the CLI tool use plugins for repository and part seeder factories.
- 2021-10-06: first backend implementation of the semantic graph subsystem.
