namespace Cadmus.Parts.General
{
    /// <summary>
    /// Bibliographic entry renderer.
    /// </summary>
    public interface IBibEntryRenderer
    {
        /// <summary>
        /// Renders the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>String.</returns>
        string Render(BibEntry entry);
    }
}
