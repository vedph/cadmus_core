using System;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// A history part wrapper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HistoryPart<T> where T : class, IPart
    {
        /// <summary>
        /// Gets or sets the history record identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the wrapped part.
        /// </summary>
        public T Part { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        public DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.
        /// </remarks>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data record (part) this history
        /// record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryPart{T}"/> class.
        /// </summary>
        /// <param name="id">The history part ID, or null for a new ID.</param>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HistoryPart(string id, T part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            Id = id ?? Guid.NewGuid().ToString();
            ReferenceId = part.Id;
            Part = part;
            TimeModified = DateTime.UtcNow;
            UserId = "";
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Id} -> {ReferenceId}: {Status}";
        }
    }
}
