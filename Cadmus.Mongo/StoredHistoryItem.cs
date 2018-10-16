using System;
using Cadmus.Core.Blocks;
using Cadmus.Core.Storage;

namespace Cadmus.Mongo
{
    /// <summary>
    /// A history item as stored in Mongo database.
    /// </summary>
    public sealed class StoredHistoryItem : StoredItem, IHasHistory
    {
        /// <summary>
        /// The collection name.
        /// </summary>
        public new const string COLLECTION = "history-items";

        /// <summary>
        /// Gets or sets the identifier of the data record this history record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredHistoryItem"/> class.
        /// </summary>
        /// <param name="item">The source item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public StoredHistoryItem(IItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = Guid.NewGuid().ToString("N");
            ReferenceId = item.Id;
            Title = item.Title;
            Description = item.Description;
            FacetId = item.FacetId;
            SortKey = item.SortKey;
            Flags = item.Flags;
            UserId = item.UserId;
            TimeModified = item.TimeModified;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredHistoryItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public StoredHistoryItem(StoredItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = Guid.NewGuid().ToString("N");
            ReferenceId = item.Id;
            Title = item.Title;
            Description = item.Description;
            FacetId = item.FacetId;
            SortKey = item.SortKey;
            Flags = item.Flags;
            UserId = item.UserId;
            TimeModified = item.TimeModified;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredHistoryItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">null item</exception>
        public StoredHistoryItem(StoredHistoryItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Id = Guid.NewGuid().ToString("N");
            ReferenceId = item.Id;
            Title = item.Title;
            Description = item.Description;
            FacetId = item.FacetId;
            SortKey = item.SortKey;
            Flags = item.Flags;
            UserId = item.UserId;
            TimeModified = item.TimeModified;
            Status = item.Status;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"[{Status}] {base.ToString()}";
        }
    }
}
