# Cadmus

Cadmus data layer and business layer.

👀 [Cadmus Page](https://myrmex.github.io/overview/cadmus/)

## Projects

XUnit test projects are provided for each project by adding a `.Test` suffix to the project name.

**Core**:

- Cadmus.Core: core models and logic
- Cadmus.Seed: mock data seeding
- Cadmus.Mongo: MongoDB data access layer 

**Index**:

- Cadmus.Index: indexing logic
- Cadmus.Index.Sql: generic SQL-based data access layer for indexing
- Cadmus.Index.PgSql: PostgreSQL data access layer for indexing

**Graph**:

- Cadmus.Graph: semantic graph core
- Cadmus.Graph.Extras: additional graph functionalities
- Cadmus.Graph.Ef: Entity Framework integration for graph
- Cadmus.Graph.Ef.PgSql: PostgreSQL support for EF graph
- Cadmus.Graph.Api: graph API for testing purposes
