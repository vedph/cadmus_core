using System;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// A bibliographic entry in a bibliography part.
    /// </summary>
    public class BibEntry
    {
        private static readonly IBibEntryRenderer _renderer =
            new StandardBibEntryRenderer();

        /// <summary>
        /// Gets or sets an optional, human-friendly key which can be used
        /// to reference this item. For instance, typical keys are built from
        /// author names + year, like <c>Rossi 1963</c>, author names + container
        /// + number, like <c>Rossi in RFIC 1965</c>, etc. The criteria for
        /// building this key are up to the consumer.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the type identifier for the entry and its container:
        /// e.g. book, journal article, book article, proceedings article,
        /// journal review, ebook, site, magazine, newspaper, tweet, TV series,
        /// etc.
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets an optional tag, which can be used to categorize or
        /// group an entry in some way.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the authors, in their desired order.
        /// </summary>
        public BibAuthor[] Authors { get; set; }

        /// <summary>
        /// Gets or sets the title. As per MLA, in a tweet the title is equal
        /// to the tweet's full content.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the language (usually ISO 639-3).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the container: this can be a journal, a book,
        /// a set of proceedings, etc.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the optional contributors, in their desired order.
        /// Typically they are associated with <see cref="Container"/>, e.g.
        /// the editors of a book collecting several articles.
        /// </summary>
        public BibAuthor[] Contributors { get; set; }

        /// <summary>
        /// Gets or sets the optional edition number.
        /// </summary>
        public short Edition { get; set; }

        /// <summary>
        /// Gets or sets the optional number. For instance, this is the
        /// number of a journal's issue, and could be alphanumeric (e.g.
        /// <c>n.s.12</c>, <c>13,4</c>, etc.).
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the optional publisher's name.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the optional year of publication.
        /// </summary>
        public short YearPub { get; set; }

        /// <summary>
        /// Gets or sets the optional place(s) of publication. Separate
        /// several places with commas.
        /// </summary>
        public string PlacePub { get; set; }

        /// <summary>
        /// Gets or sets the location identifier for the bibliographic item,
        /// e.g. an URL or a DOI.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the optional last access date. This is typically used
        /// for web-based resources.
        /// </summary>
        public DateTime? AccessDate { get; set; }

        /// <summary>
        /// Gets or sets the first page number.
        /// </summary>
        public short FirstPage { get; set; }

        /// <summary>
        /// Gets or sets the last page number.
        /// </summary>
        public short LastPage { get; set; }

        /// <summary>
        /// Gets or sets the keywords optionally attached to this entry.
        /// </summary>
        public Keyword[] Keywords { get; set; }

        /// <summary>
        /// Gets or sets an optional general purpose note about this entry.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Converts into a MLA-like string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _renderer.Render(this);
        }
    }
}
