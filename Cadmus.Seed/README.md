# Cadmus.Seed

This project provides the infrastructure for seeding a Cadmus database with mock data for testing and development. It contains interfaces, base classes, and orchestration logic. Concrete part and fragment seeders are implemented in separate projects.

## Architecture Overview

```txt
┌─────────────────────────────────────────────────────────────────┐
│                        CadmusSeeder                             │
│  Orchestrates seeding: creates items with parts and fragments   │
└─────────────────────────────────┬───────────────────────────────┘
                                  │
                                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                     PartSeederFactory                           │
│  Instantiates and configures seeders from JSON configuration    │
└──────────┬─────────────────────────────────────┬────────────────┘
           │                                     │
           ▼                                     ▼
┌─────────────────────┐               ┌─────────────────────┐
│    ItemSeeder       │               │   IPartSeeder[]     │
│  Creates items      │               │  Create parts       │
└─────────────────────┘               └──────────┬──────────┘
                                                 │
                                                 ▼
                                      ┌─────────────────────┐
                                      │  IFragmentSeeder[]  │
                                      │  Create fragments   │
                                      │  (for layer parts)  │
                                      └─────────────────────┘
```

## Core Concepts

### Items, Parts, and Fragments

Cadmus uses a **compositional data model**:

- **Item**: A container (like a box) representing a data record. Items have metadata (ID, title, description, facet ID) and contain parts.
- **Part**: A discrete data model contained in an item. Parts are POCOs with a `TypeId` identifying their model and an optional `RoleId` for disambiguation.
- **Fragment**: A sub-component of a **layer part**. Fragments represent annotations linked to specific text spans in a base text part.

### Type IDs and Role IDs

Every part has a **type ID** (e.g., `it.vedph.categories`) that identifies its data model. The corresponding seeder has ID `seed.{typeId}` (e.g., `seed.it.vedph.categories`).

**Role IDs** disambiguate multiple parts of the same type:

- **Same item, same type, different purposes**: Two `categories` parts in one item: one for inscription function (`roleId: "function"`), one for material (`roleId: "material"`).
- **Different thesauri for same part type**: Categories part draws from different taxonomies based on role.

### Layer Parts and Fragment Roles

**Layer parts** are special parts that contain an array of fragments, representing annotations on a base text. They follow specific conventions:

1. **Layer part role ID = fragment type ID**: A layer part's `RoleId` identifies the type of fragments it contains, prefixed with `fr.` (e.g., `fr.it.vedph.comment`).

2. **Fragment roles**: Fragments can have additional roles appended after a colon:
   - Without fragment role: `fr.it.vedph.comment`
   - With fragment role: `fr.it.vedph.comment:scholarly`

3. **The `fr.` prefix**: All fragment type IDs start with `fr.` to distinguish them from regular part type IDs. This prefix is defined in `PartBase.FR_PREFIX`.

4. **Base text role**: The special role `base-text` (defined in `PartBase.BASE_TEXT_ROLE_ID`) identifies the part containing the base text that layer parts annotate.

**Example structure**:

```txt
Item
├── TokenTextPart (roleId: "base-text")     ← The annotated text
├── LayerPart (roleId: "fr.it.vedph.comment")
│   └── CommentFragment[]                    ← Comment annotations
├── LayerPart (roleId: "fr.it.vedph.comment:scholarly")
│   └── CommentFragment[]                    ← Scholarly comment annotations
└── LayerPart (roleId: "fr.it.vedph.apparatus")
    └── ApparatusFragment[]                  ← Critical apparatus entries
```

## Component Reference

### CadmusSeeder

The top-level orchestrator. Its `GetItems(int count)` method:

1. Initializes the random seed (if configured)
2. For each item:
   - Picks a random facet from configuration
   - Creates an item via `ItemSeeder`
   - Adds non-layer parts (required first, then optional)
   - Adds layer parts (only if base text exists)
3. Optionally rebuilds item sort keys

**Part inclusion logic based on `IsRequired`:**

- **Non-layer, required**: Always seeded.
- **Non-layer, optional**: 50% chance to include.
- **Layer, required**: Always seeded (if base text exists).
- **Layer, optional**: 50% chance to include.

Layer parts require a base text part to exist. If required layer parts are defined, the seeder ensures the base text is created first.

### PartSeederFactory

Factory that instantiates seeders from configuration:

- `GetSeedOptions()`: Returns `SeedOptions` from `Seed:Options`
- `GetPartSeeders()`: Returns dictionary of part seeders keyed by type ID (with optional role suffix)
- `GetFragmentSeeder(typeId)`: Returns fragment seeder with role-aware fallback
- `GetItemSeeder()`: Returns the item seeder
- `GetItemSortKeyBuilder()`: Returns optional custom sort key builder

### IPartSeeder / PartSeederBase

Interface and base class for part seeders:

```csharp
public interface IPartSeeder
{
    void SetSeedOptions(SeedOptions options);
    IPart? GetPart(IItem item, string? roleId, PartSeederFactory? factory);
}
```

`PartSeederBase` provides:

- `SetPartMetadata(part, roleId, item)`: Sets item ID, creator, user, and role on the part
- Role assignment logic: may randomly assign roles from `PartRoles`/`FragmentRoles` (1 in 10 chance)

### IFragmentSeeder / FragmentSeederBase

Interface and base class for fragment seeders:

```csharp
public interface IFragmentSeeder
{
    void SetSeedOptions(SeedOptions options);
    Type GetFragmentType();
    ITextLayerFragment? GetFragment(IItem item, string location, string baseText);
}
```

### SeederTypeRoleId

Helper struct for parsing composite `typeId:roleId` identifiers:

```csharp
// Parse "seed.it.vedph.categories:function" → TypeId="it.vedph.categories", RoleId="function"
var parsed = SeederTypeRoleId.Parse("seed.it.vedph.categories:function");

// Build key: "it.vedph.categories:function"
string key = SeederTypeRoleId.BuildKey("it.vedph.categories", "function");

// Get roles for a type from PartRoles list
IList<string> roles = SeederTypeRoleId.GetRolesForType(options.PartRoles, "it.vedph.categories");

// Extract fragment role from layer part roleId
string? fragRole = SeederTypeRoleId.ExtractFragmentRole("fr.it.vedph.comment:scholarly"); // "scholarly"
```

### SeedOptions

Configuration options:

- `Seed`: Optional random seed for reproducible results.
- `Users`: User names to randomly assign to seeded data.
- `PartRoles`: Composite IDs (`seed.{typeId}:{roleId}`) for part role assignment.
- `FragmentRoles`: Composite IDs (`seed.{fragmentTypeId}:{roleId}`) for fragment role assignment.
- `FacetDefinitions`: Loaded from `Facets` section; defines item models.

### SeedHelper

Utility methods for random selection:

- `RandomPickOf<T>(entries, count)`: Pick multiple distinct entries
- `RandomPickOneOf<T>(entries)`: Pick a single entry

## Role-Aware Seeding

The seeder supports **role-aware configuration**, allowing different options for the same part/fragment type with different roles.

### How It Works

1. **Seeder lookup**: When creating a part with `typeId` and `roleId`:
   - First tries key `{typeId}:{roleId}`
   - Falls back to `{typeId}` if not found

2. **Random role assignment**: If a part has no role but `PartRoles` contains entries for its type, a role may be randomly assigned (1 in 10 chance). Same for fragments with `FragmentRoles`.

3. **Backward compatible**: Configurations without roles work unchanged.

## Configuration Guide

### Minimal Configuration

```json
{
  "facets": [
    {
      "id": "default",
      "label": "Default",
      "description": "Default facet",
      "partDefinitions": [
        { "typeId": "it.vedph.note", "name": "note", "isRequired": true }
      ]
    }
  ],
  "seed": {
    "options": {
      "users": ["zeus", "hera"]
    },
    "partSeeders": [
      { "id": "seed.it.vedph.note" }
    ]
  }
}
```

### With Part Roles

```json
{
  "facets": [
    {
      "id": "inscription",
      "label": "Inscription",
      "partDefinitions": [
        { "typeId": "it.vedph.categories", "roleId": "function", "name": "function" },
        { "typeId": "it.vedph.categories", "roleId": "material", "name": "material" }
      ]
    }
  ],
  "seed": {
    "options": {
      "users": ["zeus"],
      "partRoles": [
        "seed.it.vedph.categories:function",
        "seed.it.vedph.categories:material"
      ]
    },
    "partSeeders": [
      {
        "id": "seed.it.vedph.categories:function",
        "options": {
          "maxCategoriesPerItem": 3,
          "categories": ["funerary", "votive", "honorary"]
        }
      },
      {
        "id": "seed.it.vedph.categories:material",
        "options": {
          "maxCategoriesPerItem": 2,
          "categories": ["limestone", "marble"]
        }
      }
    ]
  }
}
```

### With Layer Parts and Fragment Roles

```json
{
  "facets": [
    {
      "id": "text",
      "label": "Text",
      "partDefinitions": [
        { "typeId": "it.vedph.token-text", "roleId": "base-text", "name": "text", "isRequired": true },
        { "typeId": "it.vedph.token-text-layer", "roleId": "fr.it.vedph.comment", "name": "comments" },
        { "typeId": "it.vedph.token-text-layer", "roleId": "fr.it.vedph.comment:scholarly", "name": "scholarly comments" },
        { "typeId": "it.vedph.token-text-layer", "roleId": "fr.it.vedph.apparatus", "name": "apparatus" }
      ]
    }
  ],
  "seed": {
    "options": {
      "users": ["editor"],
      "fragmentRoles": [
        "seed.fr.it.vedph.comment:scholarly",
        "seed.fr.it.vedph.comment:general"
      ]
    },
    "partSeeders": [
      { "id": "seed.it.vedph.token-text" },
      { "id": "seed.it.vedph.token-text-layer" }
    ],
    "fragmentSeeders": [
      {
        "id": "seed.fr.it.vedph.comment",
        "options": { "languages": ["eng", "ita"] }
      },
      {
        "id": "seed.fr.it.vedph.comment:scholarly",
        "options": { "languages": ["lat", "grc"], "categories": ["philology"] }
      },
      {
        "id": "seed.fr.it.vedph.apparatus",
        "options": { "authors": ["Cicero", "Virgil"] }
      }
    ]
  }
}
```

### Configuration Reference

#### Seed Options (`seed.options`)

```json
{
  "seed": {
    "options": {
      "seed": 42,
      "users": ["user1", "user2"],
      "partRoles": ["seed.{typeId}:{roleId}", "..."],
      "fragmentRoles": ["seed.{fragTypeId}:{roleId}", "..."]
    }
  }
}
```

#### Part Seeder Entry (`seed.partSeeders[]`)

```json
{
  "id": "seed.{typeId}",
  "options": { /* seeder-specific options */ }
}
```

With role:

```json
{
  "id": "seed.{typeId}:{roleId}",
  "options": { /* role-specific options */ }
}
```

#### Fragment Seeder Entry (`seed.fragmentSeeders[]`)

```json
{
  "id": "seed.fr.{typeId}",
  "options": { /* seeder-specific options */ }
}
```

With role:

```json
{
  "id": "seed.fr.{typeId}:{roleId}",
  "options": { /* role-specific options */ }
}
```

## Implementing Custom Seeders

### Part Seeder

```csharp
[Tag("seed.it.myproject.mypart")]
public sealed class MyPartSeeder : PartSeederBase,
    IConfigurable<MyPartSeederOptions>
{
    private MyPartSeederOptions? _options;

    public void Configure(MyPartSeederOptions options) => _options = options;

    public override IPart? GetPart(IItem item, string? roleId,
        PartSeederFactory? factory)
    {
        ArgumentNullException.ThrowIfNull(item);

        MyPart part = new();
        SetPartMetadata(part, roleId, item);

        // Seed part data using _options and Bogus Faker
        part.Value = new Faker().Lorem.Sentence();

        return part;
    }
}

public sealed class MyPartSeederOptions
{
    public int MaxItems { get; set; }
}
```

### Fragment Seeder

```csharp
[Tag("seed.fr.it.myproject.myfragment")]
public sealed class MyFragmentSeeder : FragmentSeederBase,
    IConfigurable<MyFragmentSeederOptions>
{
    private MyFragmentSeederOptions? _options;

    public void Configure(MyFragmentSeederOptions options) => _options = options;

    public override Type GetFragmentType() => typeof(MyFragment);

    public override ITextLayerFragment? GetFragment(
        IItem item, string location, string baseText)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(location);

        return new MyFragment
        {
            Location = location,
            Text = new Faker().Lorem.Sentence()
        };
    }
}
```

## Usage

```csharp
// Build configuration
IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        PartSeederFactory.ConfigureServices(services,
            new StandardPartTypeProvider(),
            typeof(MyPartSeeder).Assembly);
    })
    .Build();

// Create factory and seeder
var factory = new PartSeederFactory(host);
var seeder = new CadmusSeeder(factory);

// Generate items
foreach (IItem item in seeder.GetItems(100))
{
    // Process or save item
}
```
