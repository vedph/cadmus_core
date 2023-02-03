using System;

namespace Cadmus.Core.Config;

/// <summary>
/// Part type provider. This is used to get the part type from its part ID.
/// </summary>
public interface IPartTypeProvider
{
    /// <summary>
    /// Gets the type from its identifier.
    /// </summary>
    /// <param name="id">The part or fragment identifier.</param>
    /// <returns>
    /// part/fragment type, or null if not found
    /// </returns>
    Type? Get(string id);
}
