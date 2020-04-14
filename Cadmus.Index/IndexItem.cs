namespace Cadmus.Index
{
    /// <summary>
    /// Subset of item information stored in an items index.
    /// This is used to quickly join pins data to items when reading the index.
    /// </summary>
    public sealed class IndexItem
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Item title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Item short description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Item's facet ID.
        /// </summary>
        /// <value>The facet defines which parts can be stored in the item,
        /// and their order and other presentational attributes. It is a unique
        /// string defined in the corpus configuration.</value>
        public string FacetId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier. This is an arbitrary string
        /// which can be used to group items into a set. For instance, you
        /// might have a set of items belonging to the same literary work,
        /// a set of lemmata belonging to the same dictionary letter, etc.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The sort key for the item. This is a value used to sort items
        /// in a list.
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// Gets or sets generic flags for the item.
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id}: {Title}" + (FacetId != null ? $" [{FacetId}]" : "");
        }
    }
}
