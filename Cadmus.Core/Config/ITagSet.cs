using System.Collections.Generic;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// Tag set interface.
    /// </summary>
    public interface ITagSet
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        List<Tag> Tags { get; set; }
    }
}
