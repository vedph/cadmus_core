# Cadmus

**Note**: to run tests, you must start the MongoDB server.

Cadmus 3rd generation, refactored to drop MEF-based configuration. 2nd generation was in solution Cadmus, 1st in 46.

## MongoDB

It should be possible to run a mongo container storing data in host through a volume in `c:\users\YOURUSERNAME\data\mongo`, but in Windows this has issues:

    docker run --name mongo -d -p 27017:27017 --volume c:/users/dfusi/dockerVolMongo/db:/data/db mongo --noauth

See <https://github.com/docker/for-win/issues/138> about the directory location under the user folder (permissions requirements). Yet this does not seem to work even if using elevated command prompt.

Just run without persisting data:

    docker run --name mongo -d -p 27017:27017 mongo --noauth

## Architecture

The main data blocks in this data modeling system are simple: we only deal with *items* and *parts*.

The **item** is a sort of virtual record. It's not an atomic record in a storage system; it's just a *container*, and all the containers are equal, whatever their content. Any item has an essential set with a few, well-defined metadata, like ID, title, and the like, which are mainly used for browsing purposes.

As containers, items contain **parts**. A part is just a structured object, modeled in any way its designer wants it to be. "Real" data is stored in parts, which are grouped into items, whose counterpart in the real world varies, according to the data being handled. For instance, in a corpus of inscriptions each inscription would be an item; in a dictionary, each entry would be an item; in a museum catalog, each object would be an item; etc.

As for *storage*, only items and parts are treated as units. Any inner structure of each part has no independent existence, even if it may well (and often does) belong to a common model, reused across different types of parts.

In this context, items have an unlimited, open, set of types, as their "type" only results from the types of the parts they contain. All the containers (items) are equal, but their contents vary. As soon as you add a new part type to an existing item, this results in a different item "type". This is somewhat similar to what is known as "duck typing" in languages like Javascript.

Anyway, for editing and presentational purposes only, Cadmus has the notion of items *facets*. A facet defines which parts types can be included in each "type" of item. Note that this is useful for providing easier UI, and is not related to any intrinsic limitation in the system's structure. In fact, a facet also defines pure presentational features like colors, sorting or grouping criteria, etc.

In a facet, some parts can be required, others can be optional; it may even happen that there are several parts of the same type, used with different roles (e.g. in an epigraphical database two parts expressing datation, one for the original inscription, and another for its copy).

TODO

A special type of part, like the *text layer part*, is still a part like any other, but it represents a single collection of fragments. For instance, in a critical apparatus the critical apparatus is represented by a part, and all the entries in the apparatus related to the container item are that part's fragments. Thus, fragments have no independent existence (whence their name): only items and parts are stored as records.

### Parts

#### Parts - Layered Text

As for any text which should link any type of metadata to any of its portions, Cadmus provides the notion of "text layers".

A *text layer* is just a part like any other, but its model is limited to include a collection of "fragments". Each *fragment* contains the metadata to be linked to a specific portion of the text. A text layer part only contains a single type of fragments, and thus represents a "layer" overlaid to the "base txt".

For instance, you might want to add a short comment to a couple of words in a text: this would imply adding a comments layer on top of the base text, which always remains unchanged, and adding a fragment in it, containing this comment with the reference to the targeted text.

The type of text layer part varies according to the text location system used to reference a portion of the text, and to the type of its fragments. Note that for reflection purposes a text layer part *must* comply with these naming conventions:

- its class name must end with `LayerPart` (e.g. `CommentLayerPart`);
- its role ID must be equal to its fragment's type ID, which by convention always starts with the prefix `fr.`. This is already implemented by the layer part's constructor using reflection.

Thus, you can tell that a part is a layer part by looking at its class name (in C#) or at its role ID (when it starts with `fr.` it's a layer part whose fragment type ID is equal to the layer part role ID).

In general, the location system is based on the `ITextPoint` interface, which represents a single point in the text. The `ITextLocation<TPoint>` interface contains 1 or 2 of such points, whose concrete type is the generic parameter.

Usually, the location system adopted relies on lines and tokens. Here, the base text is divided in lines, and each line includes zero or more tokens, i.e. sequences of characters separated by whitespace, whatever their content. Each line and each token in a line are numbered starting from 1.

In this token-based system, a point is thus represented by 2 values: one for the line (`Y`) and another for the token (`X`). Further, as you can also select just a portion of a token (e.g. the `i` in `gaudis` for `gaudes`), the point can optionally include a character number (`At`) and count (`Run`). Thus, the location for the `i` in `gaudis` as the second token of line 3 would be expressed as `3.2@5x1` = line 3, token 2, character 5, run 1.

The token-based system can refer to a single token when using just a single point, or to two tokens when referring to a range of tokens.

In turn, the text itself is represented by another part, whose model may depend on the location system used. For instance, in the token-based system the `TokenTextPart` has a collection of `TextLine` objects, each including a line number (`Y`) and a `Text` value.

Thus, you just have to add a text part for the "base" text, and as many layer parts as you want to add metadata to any region in this text.

This approach has several advantages over a traditional markup like XML:

- the base text does not need to change whenever you add new metadata to it, as we're not marking it by inserting tags. You just write it once, and you can add new metadata at any point in time, thus freely expanding metadata without having to make any change to either the existing models or their data.

- these metadata are each modeled according to its own structure, which is freely defined by the layer designer. You just define the model of a layer fragment, and use it. For instance, a comment fragment model just has a `Text` property bearing the comment, and a `Tag` property which can optionally be used to group comments in different categories; but you can also have more complex models, as e.g. for datations (a portion of text referring to some dated information), with dozens of properties nested in their own structures to represent all the nouances of a historical datation. Whenever you need to add a new layer, just design a new part as you like it, and you are ready to go, without any change to the existing data and software.

- there is no limit in defining the text regions, while in markup technologies like XML you are constrained by the avoidance of elements overlaps. An XML dialect can overcome such issues by further complicating the markup (e.g. adding attributes or nested structures), but this is often tricky, and at any rate it fatally comes to an end where the XML schema would be overtly complex and virtually unusable.

More, text metadata are no different than any other datum stored in the system, as long as they are just represented by parts. Thus, there is really nothing special in them, and they can be treated as any other datum.

### Storage

Items and parts are _business_ POCO objects; as for their storage:

- items and history items are represented each by its own, closed schema, an object modeled according to the storage technology being used. For instance, in MongoDB we have `StoredItem` and `StoredHistoryItem`. This is because each technology has its own requirements. For instance, in Azure _DocumentDB_ all the resources must be derived from the base class `Resource`; further, as for any other NoSql database, the stored objects should not have reference properties, unless their value should be stored with the container. Thus, when dealing with such storage, an item has no property representing the list of its parts, nor the part has a reference property representing its container item. Instead, a part has its collection of fragments, which are stored in the part itself, as they have no existence of their own.

- facets are represented by a closed schema, an object modeled according to the storage technology being used. For instance, in MongoDB we have `StoredItemFacet` (implementing `IFacet`).

- parts can be directly stored as self-contained POCO objects. In MongoDB, each part has its own model, corresponding to the C# class implementing it. Parts classes come from plugins.

Currently, the "philology" parts include a bunch of parts also used in more specialized areas (e.g. epigraphical ligatures), but do not yet include specific epigraphical parts. These will be added in a separated plugin (and then, more epigraphical parts like ligatures will be moved into that plugin).

## Miscellaneous

As for the apparatus, maybe getting ideas from <https://quilljs.com/docs/delta/> could be useful.
