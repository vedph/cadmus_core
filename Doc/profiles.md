# Cadmus Profiles

A Cadmus profile is a JSON document used to define items facets, parts, flags, and taxonomies (thesauri).

When the backend API starts, the profile is used to seed a not-existing database. The location of this profile is read from the environment variable named `SEED__PROFILEPATH` (Linux; `SEED:PROFILEPATH` in Windows). If this is not specified, a default mock profile is used. This typically happens when launching the API just for the sake of experimenting with a mock database. This profile is named `seed-profile.json`, and is located under `wwwroot` in the `cadmus_api` repository.

## Facets

A facet is an abstraction representing the "type" of an item. In Cadmus there is no fixed "type" for an item, as this just results from the aggregation of its parts. Yet, for a better UI and validation purposes we provide the notion of _facet_.

A facet is a collection of part definitions, which list all the parts an item having that specific facet could contain. In a facet, some of the parts are defined as required, while other as optional. Also, there can be several parts of the same type with different roles: for instance, a datation part for the text and another one for a later copy of it.

Given the open nature of this data architecture, the notion of facet is essentially a frontend concept, designed with practical purposes. No facet-related constraint is enforced at the backend level, as by design an item could include any type of part.

A facet has a unique ID (an arbitrary string), a label with a human-readable name, a short description, and a set of part definitions.

Each part definition has these properties:

- `typeId`: the unique type ID assigned to the part.
- `roleId`: an optional role ID (an arbitrary string) assigned to the part. This is used when the part's type occurs more than once in the facet, with different roles (which systematically happens for layer parts).
- `name`: a human-friendly name assigned to the part.
- `description`: a short description for the part in the context of its facet.
- `required`: `true` if the part is required.
- `colorKey`: an RRGGBB color key optionally used in a frontend to visually mark the part.
- `groupKey`: a human-friendly short group name the part belongs to. This can be used in a frontend to present parts grouped in a logical manner.
- `sortKey`: the sort key for the part inside its group. This can be used in a frontend to sort the parts of each group when presenting them.
- `editorKey`: a string which can be used to group parts according to the frontend editors organization. Whereas `groupKey` is a purely presentational feature, `editorKey` is related to how the frontend organizes its editing components in different modules. For instance, the same frontend module might include two parts whose definitions have different `groupKey`'s, so that they get displayed in different groups, but their editors are found in the same component. This is a property used by frontend only, and has no usage beyond it.

As for `roleId`, the following reserved values have a special meaning:

- `base-text`: defines the part which represents the base text, when using text layers. This allows using different part types to represent the base text, according to the nature of the corpus being handled.

Also, the `editorKey` can be a composite value, where 2 different values are separated by a space. This happens for layer parts, when the `editorKey` value for the layer part is different from that of the fragment. In this case, the first value is the part editor key, and the second is the fragment editor key.

For instance, this is not the case of the comment fragment, as its editor key is equal to that of the layer part; but it is the case of the apparatus fragment, whose editor key (`philology`) is different from that of the layer part (`general`).

The following code is a facet sample:

```json
  "facets": [
    {
      "id": "facet-default",
      "label": "default",
      "description": "The default facet",
      "partDefinitions": [
        {
          "typeId": "net.fusisoft.categories",
          "name": "categories",
          "description": "Item's categories.",
          "required": true,
          "colorKey": "98F8F8",
          "groupKey": "general",
          "sortKey": "categories",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.historical-date",
          "name": "date",
          "description": "Historical date.",
          "required": false,
          "colorKey": "F898F8",
          "groupKey": "general",
          "sortKey": "date",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.keywords",
          "name": "keywords",
          "description": "Item's keywords.",
          "colorKey": "90C0F8",
          "groupKey": "general",
          "sortKey": "keywords",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.note",
          "name": "note",
          "description": "A free text note about the document.",
          "colorKey": "B0A0F8",
          "groupKey": "general",
          "sortKey": "note",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.token-text",
          "roleId": "base-text",
          "name": "text",
          "description": "Item's token-based text.",
          "colorKey": "31AB54",
          "groupKey": "text",
          "sortKey": "text",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.token-text-layer",
          "roleId": "fr.net.fusisoft.comment",
          "name": "comments",
          "description": "Comments on text.",
          "colorKey": "F8D040",
          "groupKey": "text",
          "sortKey": "text-comments",
          "editorKey": "general"
        },
        {
          "typeId": "net.fusisoft.token-text-layer",
          "roleId": "fr.net.fusisoft.apparatus",
          "name": "apparatus",
          "description": "Critical apparatus.",
          "colorKey": "D4E0A4",
          "groupKey": "text",
          "sortKey": "text-apparatus",
          "editorKey": "general philology"
        },
        {
          "typeId": "net.fusisoft.token-text-layer",
          "roleId": "fr.net.fusisoft.orthography",
          "name": "orthography",
          "description": "Standard orthography.",
          "colorKey": "E0D4A4",
          "groupKey": "text",
          "sortKey": "text-apparatus",
          "editorKey": "general philology"
        },
        {
          "typeId": "net.fusisoft.token-text-layer",
          "roleId": "fr.net.fusisoft.witnesses",
          "name": "witnesses",
          "description": "Witnesses list.",
          "colorKey": "A4E0D4",
          "groupKey": "text",
          "sortKey": "text-apparatus",
          "editorKey": "general philology"
        }
      ]
    }
  ]
```

## Flags

Flags are up to 32 bits assigned to any item with an arbitrarily defined meaning.

Their meaning is defined in the profile, under the `flags` property, having:

- `id`: the bit value;
- `label`: a user-friendly label;
- `description`: a short description;
- `colorKey`: an RRGGBB color key optionally used in a frontend to visually mark the flag.

Sample:

```json
  "flags": [
    {
      "id": 1,
      "label": "to revise",
      "description": "The item must be revised.",
      "colorKey": "F08080"
    }
  ]
```

## Thesauri

Often, a common requirement for data is having some shared terminology and taxonomies to be used for the whole content. For instance, think of a collection of inscriptions a typical requirement would be a set of categories, which are traditionally used to group them according to their type (e.g. funerary, votive, honorary, etc.). In fact, there are a number of such sets of tags, which vary according to the content being handled categories, languages, metres, etc.

In such cases, usually we also want our editing UI to provide these entries as a closed set of lookup values, so that users can pick them from a list, rather than typing them (which would be more difficult, and error-prone).

Cadmus provides a generic solution to these scenarios in the form of **thesauri**, each including any number of entries.

Each thesaurus has these properties in the profile:

- `id`: an arbitrary string used to identify the thesaurus. It must contain only letters `a`-`z` or `A`-`Z`, digits (`0`-`9`), underscores (`_`), dashes (`-`) and dots (`.`). The ID must be suffixed by its language code, preceded by `@`. The language code can be either ISO639-2 or ISO639-3, as needed. If no code is specified, Cadmus will still try to find another thesaurus with the same ID and any of these language codes (in that order): `eng`, `en`, or none at all (which anyway should not be the case). This is useful when you do not have a complete localization, so that when asking e.g. for the Italian version of a language you can fallback to the English one if the Italian language is not available.
- `entries`: an array of entries, each having an `id` and a `value` (in the language specified for its thesaurus). The `id` is an arbitrary string, with the same constraints illustrated above for the thesaurus ID.

A **thesaurus entry** (`ThesaurusEntry`) is thus a generic id/value pair. You can use dots to represent a hierarchical structure for the entries; for instance, a hierarchy like this:

```txt
language
  -phonology
  -morphology
  -syntax
```

can be represented using these IDs:

- `language`
- `language.phonology`
- `language.morphology`
- `language.syntax`

This allows modeling a full hierarchy without any depth limitation, while still handling a flat list of entries in a thesaurus set.

Here is a profile sample for thesauri, representing a set of language names, localized for both English and Italian:

```json
"thesauri": [
    {
      "id": "languages@en",
      "entries": [
        {
          "id": "eng",
          "value": "English"
        },
        {
          "id": "fre",
          "value": "French"
        },
        {
          "id": "deu",
          "value": "German"
        }
      ]
    },
    {
      "id": "languages@it",
      "entries": [
        {
          "id": "eng",
          "value": "inglese"
        },
        {
          "id": "fre",
          "value": "francese"
        },
        {
          "id": "deu",
          "value": "tedesco"
        }
      ]
    },
]
```

Note that currently the web UI frontend implements a convention by which if a thesaurus with id `model-types@en` exists, it will use to map part type IDs like `net.fusisoft.note` to user-friendly names like `note`. If such thesaurus does not exist, or is not complete, no error will occur; rather, the raw type IDs will be used instead of the corresponding user-friendly names.
