using System;
using System.Collections.Generic;
using Cadmus.Core.Blocks;
using Cadmus.Core.Config;
using Fusi.Tools.Data;

namespace Cadmus.Core.Storage
{
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
        IList<IFlagDefinition> GetFlagDefinitions();

        /// <summary>
        /// Gets the specified flag definition.
        /// </summary>
        /// <param name="id">The flag identifier.</param>
        /// <returns>definition or null if not found</returns>
        IFlagDefinition GetFlagDefinition(int id);

        /// <summary>
        /// Adds or updates the specified flag definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        void AddFlagDefinition(IFlagDefinition definition);

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
        IList<IFacet> GetFacets();

        /// <summary>
        /// Gets the specified item's facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        /// <returns>facet or null if not found</returns>
        IFacet GetFacet(string id);

        /// <summary>
        /// Adds or updates the specified facet.
        /// </summary>
        /// <param name="facet">The facet.</param>
        void AddFacet(IFacet facet);

        /// <summary>
        /// Deletes the specified facet.
        /// </summary>
        /// <param name="id">The facet identifier.</param>
        void DeleteFacet(string id);
        #endregion

        #region Tags
        /// <summary>
        /// Gets the IDs of all the tags sets.
        /// </summary>
        /// <returns>IDs</returns>
        IList<string> GetTagSetIds();

        /// <summary>
        /// Gets the tag set with the specified ID.
        /// </summary>
        /// <param name="id">The tag set ID.</param>
        /// <returns>tag set, or null if not found</returns>
        TagSet GetTagSet(string id);

        /// <summary>
        /// Adds or updates the specified tag set.
        /// </summary>
        /// <param name="set">The set.</param>
        void AddTagSet(TagSet set);

        /// <summary>
        /// Deletes the specified tag set.
        /// </summary>
        /// <param name="id">The tag set ID.</param>
        void DeleteTagSet(string id);
        #endregion

        #region Items
        /// <summary>
        /// Gets a page of items.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>items page</returns>
        PagedData<IItemInfo> GetItemsPage(ItemFilter filter);

        /// <summary>
        /// Gets the specified item.
        /// </summary>
        /// <param name="id">The item's identifier.</param>
        /// <param name="includeParts">if set to <c>true</c>, include all the item's parts.</param>
        /// <returns>item or null if not found</returns>
        IItem GetItem(string id, bool includeParts = true);

        /// <summary>
        /// Adds or updates the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="history">if set to <c>true</c>, the history should be affected.</param>
        void AddItem(IItem item, bool history = true);

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="history">if set to <c>true</c>, the history should be affected.</param>
        void DeleteItem(string id, string userId, bool history = true);

        /// <summary>
        /// Sets the item flags.
        /// </summary>
        /// <param name="id">The item identifier.</param>
        /// <param name="flags">The flags value.</param>
        void SetItemFlags(string id, int flags);

        /// <summary>
        /// Gets a pege of history for the specified item.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history items page</returns>
        PagedData<IHistoryItemInfo> GetHistoryItemsPage(HistoryItemFilter filter);

        /// <summary>
        /// Gets the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        /// <returns>item or null if not found</returns>
        IHistoryItem GetHistoryItem(string id);

        /// <summary>
        /// Adds the specified history item.
        /// </summary>
        /// <param name="item">The item.</param>
        void AddHistoryItem(IHistoryItem item);

        /// <summary>
        /// Deletes the specified history item.
        /// </summary>
        /// <param name="id">The history item's identifier.</param>
        void DeleteHistoryItem(string id);
        #endregion

        #region Parts
        /// <summary>
        /// Gets the specified page of matching parts, or all the matching parts when page size=0.
        /// </summary>
        /// <param name="filter">The parts filter.</param>
        /// <returns>parts page result</returns>
        PagedData<IPartInfo> GetPartsPage(PartFilter filter);

        /// <summary>
        /// Gets the parts belonging to the specified item(s), eventually filtered by their type
        /// and/or role.
        /// </summary>
        /// <param name="itemIds">The item ID(s).</param>
        /// <param name="typeId">The optional type identifier.</param>
        /// <param name="roleId">The optional role identifier.</param>
        /// <returns>parts</returns>
        IList<IPart> GetItemParts(string[] itemIds, string typeId = null, string roleId = null);

        /// <summary>
        /// Gets the layer parts role IDs and part IDs for the specified item.
        /// This is useful when you want to have a list of all the item's layer parts
        /// IDs (part ID and role ID) so that you can retrieve each of them separately.
        /// </summary>
        /// <param name="id">The item's identifier.</param>
        /// <returns>array of tuples where 1=role ID and 2=part ID</returns>
        /// <exception cref="ArgumentNullException">null item ID</exception>
        List<Tuple<string, string>> GetItemLayerPartIds(string id);

        /// <summary>
        /// Gets the specified part.
        /// </summary>
        /// <typeparam name="T">the type of the part to retrieve</typeparam>
        /// <param name="id">The part identifier.</param>
        /// <returns>part or null if not found</returns>
        T GetPart<T>(string id) where T : class, IPart;

        /// <summary>
        /// Gets the JSON code representing the part with the specified ID.
        /// </summary>
        /// <param name="id">The part identifier.</param>
        /// <returns>JSON code or null if not found</returns>
        string GetPartJson(string id);

        /// <summary>
        /// Adds or updates the specified part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="history">if set to <c>true</c>, the history should be affected.</param>
        void AddPart(IPart part, bool history = true);

        /// <summary>
        /// Adds or updates the part represented by the specified JSON code.
        /// </summary>
        /// <param name="json">The JSON code representing the part.</param>
        /// <param name="history">if set to <c>true</c>, the history should be affected.</param>
        void AddPartJson(string json, bool history = true);

        /// <summary>
        /// Deletes the specified part.
        /// </summary>
        /// <param name="id">The part's identifier.</param>
        /// <param name="userId">The identifier of the user deleting the part.</param>
        /// <param name="history">if set to <c>true</c>, the history should be affected.</param>
        void DeletePart(string id, string userId, bool history = true);

        /// <summary>
        /// Gets a page of history parts.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>history parts result</returns>
        PagedData<IHistoryPartInfo> GetHistoryPartsPage(HistoryPartFilter filter);

        /// <summary>
        /// Gets the specified history part.
        /// </summary>
        /// <typeparam name="T">The part type.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>part or null if not found</returns>
        IHistoryPart<T> GetHistoryPart<T>(string id) where T : class, IPart;

        /// <summary>
        /// Adds the specified history part.
        /// </summary>
        /// <typeparam name="T">The part type.</typeparam>
        /// <param name="part">The part.</param>
        void AddHistoryPart<T>(IHistoryPart<T> part) where T : class, IPart;

        /// <summary>
        /// Deletes the specified history part.
        /// </summary>
        /// <param name="id">The history part's identifier.</param>
        void DeleteHistoryPart(string id);
        #endregion
    }
}
