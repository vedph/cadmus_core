# Cadmus

Cadmus data layer and business layer.

👀 [Cadmus Page](https://myrmex.github.io/overview/cadmus/)

## Projects

**Core**:

- Cadmus.Core
- Cadmus.Core.Test
- Cadmus.Seed
- Cadmus.Mongo
- Cadmus.Mongo.Test

**Index**:

- Cadmus.Index
- Cadmus.Index.Sql
- Cadmus.Index.Sql.Test
- Cadmus.Index.MySql
- Cadmus.Index.MySql.Test
- Cadmus.Index.PgSql

**Graph**:

- Cadmus.Graph
- Cadmus.Graph.Test
- Cadmus.Graph.Extras
- Cadmus.Graph.Ef
- Cadmus.Graph.Ef.Test
- Cadmus.Graph.Ef.PgSql
- Cadmus.Graph.Ef.PgSql.Test
- Cadmus.Graph.Ef.MySql
- Cadmus.Graph.Ef.MySql.Test
- Cadmus.Graph.Api: for testing purposes only.

**Obsolete**: these are old projects preserved here because they can be used as the basis for refactoring current Index data layer to use EF:

- Cadmus.Index.Ef
- Cadmus.Index.Ef.MySql
- Cadmus.Index.Ef.MySql.Test
- Cadmus.Index.Ef.PgSql
- Cadmus.Index.Ef.PgSql.Test

## History

### 9.0.0

- 2025-03-09:
  - fix to query builder for search (allow non alpha characters in pins names except for those characters starting any of the operators).
  - ⚠️ removed legacy field names support from query builder.
  - renamed fields in MySql index scripts (e.g. `itemId` becomes `item_id`).
  - updated packages.

### 8.0.6

- 2025-02-14: updated packages.

### 8.0.5

- 2025-02-08:
  - added settings methods to Cadmus repository.
  - updated packages.

### 8.0.4

- 2025-01-28: updated packages.

### 8.0.2

- 2024-11-29: added `isAdmin` to `FlagDefinition`.

### 8.0.1

- 2024-11-20: updated packages.

### 8.0.0

- 2024-11-13: ⚠️ upgraded to .NET 9. This also implied removing legacy MongoDB.Driver version 2 which still coexisted next to version 3, thus replacing `IMongoQueryable` with `IQueryable`.

### 7.0.4

- 2024-09-27: updated packages.

### 7.0.3

- 2024-05-13: updated packages.

### 7.0.2

- 2024-01-31:
  - updated packages.
  - ⚠️ moved projects from `Cadmus.Graph` solution into this solution, as the graph components are now integral part of Cadmus and their dependencies are easier to handle from a unique solution. This implied aligning the version numbers to 7.0.2 (from Graph 5.0.2).

### 7.0.1

- 2023-11-21:
  - updated packages (no more RC2 for PostgreSql).
  - added optional `Settings` to `PartDefinition`.

### 7.0.0

- 2023-11-18: ⚠️ Upgraded to .NET 8. Please notice that currently this breaks MySql-related components, which anyway are legacy. As soon as an up to date MySql package will be available, this will be fixed.

### 6.1.19

- 2023-10-05: updated graph-related packages.

### 6.1.18

- 2023-09-22: added optional filter to Cadmus repository `GetDistinctGroupIdsAsync`.

### 6.1.17

- 2023-09-22:
  - updated packages.
  - in `EfItemIndexWriter.WritePart`, ensure that the item exists before saving part's pins. This makes indexing more robust, in the event that someone deleted the item while the indexer was working on it.

### 6.1.16

- 2023-09-04: updated packages.

### 6.1.15

- 2023-07-26:
  - updated packages.
  - more logging in `ItemIndexer`.
  - in `EfItemIndexWriter` truncate pin value if length is greater than 500.

### 6.1.13

- 2023-07-01: updated packages.

### 6.1.12

- 2023-06-23: updated packages.

### 6.1.11

- 2023-06-23: updated packages.

### 6.1.10

- 2023-06-21: fixed missing `time_indexed` field in EF pin search.

### 6.1.9

- 2023-06-21: fix to index pin based search in EF.

### 6.1.8

- 2023-06-16: added `ItemGraphFactory`. You should change your code to use this factory (usually via `IItemGraphFactoryProvider`) rather than using the same factory used for item indexes. The graph methods in the `ItemIndexFactory` dealing with graph have been marked as obsolete, and will be removed in future versions.

### 6.1.7

- 2023-06-15: fixes to EF components.

### 6.1.6

- 2023-06-15: public setter for EF index reader/writer connection strings.

### 6.1.5

- 2023-06-15: fixes to EF index reader.

### 6.1.4

- 2023-06-13:
  - adding EF-based index writer/reader.
  - updated packages and version numbers (packages push still to be done).
- 2023-06-07: adding PostgreSql support.

### 6.1.3

- 2023-05-31: updated packages.

### 6.1.2

- 2023-05-29: updated packages.

### 6.1.1

- 2023-05-26: added more pin fields to `SqlQueryBuilderBase`.

### 6.1.0

- 2023-05-23: updated packages (general parts have been updated for the new [AssertedCompositeId](https://github.com/vedph/cadmus-bricks-shell/blob/master/projects/myrmidon/cadmus-refs-asserted-ids/README.md#asserted-composite-id)).

### 6.0.5

- 2023-05-16: added `CreateIndex` method to `IItemIndexWriter`. This allows forcing the creation of an empty index by the writer, which otherwise lazily creates the index when something is to be written only. This can be useful e.g. when you want to import some preset data into the index graph, before starting to write items to it. To do this you can use the Cadmus CLI tool.

### 6.0.4

- 2023-05-16: updated Graph packages for index packages.

### 6.0.3

- 2023-05-011: updated packages.

### 6.0.2

- 2023-02-01: migrated to new components factory. This is a breaking change for backend components, please see [this page](https://myrmex.github.io/overview/cadmus/dev/history/#2023-02-01---backend-infrastructure-upgrade). Anyway, in the end you just have to update your libraries and a single namespace reference. Benefits include:
  - more streamlined component instantiation.
  - more functionality in components factory, including DI.
  - dropped third party dependencies.
  - adopted standard MS technologies for DI.

### 5.0.0

- 2022-11-10: upgraded to NET 7.

### 4.2.3

- 2022-11-04: part/fragment seeders can return null.

### 4.2.1

- 2022-11-04: minor nullability adjustments.

### 4.2.0

- 2022-11-04: all the projects refactored for nullability check.

### 4.1.1

- 2022-11-03:
  - removed legacy ID check from thesaurus entry constructor.
  - updated packages.
- 2022-10-12: moved `Cadmus.Cli.Core` to Cadmus tool solution.

### 4.1.0

- 2022-10-10: **breaking change**: refactoring providers for repository and part seeder factory. As Cadmus core is now on .NET 6.0, we're going to remove the CLI-specific providers and let the CLI tool use the generic providers (from each project's PRJ.Services library) together with the API. The only change in the core for this is adding a `ConnectionString` property to `Cadmus.Core.IRepositoryProvider`. Correspondingly, the provider interfaces in `Cadmus.Cli.Core` have been marked as obsolete.

This change has these effects:

- **project API**: update your packages, and change the injected service in `Startup.cs` so that you specify the connection string:

```cs
// old code (Configuration was injected via ASPNET dependency injector):
// services.AddSingleton<IRepositoryProvider, MyProjectRepositoryProvider>();
// new code:
string dataCS = string.Format(
  Configuration.GetConnectionString("Default"),
  Configuration.GetValue<string>("DatabaseNames:Data"));
services.AddSingleton<IRepositoryProvider>(
  _ => new AppRepositoryProvider { ConnectionString = dataCS });
```

- **project models**: you can remove the CLI providers library, and ensure that you implement correctly the repository provider in the `Services` library (see below).

- **CLI tool**: the CLI tool will adapt to the new architecture so that it consumes providers from the shared library rather than from the CLI-specific one, i.e. in its `IRepositoryProvider` implementation:
  - add a `ConnectionString` property to `CreateRepository`.
  - add a `TagAttribute` to the class so that it can be retrieved from a plugin.

Sample (remove the injected `IConfiguration`, no more used):

```cs
/// <summary>
/// Cadmus TGR repository provider.
/// Tag: <c>repository-provider.tgr</c>.
/// </summary>
[Tag("repository-provider.tgr")]
public sealed class TgrRepositoryProvider : IRepositoryProvider
{
    /// <summary>
    /// The connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    // ... other code unchanged ...

    /// <summary>
    /// Creates a Cadmus repository.
    /// </summary>
    /// <param name="database">The optional database name. If not specified,
    /// this will be read from the configuration object used by this
    /// provider (<c>ConnectionStrings:Default</c> and <c>DatabaseNames:Data</c>).
    /// </param>
    /// <returns>repository</returns>
    /// <exception cref="InvalidOperationException">connection string not set
    /// </exception>
    public ICadmusRepository CreateRepository(string? database = null)
    {
        // create the repository (no need to use container here)
        MongoCadmusRepository repository = new(_partTypeProvider,
                new StandardItemSortKeyBuilder());

        repository.Configure(new MongoCadmusRepositoryOptions
        {
            ConnectionString = ConnectionString ??
                throw new InvalidOperationException(
                    "No connection string set for IRepositoryProvider implementation")
        });

        return repository;
    }
}
```

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
