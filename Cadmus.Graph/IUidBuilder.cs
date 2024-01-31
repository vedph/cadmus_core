namespace Cadmus.Graph;

/// <summary>
/// UID builder interface. An UID builder takes a SID and an unsuffixed
/// UID, and returns an eventually suffixed UID, when the requested UID
/// should be unique.
/// </summary>
public interface IUidBuilder
{
    /// <summary>
    /// Build the eventually suffixed UID.
    /// </summary>
    /// <param name="unsuffixed">The UID as calculated from its source,
    /// without any suffix. If the caller wants a unique new UID for it, it
    /// must be suffixed with <c>##</c>: this suffix will be either removed
    /// when no entry is present with the same <paramref name="unsuffixed"/>
    /// value, or replaced by a numeric suffix preceded by <c>#</c> to grant
    /// its uniqueness.</param>
    /// <param name="sid">The source ID (SID).</param>
    /// <returns>UID, eventually suffixed with #N.</returns>
    string BuildUid(string unsuffixed, string sid);
}
