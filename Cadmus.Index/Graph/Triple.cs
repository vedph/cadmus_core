namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A triple.
    /// </summary>
    public class Triple
    {
        /// <summary>
        /// Gets or sets the triple's identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the predicate identifier.
        /// </summary>
        public int PredicateId { get; set; }

        /// <summary>
        /// Gets or sets the object identifier.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the object literal value. This is alternative to
        /// <see cref="ObjectId"/>.
        /// </summary>
        public string ObjectLiteral { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id}: #{SubjectId} - #{PredicateId} - " +
                (ObjectId == 0? ObjectLiteral : "#" + ObjectId);
        }
    }
}
