namespace Cadmus.Core
{
    /// <summary>
    /// Interface implemented by parts which contain some sort of continuous
    /// text, which could be used as a document for indexing purposes.
    /// </summary>
    public interface IHasText
    {
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns>full text</returns>
        string GetText();
    }
}
