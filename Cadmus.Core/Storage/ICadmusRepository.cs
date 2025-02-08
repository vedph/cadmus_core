using System.Collections.Generic;
using System.Threading.Tasks;
using Cadmus.Core.Config;
using Cadmus.Core.Layers;
using Fusi.Tools.Data;

namespace Cadmus.Core.Storage;

/// <summary>
/// Cadmus repository.
/// </summary>
public interface ICadmusRepository
{
    #region Flags
    /// <summary>
    /// Gets the flag definitions.
    /// </summary>
    /// <returns>definitions</returns>
    IList<FlagDefinition> GetFlagDefinitions();

    /// <summary>
    /// Gets the specified flag definition.
    /// </summary>
    /// <param name="id">The flag identifier.</param>
    /// <returns>definition or null if not found</returns>
    FlagDefinition? GetFlagDefinition(int id);

    /// <summary>
    /// Adds or updates the specified flag definition.
    /// </summary>
    /// <param name="definition">The definition.</param>
    void AddFlagDefinition(FlagDefinition definition);

    /// <summary>
    /// Deletes the specified flag definition.
    /// </summary>
    /// <param name="id">The flag identifier.</param>
    void DeleteFlagDefinition(int id);
    #endregion

    #region Facets
    /// <summary>
    /// Gets the item's facets.
    /// </summary>
    /// <returns>facets</returns>
    IList<FacetDefinition> GetFacetDefinitions();

    /// <summary>
    /// Gets the specified item's facet.
    /// </summary>
    /// <param name="id">The facet identifier.</param>
    /// <returns>facet or null if not found</returns>
    FacetDefinition? GetFacetDefinition(string id);

    /// <summary>
    /// Adds or updates the specified facet.
    /// </summary>
    /// <param name="facet">The facet.</param>
    void AddFacetDefinition(FacetDefinition facet);

    /// <summary>
    /// Deletes the specified facet.
    /// </summary>
    /// <param name="id">The facet identifier.</param>
    void DeleteFacetDefinition(string id);
    #endregion

    #region Thesaurus
    /// <summary>
    /// Gets the IDs of all the thesauri, or of all those matching the
    /// specified filter.
    /// </summary>
    /// <param name="filter">The optional filter.</param>
    /// <returns>IDs</returns>
    IList<string> GetThesaurusIds(ThesaurusFilter? filter = null);

    /// <summary>
    /// Gets the specified page of thesauri.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>Page.</returns>
    DataPage<Thesaurus> GetThesauri(ThesaurusFilter filter);

    /// <summary>
    /// Gets the tag set with the specified ID.
    /// </summary>
    /// <param name="id">The tag set ID.</param>
    /// <returns>tag set, or null if not found</returns>
    Thesaurus? GetThesaurus(string id);

    /// <summary>
    /// Adds or updates the specified tag set.
    /// </summary>
    /// <param name="thesaurus">The set.</param>
    void AddThesaurus(Thesaurus thesaurus);

    /// <summary>
    /// Deletes the specified tag set.
    /// </summary>
    /// <param name="id">The tag set ID.</param>
    void DeleteThesaurus(string id);
    #endregion

    #region Items
    /// <summary>
    /// Gets a page of items.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>items page</returns>
    DataPage<ItemInfo> GetItems(ItemFilter filter);

    /// <summary>
    /// Gets the specified item.
    /// </summary>
    /// <param name="id">The item's identifier.</param>
    /// <param name="includeParts">if set to <c>true</c>, include all the
    /// item's parts.</param>
    /// <returns>item or null if not found</returns>
    IItem? GetItem(string id, bool includeParts = true);

    /// <summary>
    /// Adds or updates the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="history">if set to <c>true</c>, the history should be
    /// affected.</param>
    void AddItem(IItem item, bool history = true);

    /// <summary>
    /// Deletes the item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="history">if set to <c>true</c>, the history should be
    /// affected.</param>
    void DeleteItem(string id, string userId, bool history = true);

    /// <summary>
    /// Set the flags of the item(s) with the specified ID(s).
    /// Note that this operation never affects the item's history.
    /// </summary>
    /// <param name="ids">The item identifier(s).</param>
    /// <param name="flags">The flags value.</param>
    void SetItemFlags(IList<string> ids, int flags);

    /// <summary>
    /// Set the group ID of the item(s) with the specified ID(s).
    /// Note that this operation never affects the item's history.
    /// </summary>
    /// <param name="ids">The items identifiers.</param>
    /// <param name="groupId">The group ID (can be null).</param>
    void SetItemGroupId(IList<string> ids, string? groupId);

    /// <summary>
    /// Gets the requested page from a list of all the distinct, non-null
    /// group IDs found in the items.
    /// </summary>
    /// <param name="options">The paging options.</param>
    /// <param name="filter">The optional filter to be found inside any group
    /// ID (case insensitive).</param>
    /// <returns>The page.</returns>
    Task<DataPage<string>> GetDistinctGroupIdsAsync(PagingOptions options,
        string? filter = null);

    /// <summary>
    /// Get the count of all the non-empty layers in the specified items
    /// group.
    /// </summary>
    /// <param name="groupId">The group ID.</param>
    /// <returns>The count.</returns>
    Task<int> GetGroupLayersCountAsync(string groupId);

    /// <summary>
    /// Gets a pege of history for the specified item.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>history items page</returns>
    DataPage<HistoryItemInfo> GetHistoryItems(HistoryItemFilter filter);

    /// <summary>
    /// Gets the specified history item.
    /// </summary>
    /// <param name="id">The history item's identifier.</param>
    /// <returns>item or null if not found</returns>
    HistoryItem? GetHistoryItem(string id);

    /// <summary>
    /// Deletes the specified history item.
    /// </summary>
    /// <param name="id">The history item's identifier.</param>
    void DeleteHistoryItem(string id);
    #endregion

    #region Parts
    /// <summary>
    /// Gets the specified page of parts.
    /// </summary>
    /// <param name="filter">The parts filter.</param>
    /// <returns>Parts page.</returns>
    DataPage<PartInfo> GetParts(PartFilter filter);

    /// <summary>
    /// Gets the parts belonging to the specified item(s), eventually
    /// filtered by their type
    /// and/or role.
    /// </summary>
    /// <param name="itemIds">The item ID(s).</param>
    /// <param name="typeId">The optional type identifier.</param>
    /// <param name="roleId">The optional role identifier.</param>
    /// <returns>parts</returns>
    IList<IPart> GetItemParts(string[] itemIds, string? typeId = null,
        string? roleId = null);

    /// <summary>
    /// Gets layer parts information about the item with the specified ID.
    /// </summary>
    /// <param name="itemId">The item's identifier.</param>
    /// <param name="absent">if set to <c>true</c>, include also information
    /// about absent parts, i.e. those parts which are not present in the
    /// repository, but are defined in the item's facet.</param>
    /// <returns>layers parts information.</returns>
    IList<LayerPartInfo> GetItemLayerInfo(string itemId, bool absent);

    /// <summary>
    /// Gets the specified part.
    /// </summary>
    /// <typeparam name="T">the type of the part to retrieve</typeparam>
    /// <param name="id">The part identifier.</param>
    /// <returns>part or null if not found</returns>
    T? GetPart<T>(string id) where T : class, IPart;

    /// <summary>
    /// Gets the identifier of the item including the part with the
    /// specified part identifier.
    /// </summary>
    /// <param name="id">The part identifier.</param>
    /// <returns>The item identifier, or null if part not found</returns>
    string? GetPartItemId(string id);

    /// <summary>
    /// Gets the code representing the part with the specified ID.
    /// </summary>
    /// <param name="id">The part identifier.</param>
    /// <returns>JSON code or null if not found</returns>
    string? GetPartContent(string id);

    /// <summary>
    /// Adds or updates the specified part.
    /// </summary>
    /// <param name="part">The part.</param>
    /// <param name="history">if set to <c>true</c>, the history should be
    /// affected.</param>
    void AddPart(IPart part, bool history = true);

    /// <summary>
    /// Adds or updates the specified part from its encoded content.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="history">if set to <c>true</c>, the history should be
    /// affected.</param>
    void AddPartFromContent(string content, bool history = true);

    /// <summary>
    /// Deletes the specified part.
    /// </summary>
    /// <param name="id">The part's identifier.</param>
    /// <param name="userId">The identifier of the user deleting the part.
    /// </param>
    /// <param name="history">if set to <c>true</c>, the history should be
    /// affected.</param>
    void DeletePart(string id, string userId, bool history = true);

    /// <summary>
    /// Gets a page of history parts.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>history parts result</returns>
    DataPage<HistoryPartInfo> GetHistoryParts(HistoryPartFilter filter);

    /// <summary>
    /// Gets the specified history part.
    /// </summary>
    /// <typeparam name="T">The part type.</typeparam>
    /// <param name="id">The identifier.</param>
    /// <returns>part or null if not found</returns>
    HistoryPart<T>? GetHistoryPart<T>(string id) where T : class, IPart;

    /// <summary>
    /// Deletes the specified history part.
    /// </summary>
    /// <param name="id">The history part's identifier.</param>
    void DeleteHistoryPart(string id);

    /// <summary>
    /// Gets the ID of the creator of the part with the specified ID.
    /// </summary>
    /// <param name="id">The part ID.</param>
    /// <returns>Creator ID, or null.</returns>
    string? GetPartCreatorId(string id);

    /// <summary>
    /// Determines whether the layer part with the specified ID might
    /// potentially have been broken because of changes in its base text.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="toleranceSeconds">The count of seconds representing
    /// the tolerance time interval between a base text save time and that
    /// of its layer part. Once this interval has elapsed, the layer part
    /// is not considered as potentially broken. If set to 0 or less, no
    /// tolerance interval is allowed.</param>
    /// <returns>
    /// <c>0</c> if the layer part is not potentially broken; <c>1</c>
    /// if it's potentially broken; <c>2</c> if it's surely broken.
    /// </returns>
    /// <remarks>A layer part is potentially broken when the corresponding
    /// text part has been saved (with a different text) either after it,
    /// or a few time before it.
    /// In both cases, this implies that the part fragments might have
    /// broken links, as the underlying text was in some way changed.
    /// To detect a potential break we can just check for last modified date
    /// and time; if the above conditions for save date and time are not met,
    /// the method can return false. If instead they are met, we must ensure
    /// that text has changed. To this end, we must go back in the text part
    /// history to find the latest save which changed the text, and refer
    /// to its date and time.
    /// </remarks>
    int GetLayerPartBreakChance(string id, int toleranceSeconds);

    /// <summary>
    /// Gets the layer part reconciliation hints.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The hints, one for each fragment in the layer part.
    /// If any of the required resources is not found, an empty list
    /// will be returned.</returns>
    IList<LayerHint> GetLayerPartHints(string id);

    /// <summary>
    /// Applies the specified patches instructions to the layer part
    /// with ID equal to <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The layer part identifier.</param>
    /// <param name="userId">The user identifier. This will be set as
    /// the author of the changes in the part.</param>
    /// <param name="patches">The patch instructions.</param>
    /// <returns>The patched layer part content or null if layer part
    /// not found.</returns>
    string? ApplyLayerPartPatches(string id, string userId, IList<string> patches);

    /// <summary>
    /// Set the <see cref="IPart.ThesaurusScope"/> property of all the
    /// parts with the specified ID.
    /// Note that this operation never affects the item's history.
    /// </summary>
    /// <param name="ids">The item identifier(s).</param>
    /// <param name="scope">The new scope (may be null).</param>
    void SetPartThesaurusScope(IList<string> ids, string? scope);
    #endregion

    #region Settings    
    /// <summary>
    /// Adds or updates the settings with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="json">The JSON code representing the settings value.</param>
    void AddSetting(string key, string json);

    /// <summary>
    /// Gets the setting with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The JSON code representing the settings value, or null
    /// if not found.</returns>
    string? GetSetting(string key);

    /// <summary>
    /// Deletes the setting with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    void DeleteSetting(string key);
    #endregion
}
