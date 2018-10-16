using System;
using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Content wrapper for tracking editing history.
    /// </summary>
    /// <typeparam name="T">type of content</typeparam>
    public sealed class HistoryWrapper<T> : IHasVersion where T : class
    {
        /// <summary>
        /// Gets the content.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Gets or sets the ID of the history wrapper.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the status of the contained object.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.</remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryWrapper{T}"/> class.
        /// </summary>
        /// <remarks>A new <see cref="Id"/> value is automatically generated.</remarks>
        /// <param name="content">The content.</param>
        /// <exception cref="ArgumentNullException">null content</exception>
        public HistoryWrapper(T content)
        {
            Id = Guid.NewGuid().ToString("N");
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }

    /// <summary>
    /// Edit status.
    /// </summary>
    public enum EditStatus : byte
    {
        /// <summary>The item has been created.</summary>
        Created = 0,
        /// <summary>The item was existing and has been updated.</summary>
        Updated = 1,
        /// <summary>The item has been deleted. This is a status typically
        /// used in history.</summary>
        Deleted = 2
    }
}
