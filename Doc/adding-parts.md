# Adding Parts

Server-side parts in the editor are essentially used for indexing. Anyway, their model and logic can be used by any other 3rd party processor for its own purposes.

## Parts

Guidelines for **implementing a part**:

- *derive* from `PartBase`, even if this is not strictly a requirement, but rather a commodity. The part class must anyway implement the `IPart` interface.

- *decorate* the class with a `TagAttribute` providing the part's type ID.

- *do not add any logic* to the part. The part is just a POCO object modeling the data it represents, and should have no logic. The only piece of logic required is the method returning the part's data pins, which is just a form of reflecting on the part's data themselves, e.g. for indexing.

- if creating a part representing a *base text* for text layers, implement the `IHasText` interface by providing a `GetText()` method which, whatever the part's model, produces a single string representing its whole text. The same interface should be implemented whenever your part has some rather long piece of free, unstructured text you might want to be included in processes like full-text indexing. Also, set the role ID to `base-text` (defined in `PartBase.BASE_TEXT_ROLE_ID`).

- consider that the part will be subject to automatic serialization and deserialization. As the part is just a POCO object, this should not pose any issue.

**Part template** sample (in the following template replace `__NAME__` with your part's name, minus the `Part` suffix):

```cs
/// <summary>
/// Tag: <c>net.fusisoft.__NAME__</c>.
/// </summary>
[Tag("net.fusisoft.__NAME__")]
public class __NAME__Part : PartBase
{
    // TODO: add your properties here...

    /// <summary>
    /// Get all the key=value pairs (pins) exposed by the implementor.
    /// </summary>
    /// <param name="item">The optional item. The item with its parts
    /// can optionally be passed to this method for those parts requiring
    /// to access further data.</param>
    /// <returns>The pins.</returns>
    public override IEnumerable<DataPin> GetDataPins(IItem item)
    {
        // TODO: implement indexing logic...
        // sample:
        // return Tag != null
        //    ? new[]
        //    {
        //        CreateDataPin("tag", Tag)
        //    }
        //    : Enumerable.Empty<DataPin>();
    }

    /// <summary>
    /// Returns a <see cref="String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("[__NAME__]");

        // TODO: append summary data...

        return sb.ToString();
    }
}
```

**Test template** sample: the part being usually a DTO object, its logic is found only in indexing. Thus, the tests must ensure that the object is serializable, and that the pins are created as expected. In the following template replace `__NAME__` with your part's name, minus the `Part` suffix.

```cs
public sealed class __NAME__PartTest
{
    private static __NAME__Part GetPart()
    {
        return new __NAME__Part
        {
            ItemId = Guid.NewGuid().ToString(),
            RoleId = "some-role",
            CreatorId = "zeus",
            UserId = "another",
            // TODO: set parts data here...
        };
    }

    [Fact]
    public void Part_Is_Serializable()
    {
        __NAME__Part part = GetPart();

        string json = TestHelper.SerializePart(part);
        __NAME__Part part2 = TestHelper.DeserializePart<__NAME__Part>(json);

        Assert.Equal(part.Id, part2.Id);
        Assert.Equal(part.TypeId, part2.TypeId);
        Assert.Equal(part.ItemId, part2.ItemId);
        Assert.Equal(part.RoleId, part2.RoleId);
        Assert.Equal(part.CreatorId, part2.CreatorId);
        Assert.Equal(part.UserId, part2.UserId);
        // TODO: check parts data here...
    }

    // TODO: check pins here, e.g. for the NotePart we get a single pin
    // when the tag is set, with name=tag and value=tag value:
    // [Fact]
    // public void GetDataPins_NoTag_Empty()
    // {
    //     __NAME__Part part = GetPart();
    //     part.Tag = null;

    //     Assert.Empty(part.GetDataPins());
    // }

    // [Fact]
    // public void GetDataPins_Tag_1()
    // {
    //     __NAME__Part part = GetPart();

    //     List<DataPin> pins = part.GetDataPins().ToList();
    //     Assert.Single(pins);

    //     DataPin pin = pins[0];
    //     Assert.Equal(part.ItemId, pin.ItemId);
    //     Assert.Equal(part.Id, pin.PartId);
    //     Assert.Equal(part.RoleId, pin.RoleId);
    //     Assert.Equal("tag", pin.Name);
    //     Assert.Equal("some-tag", pin.Value);
    // }
}
```

## Layer Parts

For layer parts, the same guidelines already listed for the other parts are applicable, with the following additions:

- create a `...LayerFragment` class representing the fragment for the layer part. This is the true data model for the metatextual data represented by the layer. The class must implement `ITextLayerFragment`. Do not add any other property to the class; *by design, the only property of a layer part is its collection of fragments*.

- give the fragment a type ID (via the usual `TagAttribute`), which *must* begin with the prefix `fr.` (=`PartBase.FR_PREFIX`; note the trailing dot).

- if adding pins in the fragment, just provide the pin's name and value; the other properties will be supplied by the container part. By convention, you should prefix your pin name with the `fr.` prefix (defined in `PartBase.FR_PREFIX`).

Anyway, adding a new layer part would be rarely required, as there is just a generic (parameterized) layer part provided for this: one part, many fragments. You rather have to provide fragments and their tests.

**Fragment test template** sample:

```cs
public sealed class __NAME__LayerFragmentTest
{
    private static __NAME__LayerFragment GetFragment()
    {
        return new __NAME__LayerFragment
        {
            Location = "1.23",
            // TODO: add properties here...
        };
    }

    [Fact]
    public void Fragment_Has_Tag()
    {
        TagAttribute attr = typeof(__NAME__LayerFragment).GetTypeInfo()
            .GetCustomAttribute<TagAttribute>();
        string typeId = attr != null ? attr.Tag : GetType().FullName;
        Assert.NotNull(typeId);
        Assert.StartsWith(PartBase.FR_PREFIX, typeId);
    }

    [Fact]
    public void Fragment_Is_Serializable()
    {
        __NAME__LayerFragment fr = GetFragment();

        string json = TestHelper.SerializeFragment(fr);
        __NAME__LayerFragment fr2 =
            TestHelper.DeserializeFragment<__NAME__LayerFragment>(json);

        Assert.Equal(fr.Location, fr2.Location);
        // TODO: check properties here...
    }

    // TODO: check pins here, e.g. for the CommentLayerFragment
    // we get a single pin when the tag is set, with name=fr.tag
    // and value=tag value:
    // [Fact]
    // public void GetDataPins_NoTag_0()
    // {
    //     CommentLayerFragment fr = GetFragment();
    //     fr.Tag = null;

    //     Assert.Empty(fr.GetDataPins());
    // }

    // [Fact]
    // public void GetDataPins_Tag_1()
    // {
    //     CommentLayerFragment fr = GetFragment();

    //     List<DataPin> pins = fr.GetDataPins().ToList();

    //     Assert.Single(pins);
    //     DataPin pin = pins[0];
    //     Assert.Equal("fr.tag", pin.Name);
    //     Assert.Equal("some-tag", pin.Value);
    // }
}
```
