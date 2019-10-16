namespace Cadmus.Core.Config
{
    /// <summary>
    /// A serializer for <see cref="DataProfile"/>'s.
    /// </summary>
    public interface IDataProfileSerializer
    {
        /// <summary>
        /// Reads the profile from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The profile.</returns>
        DataProfile Read(string text);
    }
}
