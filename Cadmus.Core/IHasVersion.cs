using System;

namespace Cadmus.Core
{
    /// <summary>
    /// Interface implemented by objects which are stored with version
    /// information.
    /// </summary>
    public interface IHasVersion
    {
        /// <summary>
        /// Creation date and time (UTC).
        /// </summary>
        DateTime TimeCreated { get; set; }

        /// <summary>
        /// ID of the user who created the resource.
        /// </summary>
        string CreatorId { get; set; }

        /// <summary>
        /// Last saved date and time (UTC).
        /// </summary>
        DateTime TimeModified { get; set; }

        /// <summary>
        /// ID of the user who last saved the resource.
        /// </summary>
        string UserId { get; set; }
    }
}
