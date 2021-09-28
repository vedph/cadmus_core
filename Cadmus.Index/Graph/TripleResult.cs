namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A result of searching a <see cref="Triple"/>.
    /// </summary>
    /// <seealso cref="Triple" />
    public class TripleResult : Triple
    {
        /// <summary>
        /// Gets or sets the subject node URI.
        /// </summary>
        public string SubjectUri { get; set; }

        /// <summary>
        /// Gets or sets the predicate node URI.
        /// </summary>
        public string PredicateUri { get; set; }

        /// <summary>
        /// Gets or sets the object node URI.
        /// </summary>
        public string ObjectUri { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id}: {SubjectUri} - #{PredicateUri} - " +
                (ObjectId == 0 ? ObjectLiteral : ObjectUri);
        }
    }
}
