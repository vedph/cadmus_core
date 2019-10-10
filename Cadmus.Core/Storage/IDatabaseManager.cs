namespace Cadmus.Core.Storage
{
    /// <summary>
    /// Cadmus database manager.
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <param name="profile">The database profile.</param>
        void CreateDatabase(string source, string profile);

        /// <summary>
        /// Deletes the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        void DeleteDatabase(string source);

        /// <summary>
        /// Clears the database.
        /// </summary>
        /// <param name="source">The database source.</param>
        void ClearDatabase(string source);

        /// <summary>
        /// Databases the exists.
        /// </summary>
        /// <param name="source">The database source.</param>
        /// <returns>true if the database exists</returns>
        bool DatabaseExists(string source);
    }
}
