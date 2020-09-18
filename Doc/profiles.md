# Cadmus Profiles

A Cadmus profile is a JSON document used to define items facets, parts, flags, and taxonomies (thesauri).

When the backend API starts, the profile is used to seed a not-existing database. The location of this profile is read from the environment variable named `SEED__PROFILESOURCE` (Linux; `SEED:PROFILESOURCE` in Windows).

If this is not specified, a default mock profile is used. This typically happens when launching the API just for the sake of experimenting with a mock database. This profile is named `seed-profile.json`, and is located under `wwwroot` in the `cadmus_api` repository.

The profile source can either be a file name, or an HTTP(S) resource; in the latter case, it is assumed that it starts with `http`.

When the profile source is a file name, it may contain directory variables between `%`. Currently, the variable `%wwwroot%` is reserved to resolve to the web content root directory; any other variable name is searched in the configuration.

## Facets

A facet is an abstraction representing the "type" of an item. In Cadmus there is no fixed "type" for an item, as this just results from the aggregation of its parts. Yet, for a better UI and validation purposes we provide the notion of _facet_.

A facet is a collection of part definitions, which list all the parts an item having that specific facet could contain. In a facet, some of the parts are defined as required, while other as optional. Also, there can be several parts of the same type with different roles: for instance, a datation part for the text and another one for a later copy of it.

These restrictions apply to facets:

- a facet must include at least 1 part definition.
- when a facet contains text layers, it *must* contain 1 (and only 1) part definition representing the "base text". This is a logical requirement, as you must have some base text to link metatextual data to. This part definition must have the reserved role ID `base-text`.

Given the open nature of this data architecture, the notion of facet is essentially a frontend concept, designed with practical purposes. No facet-specific constraint is enforced at the backend level, as by design an item could include any type of part.

A facet has a unique ID (an arbitrary string), a label with a human-readable name, a short description, a color key, and a set of part definitions.

Each part definition has these properties:

- `typeId`: the unique type ID assigned to the part.
- `roleId`: an optional role ID (an arbitrary string) assigned to the part. This is used when the part's type occurs more than once in the facet, with different roles (which systematically happens for layer parts).
- `name`: a human-friendly name assigned to the part.
- `description`: a short description for the part in the context of its facet.
- `required`: `true` if the part is required.
- `colorKey`: an RRGGBB color key optionally used in a frontend to visually mark the part.
- `groupKey`: a human-friendly short group name the part belongs to. This can be used in a frontend to present parts grouped in a logical manner.
- `sortKey`: the sort key for the part inside its group. This can be used in a frontend to sort the parts of each group when presenting them.

As for `roleId`, the following reserved values have a special meaning:

- `base-text`: defines the part which represents the base text, when using text layers. This allows using different part types to represent the base text, according to the nature of the corpus being handled.

The following code is a facet sample:

```json
  "facets": [
    {
      "id": "default",
      "label": "default",
      "description": "The default facet",
      "colorKey": "86ACEB",
      "partDefinitions": [
        {
          "typeId": "it.vedph.categories",
          "name": "categories",
          "description": "Item's categories.",
          "required": true,
          "colorKey": "98F8F8",
          "groupKey": "general",
          "sortKey": "categories"
        },
        {
          "typeId": "it.vedph.historical-date",
          "name": "date",
          "description": "Historical date.",
          "required": false,
          "colorKey": "F898F8",
          "groupKey": "general",
          "sortKey": "date"
        },
        {
          "typeId": "it.vedph.keywords",
          "name": "keywords",
          "description": "Item's keywords.",
          "colorKey": "90C0F8",
          "groupKey": "general",
          "sortKey": "keywords"
        },
        {
          "typeId": "it.vedph.note",
          "name": "note",
          "description": "A free text note about the document.",
          "colorKey": "B0A0F8",
          "groupKey": "general",
          "sortKey": "note"
        },
        {
          "typeId": "it.vedph.token-text",
          "roleId": "base-text",
          "name": "text",
          "description": "Item's token-based text.",
          "colorKey": "31AB54",
          "groupKey": "text",
          "sortKey": "text"
        },
        {
          "typeId": "it.vedph.token-text-layer",
          "roleId": "fr.it.vedph.comment",
          "name": "comments",
          "description": "Comments on text.",
          "colorKey": "F8D040",
          "groupKey": "text",
          "sortKey": "text-comments"
        },
        {
          "typeId": "it.vedph.token-text-layer",
          "roleId": "fr.it.vedph.apparatus",
          "name": "apparatus",
          "description": "Critical apparatus.",
          "colorKey": "D4E0A4",
          "groupKey": "text",
          "sortKey": "text-apparatus"
        },
        {
          "typeId": "it.vedph.token-text-layer",
          "roleId": "fr.it.vedph.orthography",
          "name": "orthography",
          "description": "Standard orthography.",
          "colorKey": "E0D4A4",
          "groupKey": "text",
          "sortKey": "text-apparatus"
        },
        {
          "typeId": "it.vedph.token-text-layer",
          "roleId": "fr.it.vedph.witnesses",
          "name": "witnesses",
          "description": "Witnesses list.",
          "colorKey": "A4E0D4",
          "groupKey": "text",
          "sortKey": "text-apparatus"
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

Note that currently the web UI frontend implements a convention by which if a thesaurus with id `model-types@en` exists, it will be used to map raw part type IDs like `it.vedph.note` to user-friendly names like `note`. If such thesaurus does not exist, or is not complete, no error will occur; rather, the raw type IDs will be used instead of the corresponding user-friendly names.

### Thesauri Aliases

Additionally, thesauri allow for an aliasing mechanism. This may be useful when your frontend is aggregating different editors which might use different identifiers for the same thesaurus resource.

In this case, you can define a thesaurus having only its `id` property, equal to the alias, and a `targetId` property, equal to the identifier of the target thesaurus. No entries will be set for the alias.

Once this alias is set, Cadmus will return the target thesaurus when requested for the alias thesaurus.

For instance, say you want to retrieve the `languages@en` thesaurus using two different identifiers: `languages@en`, and the alias `ui-languages@en`. You could just add this thesaurus to the above sample:

```json
{
    "id": "ui-languages@en",
    "targetId": "languages@en"
}
```

This way, the languages thesaurus would be retrieved whatever ID you use: `languages@en` or the alias `ui-languages@en`, without having to duplicate resources or changing the frontend components.

### Thesaurus Scope in Parts

In the editor architecture, thesauri are loaded when the part or fragment editor loads.

Each editor provides its own list of thesauri IDs it requires. For instance, the keywords part editor requires a thesaurus listing all the languages used for them. The ID requested for this thesaurus is a string, usually without the language suffix, e.g. `languages`.

When the editor loads, it passes its requested IDs to the base editor class (here BEC for short), which in turn passes them to the service in charge of loading the edited data in the corresponding editor state.

This service loads its data together with the requested thesauri, by calling the backend via its exposed API. The backend forwards the request to the data layer, which retrieves the matching thesauri from the database.

The backend adopts a fallback mechanism for languages; if the specified language is not found, or no language is specified, it falls back to the default language (`eng` or `en`).

Also, as we have seen above it is possible to define alias IDs for thesauri, so that we can eventually target the same thesaurus, whatever the IDs requested by different editors. For instance, say there are two editors requesting languages thesaurus, developed independently. One of them requests the ID `languages`; another requests the ID `keyword-languages`; yet, we want to use the same list of language for both. Rather than duplicating it, we just provide an alias for one of the two IDs, pointing to the other one. So, the requests of each editor will be satisfied, without having to modify their code, or to produce redundancy in the thesauri set.

Until here, all the scenarios we have illustrated work with *statically* defined thesauri IDs; we can fallback with the language, or use aliases; but editors requests are totally defined at design time.

In some cases, it may happen that editors require to define their requested IDs *dynamically*, at runtime. For instance, this happens for the apparatus fragment editor, which requires 2 dynamically defined IDs: the witnesses thesaurus and the authors thesaurus are different for each work being handled.

The backend stores each of these thesauri with an ID which differs only for their "scope", i.e. the ID portion starting with the last dot: for instance, we might have these IDs for two different works:

- `apparatus-witnesses.verg-eclo@en`: apparatus witnesses for Vergilius *Eclogae*.
- `apparatus-witnesses.verg-aen@en`: apparatus witnesses for Vergilius *Aeneis*.

In the frontend, at design time the fragment editor can just request an ID like `apparatus-witnesses`; it cannot know which is the work the fragment belongs to. So, how can we get to the actual IDs listed above for Vergilius? To this end, the BEC leverages a generic mechanism based on the part's *thesaurus scope*. This is an optional property which defines the scope of the thesaurus IDs for those editors requiring runtime data.

If now the apparatus layer part referring to Vergilius *Eclogae* has its thesaurus scope defined, here as `verg-eclo`, this triggers a generic frontend mechanism which overrides the apparatus fragment editor requests. This just requests `apparatus-witnesses`; but once the mechanism finds out that its container part has a thesaurus ID scope, the ID is overridden as `apparatus-witnesses.verg-eclo`. Thus, the backend receives this ID request and satisfies it by retrieving the correct thesaurus.

Finally, some of the IDs requested by the editor might require not to be overridden by the scope. For instance, the fragment editor requests a third thesaurus, the `apparatus-tags`, a list shared among the apparatuses of all the works. As such, we must not override it as `apparatus-tags.verg-eclo`, which would not be found, because the database just has a single thesaurus with ID `apparatus-tags`. In this case, the fragment editor notifies the mechanism that it should not override this ID by prefixing it with an exclamation mark.

To sum up, the fragment editor at design time requests 3 IDs:

- `!apparatus-tags`: apparatus tags. Note the leading `!` which avoids the override.
- `apparatus-witnesses`: apparatus witnesses.
- `apparatus-authors`: apparatus authors.

This is all what the editor needs to know. It is up to the generic mechanism from which the editor is derived to lookup the corresponding part, and apply overrides when it finds that it has a thesaurus scope.

## Browsers

Optionally, a profile can include a `browsers` section to define and configure specialized [items browsers](core.md#a.3.4.-items-browsers).

This includes an array of objects, each with an `id` and an eventual `options` object.

For instance, here is the configuration for the hierarchy items browser:

```json
"browsers": [
  {
    "id": "it.vedph.item-browser.mongo.hierarchy"
  }
]
```

A corresponding thesaurus in the [profile](profiles.md), with ID `item-browsers@en`, is used to provide a human-friendly list of browsers in a UI:

```json
{
  "id": "item-browsers@en",
  "entries": [
    {
      "id": "it.vedph.item-browser.mongo.hierarchy",
      "value": "items hierarchy"
    }
  ]
}
```

Note that the thesaurus entry is required to configure the browser object instantiated by the factory, while the thesaurus entry is just a presentational feature, used to provide a list of browsers to a UI.

For instance, when this thesaurus is found, the standard Cadmus web app provides an `Items` menu with a list of browsers derived from it, plus the link to the default list-based browser. When it is not found, the app just provides a single `Items` link which provides the default list-based browser.
