namespace Cadmus.Parts
{
    /// <summary>
    /// A general purpose, 3-levels short document reference: author, work,
    /// location, plus some optional metadata. This model can be applied to
    /// literary references, bibliographic references, archive documents
    /// references, etc.
    /// </summary>
    public class DocReference
    {
        /// <summary>
        /// Any classification tag meaningful in the data context.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The author ID (e.g. <c>Hom.</c>). Apart from this name, which
        /// reflects its prevalent usage, this is the 1st level of the reference.
        /// For archive documents, it can be a constant reserved ID. For
        /// bibliographic references, it's the modern author ID.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The work ID (e.g. <c>Il.</c>). Apart from this name, which reflects
        /// its prevalent usage, this is the 2nd level of the reference. For
        /// archive documents, it can be the archive name. For bibliographic
        /// references, it's the modern work's title ID.
        /// </summary>
        public string Work { get; set; }

        /// <summary>
        /// The work's location (e.g. <c>12.34</c>). Apart from this name, which
        /// reflects its prevalent usage, this is the 3rd level of the reference.
        /// For archive documents, it can be their location in the archive
        /// (e.g. a signature). For bibliographic references, it's usually a
        /// page number or other means of locating some passage.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// A generic annotation.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>String.</returns>
        public override string ToString()
        {
            return $"{Author}, {Work}, {Location}" +
                (string.IsNullOrEmpty(Tag) ? "" : $" [{Tag}]");
        }
    }
}
