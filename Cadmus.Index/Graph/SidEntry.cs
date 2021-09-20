namespace Cadmus.Index.Graph
{
    /// <summary>
    /// An entry in the SIDs lookup set.
    /// </summary>
    public class SidEntry
    {
        /// <summary>
        /// Gets or sets the source identifier (SID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the unsuffixed portion of the SID.
        /// </summary>
        public string Unsuffixed { get; set; }

        /// <summary>
        /// Gets or sets the numeric suffix added to <see cref="Unsuffixed"/>
        /// to build the full SID.
        /// </summary>
        public int Suffix { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }
    }
}
