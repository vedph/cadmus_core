﻿namespace Cadmus.Index.Graph
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
        /// Gets or sets the optional SID for this triple. This is null for
        /// manually created triples.
        /// </summary>
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets a general purpose tag used to mark triples. For instance,
        /// the tag value <c>restriction</c> can be used to tag those triples
        /// representing property restrictions, so that they can be excluded
        /// from the normal users view.
        /// </summary>
        public string Tag { get; set; }

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
