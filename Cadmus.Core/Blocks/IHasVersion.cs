using System;

namespace Cadmus.Core.Blocks
{
    /// <summary>
    /// Interface implemented by objects which are stored with version
    /// information.
    /// </summary>
    public interface IHasVersion
    {
        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        DateTime TimeModified { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        /// <remarks>This is the ID of the user who last modified the object.
        /// </remarks>
        string UserId { get; set; }
    }
}
