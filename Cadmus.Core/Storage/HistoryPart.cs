using System;
using Cadmus.Core.Blocks;

namespace Cadmus.Core.Storage
{
    /// <summary>
    /// A history part wrapper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HistoryPart<T> : IHistoryPart<T> where T:class,IPart
    {
        /// <summary>
        /// Gets or sets the history record identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the wrapped part.
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the data record (part) this history record refers to.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public EditStatus Status { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryPart{T}"/> class.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HistoryPart(T part)
        {
            if (part == null) throw new ArgumentNullException(nameof(part));

            ReferenceId = part.Id;
            Content = part;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Content?.ToString() ?? base.ToString();
        }
    }
}
