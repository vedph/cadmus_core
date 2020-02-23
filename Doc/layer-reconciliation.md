# Layers Reconciliation

## Linking Text and Metatextual Data

In any system where you are adding metadata to a base text while keeping both separated, from TEI standoff solutions to Cadmus, an obvious practical issue is reconciling the resource depending on the other one, once you the other has changed.

For instance, think of a standoff apparatus linked to a separated TEI text, where each portion to be linked to the apparatus has its own ID (an `xml:id` attribute). If we attach an apparatus entry to the portion identified by ID `p2.34`, and then for some reason we edit the text document and remove that portion of text, this will break the connection with the apparatus entry, leaving it orphaned.

Now, similar situations do rarely occur, as usually the practice is first establishing the text with its full markup in the text document, and then add apparatus entries in a standoff, dependent document. Once the text has been established, it is rarely the case that we change it; and when this happens, it is easy to manually reconcile the dependent apparatus entry (or whatever other data attached to the text).

This scenario derives from the typical practice and workflow: scholars take a text, start adjusting and marking it up, and finally add standoff notations, if any. So, having a text which is very unlikely to change once established and marked up is a fact descending from this practice, rather than an artificially imposed limitation. Of course, this is also logical: we first have a text, and then add non-trivial metadata to it (such as the ones usually found in standoff).

It should also be added that standoff is a complex thing to be implemented manually, and that usually we recur to it when we have a big quantity of data belonging to a structure which is different from that represented by the main XML's document tree. This happens for data like apparatus, which cover the whole text as far as it's related to its constitution, or for data systematically collected by automated procedures (e.g. for syntactical, morphological or metrical tagging). In the former case, it's obvious that these processes do work on an already established text, and add new metadata to it.

## The Cadmus Approach

The same happens for Cadmus: the usual, logic workflow in dealing with a text is first establishing it, and then adding specialized metadata to it.

In Cadmus the text is a *part*, and each type of metadata is contained in another part, representing a text layer. For instance, we have the text of an inscription, as it appears on the stone, in the text part (e.g. `bixit` instead of `vixit`); and the orthographic normalizations (`vixit`) in a different part, linked to each specific portion of the text part. As in the standard text location scheme we rely on line and token numbers, if we later move or delete the word `bixit` from the base text, this breaks the connection between it and its normalized form (`vixit`).

When moving a word (but not when deleting it), this does not happen in XML, as far as the word is inside an element which can carry an ID to identify it; but this implies adding an element to each specific portion of the base text, even when there would be no other reason than that of having an ID for it. This is perfectly fine; but one of its consequences is that whenever we want to add a new layer of metadata to the same text, we must change its markup, so that we can have our desired span of text embraced by some tag; and this is not always possible, because several structures we may want to overlay on the same text often have a totally different and incompatible nature.

For instance, it is easy to understand of the spans of the same epigraphical text would differ when we consider it from the standpoint of its layout on the stone (lines, dimensions, alignments, etc.), or from that of its syntactic structure, or from that of its metrical structure, or from that of its paleographic description, historical comments, prosopographic annotations, etc.

The more these structures, the more complex the resulting XML code, up to a point where we hit the overlap barrier. So, this is not ideal when scalability is a primary concern, and we want an open data architecture, where we can add whatever metadata with any model, without (a) having to think it in terms of elements and attributes which must fit with all the other structures which must coexist in the same document tree; and (b) having to touch the text whenever we are adding a new layer of metadata to it.

The latter issue is not only practical, as it implies designing an XML schema which must be *adapted* to semantically different data, where in an abstract consideration they would belong to different domains, and have different models.

The former issue is really about freeing data modeling from the cage of text which should embed it as markup, thus making a clearer separation between the physical model, implemented by a specific serialization data format, and the logical model, which should be freely designed by scholars *iuxta propria principia*.

In Cadmus, the approach to such issues is **reversing the hierarchy**: rather than having a dominant, unique structure represented by the text, where any other data is embedded as markup, we start from the data themselves, each considered in isolation, on its own, as an independent model: one for representing orthography and linguistic phenomena underlying it; one for paleographic data; one for metrical analysis; one for syntactical analysis; etc.

In this view, the text is no more the bearing structure any other data is attached to; rather, it is a part of our data, just like any other part, like metrics or paleography.

Further on this path, there can be place for any part of data which not only is non-textual, but it is not even directly related to a text: for instance, think of the archaelogical description of the context where the inscription has been found; or of a monographic collection of data related to ancient religious associations. The latter is a work on its own, which would typically fit in a traditional database; yet, one of its primary sources are inscriptions, so that scholars wanting to produce a similar study would probably have to build up their own epigraphic corpus as a part of that study.

In the context of Cadmus, we are free to introduce both textual and non-textual data, whatever their nature and model, thus connecting interdisciplinary data in the same content production environment. This being based on a true database and exposed via web, rather than represented by a continuous flow of characters in a file system, multiple users will be able to concurrently edit data: we might have scholars specialized in epigraphy building up the inscriptions parts, scholars specialized in history and religion taking care of the associations, linguists adding specialized data about the peculiar orthography of many of these texts, etc.; their works and competences converge into a multidisciplinary product, which remains open to new additions, that never affect the existing data or (even worse) their models.

## Editing Experience

In this context, the Cadmus editing experience is totally different. Rather than opening a text file in our machine and marking it up manually, we login into a website, and browse a set of records: some of them represent text, others represent metatextual data, others even non-textual data at all. At any rate, we pick the record we want, and edit it in a graphical user interface.

In the case of text, the text is edited in its own interface, and modeled and stored in its own part. The metadata related to -say- orthography and linguistics are edited in a different part, having any number of fragments, each linked to a portion of the text they refer to.

Thus, in the above example we would just type our text with `bixit`, and then add an orthography layer part. There, we would specify that the portion of the base text part corresponding to that word is to be normalized in `vixit`, and the underlying linguistic phenomenon is the spirantization of the voiced bilabial plosive before vowel, with all the details we may want to attach to its description.

We thus end up with two parts, text and orthography, belonging to the same logical unit: our inscription. In Cadmus terms, text and orthography are *parts* of the same *item* (the inscription).

Once in this *item and parts* architecture, we are totally free to add whatever part we want to the same item: apparatus, comments, paleographic description, physical descriptions, prosopographical data, geographical data, etc. We edit data of different type, each with its own model, *independently* designed to fit the view and purpose of the scholars, and with its own user interface. Some of these models happen to represent text; some other models happen to represent metatextual data; others just have nothing to do with text. Yet, all are parts of the same data universe, and get edited in the context of the same system.

## Reconciliating Layers via Diffing

In this totally modular scenario, we strive to keep each component independent, freely pluggable and concurrently editable. As for text, we can have different models representing it (currently Cadmus has two); as for layers, we can have different ways of linking their fragments to the corresponding portion of text. Currently Cadmus has just one, which is the most viable compromise between usability and readability, and can be applied to both the text models designed so far.

This linking method is based on the fact that whatever the details of the two text models, both can ultimately be projected on a bidimensional layout, with an Y-coordinate and an X-coordinate. For instance, a simple plain text has several lines, each containing several graphical words (=sequences of characters separated by space). We can just assign an Y value to each line, and an X value to each "word". These coordinates can target a single word, or any range of words (with a couple of coordinates for the range start and end).

In some cases, we are required to go further down the text hierarchy, up to the single character; to this end, we can add two other coordinates to `Y` and `X`, named "at" (`A`) and "run" (`R`): `A` is the ordinal number of the character in the word; `R` is its extent measured in characters. Thus, the `b` in `bixit` in this text:

```txt
d m
Decentius
qui bixit
...
```

can be addressed as `3.2` (=line 3, word 2) `@1x1` (=at character 1, running for 1 single character).

In the frontend editor, users just have their base text displayed, select any portion of it, and click a button to edit a specific metatextual data model linked to that selection. Editing will happen in a different page, with its own user interface and logic. Each part has its own, independent model; and its own, independent editing UI.

Yet, we are in a multiuser, concurrent editing environment. We have already seen that it is very rare that a text is changed once it has been entered; yet, this may happen, usually just to fix an error in the text.

So, for instance it is possible that while user B is editing an orthographic layer for the word `bixit`, user A deletes it in the base text. Granted, this should be a corner case, in a well-established flow and teamwork; but it might happen, and it would leave an orphaned fragment in the orthographic layer. User A might not even be aware of having broken a link, as he's working independently, on a separate part.

By the way, we should add that the system provides full editing history and auditing, so similar concurrency issues are somewhat mitigated.

Further, this is a situation where the orphaned fragment in the layer would be harmless: we could simply drop it in a validation pass, just like we can automate the removal of all the broken links in a set of static HTML pages. In fact, the fragment annotated a word which no more exists; so that's probably what we would want to do anyway. Yet, it would be useful for user B to know about this, should he ever want to move elsewhere some information he put in the fragment's model.

Also, other edits might affect the layers in a more critical way: for instance, if we move a word around, most of the word-based coordinates in the affected area would probably require to be rebased. This is not a process we would want to go unsupervised, as the metatextual data models might be complex, and so their reference to a specific portion of the base text. Thus, we cannot predict which types of adjustments users might want to do.

Rather, we can provide some generic mechanism to allow users reconcile layers with text in those rare occasions where a "layered" text gets changed. This needs to work *a posteriori*, in any moment after any editing has been done, by any of the users.

Validations will systematically happen on request, on the whole database; but we can also provide users with a view showing the consequences of their text editing in specific parts, so they can immediately take a look, and fix eventual issues as soon as they have finished with a potentially disruptive change.

In this context, we need a mechanism which just relies on different states of the text in time: at any moment, either as soon as we change a text, or months later, we want to be able to have a look at the changes made in the text, and how these could have affected its layers.

There are two types of aids we can provide to users: a general notification about all the layers which should be checked; and more specific, and sometimes automatic notifications, targeting well-defined fragments in them.

### Notifying Layers as a Whole

Cadmus provides a full editing history for any part and item. Thus, whenever a base text part is changed, we can report as potentially affected all the layer parts depending on it, but only if:

- they were saved *before* that change, or
- they were saved a reasonable amount of time after it.

The latter condition allows to take into account some unlucky concurrent editing scenarios, e.g.:

1. user A loads a layer part and starts editing it;
2. in the meantime, user B saves the corresponding base text he was editing, or he has started editing while A was working;
3. a few seconds after, user A saves his changes.

Now, user A was not aware of user B changing the text while working on the layer part, which was loaded *before* user B changes, and saved *just after* them. Thus, user A has worked with reference to a stale copy of the base text. Anyway, he happened to save his work a few seconds *after* B saved, rather than *before*. So, if we don't take into account also a reasonable amount of time *after* B saves, we might miss to notify about the requirement of a potential revision.

Apart from this time interval, there are two **additional factors** which are taken into account when calculating the potential break of a text layer:

1. *what has changed in the text part*: the system does not simply refer to the text part's last modified date and time; rather, it goes back in its editing history, to find the first version where the text changed. If you have just changed the text and saved it, this does not make any difference. If instead you have just modified e.g. the text's citation, but not the text itself, this makes the difference: the text has not changed even if the part was saved. In this case, the system will go back in the part editing history, until it finds the last change in the *text*. Once it finds it, it uses that date and time to compare to the layer's part modification time. Thus, if you edit a layer part, and then a text's part without changing its text, there will be no potential break detection, as the system knows that the text part though saved later had no change in its text.

2. *whether the text part is in its original state*: if the text part has no editing history except for its initial version, then no potential break is detected, even if the text part was saved a few milliseconds before the layer part. This is because this scenario is typical of data seeding or importing, where all the parts are imported one after another at about the same time, and we don't want newly imported layers to be marked as potentially broken just because of this, when no editing has yet occurred.

### Notifying Specific Fragments via Diffing

In addition to detecting a potential break in layers, we also want to provide users with a few hints and some automatic patching, where possible.

For instance, if you move the word `bixit` around, and an orthographic layer fragment was linked to it, we'd like to offer the user the ability of automatically relocating that fragment to the new location of that word. Or, if you have deleted that word, we'd like to offer the user the ability of automatically delete the dependent, now orphaned fragment from the orthographic layer.

In other cases the scenario is too complex for automating reconciliation, because fragment models are open, and on a general ground we can never know the complex reasoning underlying their data and location with reference to the base text. In these cases, we can at most direct users attention to those fragments which are the most likely to have been affected by text editing, in the area touched by their location.

As Cadmus implements editing history, we always have the previous state of the text at hand. Ultimately, whatever the model of the text, it can be represented as a string (which is expressed by the fact that in the backend all the base text parts implement the `IHasText` interface).

Inside that text, users might have performed any sort of editing operations, in any order and time; we cannot know anything in advance, about which of them affected what. So, the logical approach is **diffing**, as for any editing system: we compare two texts, and spot their differences in terms of the two basic operations capable of representing any change between them, i.e. deletions and insertions.

To this end, we're adopting the [Google's diff-match-patch library](https://github.com/google/diff-match-patch), both because its power and its multiple-languages implementations. With it, if we take a text like this:

```txt
alpha beta
gamma
delta epsilon waw
eta
```

and move `delta` after `epsilon`:

```txt
alpha beta
gamma
epsilon delta waw
eta
```

we get a differences list which represents the target text in terms of its relationship with the source text: what has been *removed*, what has been *added*, and what remained *equal*, in the order in which characters appear in the text:

```txt
(1)
Diff(EQUAL,"alpha beta↩¶gamma↩¶")
(2)
Diff(DELETE,"delta ")
(3)
Diff(EQUAL,"epsilon ")
(4)
Diff(INSERT,"delta ")
(5)
Diff(EQUAL,"waw↩¶eta")
```

As you can see, we have a number of diff operations, each with an operator and a text portion (for the reader's commodity, CR and LF characters are represented by symbols):

1. `alpha beta gamma` in the first two lines were not changed.
2. `delta` was deleted.
3. `epsilon` was not touched.
4. `delta` was inserted.
5. the final lines with `waw eta` were not changed.

This is what a standard diffing tool tells us about the changes in the text. In Cadmus terms, we are more interested in some specific data connected to these changes, i.e.:

- the detection of *movements*. If word `delta` carried a number of metadata, we want them to be moved with it, rather than being wrongly assigned to `epsilon`, which now has `delta`'s original coordinates.

- the detection of *replacements*. For similar reasons, a replacement might or not involve changes in the layers; if I just replaced `Bixit` (capitalized by error) with `bixit`, probably no layer would be affected. If instead I replaced it with `vixit`, the orthography layer might be affected, as far as it was referring a form with `b` which no more exists. In both cases, the word-based coordinates of this and other layers are not affected by this change, as it is still -say- the second word of the third line, whatever its content.

- the *coordinates* of each affected text. As layers are connected via text-based coordinates, we must know the coordinates of each piece of diffed text, so that we can refer it to the layers.

To this end, the diff report is *adapted* to a new list of operations, which add the higher-level operations of replacement and movement, plus the Y/X coordinates for each word. In the case of our sample, the result is:

```txt
(1)
1.1 equ alpha
(2)
1.2 equ beta
(3)
2.1 equ gamma
(4)
3.1 mvd delta (1)
(5)
3.1 equ epsilon
(6)
3.2 mvi delta (1)
(7)
3.3 equ waw
(8)
4.1 equ eta
```

Here, we have split the diffing at the atomic level of words: for instance, a single diff operation with text `alpha beta gamma` is now 3 distinct operations of the same type, one for each word, and each with its own coordinates.

Also, a move operation has been detected from two distinct diff operations: a deletion and an insertion. We want these distinct operations to remain the same, as we want to know what has been deleted and inserted, and where. Yet, we also want to know that these two operations are related, as part of a higher-order move operation. As a result, we have:

- an `mvd` (move-delete) operation, representing the deletion part of the movement operation.
- an `mvi` (move-insert) operation, representing the insertion part of the movement operation.

Both these parts are grouped under the same numeric ID `1`, which identifies them as parts of a higher-order operation.

Armed with this knowledge, we can say that:

- data linked to the words `alpha beta gamma` from `1.1` to `2.1` are probably unaffected.
- data linked to the word `delta` at `3.1` should probably be moved to the same word now at `3.2`.
- data linked to the words `epsilon` and `waw eta` are probably unaffected.

In other terms:

- every layer fragment targeting *point* (not *range*) `3.1`, or any of its sub-portions (e.g. just a character inside that word), should probably be moved to `3.2`. We could offer automatic reconciliation in this case.

- every layer fragment targeting a *range* including `3.1` (the deletion point) or `3.2` (the insertion point), e.g. a fragment targeting from `2.1` to `3.1`, or another from `3.2` to `4.1`, should be revised, to ensure that its data are still meaningful in the context of the new text.

Notice that the fragment models might be covering any sort of data, related to any portion of text, eventually in a specific context, which could determined by factors outside the text range covered by the fragment. So human intervention is required anyway; but at least we can provide users with hints, and automatic reconciliations (patches) where possible.

This is just a helper tool for scenarios which anyway represent corner cases, which rarely happen, and pose similar problems in any environment where a resource depends on another one and the latter changes. At least such mechanism provide a way of addressing users to those cases which might require their intervention.

As we have seen, this mechanism combines the diffing results with their old and new token-based locations. This does not rule out the possibility to use it for base text types other than token-based: in fact, it can be used with other types too, as far as they can use the same coordinates or a subset of them. This is the case of the tiled text, which has the same Y/X coordinates system, without the *at* and *run* components, which anyway are not used by this reconciliation system. There, each tile can be represented as a token (X), and each row of tiles as a line (Y).

Note: a demo for diffing and adapting results to the Cadmus layer system is available in the [DMP Playground throwaway app](https://github.com/vedph/cadmus_dmp-playground).

#### Hints and Patches Generation

Once this mechanism is in place, we could provide hints or automated patches for the following operations:

- `equ` (equal): text not changed, but it might have been relocated to new coordinates.
  - fragments whose extent is equal to or less than the operation coordinates: for the `equ` operations where the old location is different from the new one: offer *patch* (relocate fragments).
  - all the other fragments touching the operation coordinates: for the `equ` operations where the old location is different from the new one: mark them as *affected* (text moved inside).

- `del` (delete):
  - fragments whose extent is equal to or less than the operation coordinates: offer *patch* (delete fragments).
  - all the other fragments touching the operation coordinates: mark them as *affected* (text deleted inside).

- `ins` and `mvi` (insert): no hint required, relocations are already processed for `equ`.

- `rep` (replace): all the fragments touching the operation coordinates: mark them as *affected* (text replaced inside). We can't know if and how such fragments might be affected when their underlying text changes.

- `mvd` (delete by moving):
  - fragments whose extent is equal to or less than the operation coordinates: offer *patch* (move fragments to the target position defined by the `mvi` operation with the same group ID).
  - all the other fragments touching the operation coordinates: mark them as *affected* (text moved outside).

Thus, patch implies only these operations:

- move a fragment, i.e. change its coordinates: `mov <old-location> <new-location>`.
- delete a fragment: `del <location>`.
