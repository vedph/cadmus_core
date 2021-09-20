namespace Cadmus.Index.Graph
{
    /// <summary>
    /// A restriction on a <see cref="Property"/>.
    /// </summary>
    public class PropertyRestriction
    {
        /// <summary>
        /// Gets or sets the restriction identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the node property this restriction refers to.
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the restriction. Essentially this is limited to any of
        /// subproperty of (<c>rdfs:subPropertyOf</c>), domain (<c>rdfs:domain</c>),
        /// range (<c>rdfs:range</c>), literal only (<c>owl:DataTypeProperty</c>),
        /// object only (<c>owl:ObjectProperty</c>), symmetric
        /// (<c>owl:SymmetricProperty</c>), asymmetric (<c>owl:AsymmetricProperty</c>).
        /// Except for domain/range/subproperty, which are used as predicates
        /// (property - has-range - node), all the other restrictions listed
        /// here are used as objects (property - is-a - object-property).
        /// </summary>
        public string Restriction { get; set; }

        /// <summary>
        /// Gets or sets the optional object identifier used as the object of
        /// domain/range restrictions.
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id}->{PropertyId} {Restriction}" +
                (ObjectId > 0? $" {ObjectId}" : "");
        }
    }
}
