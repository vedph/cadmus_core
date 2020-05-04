using System;
using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Keywords part.
    /// Tag: <c>net.fusisoft.keywords</c>.
    /// </summary>
    /// <remarks>This part contains any number of <see cref="Keyword"/>'s,
    /// each with its own language and value.
    /// </remarks>
    [Tag("net.fusisoft.keywords")]
    public sealed class KeywordsPart : PartBase
    {
        /// <summary>
        /// Keywords.
        /// </summary>
        public List<Keyword> Keywords { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeywordsPart()
        {
            Keywords = new List<Keyword>();
        }

        /// <summary>
        /// Adds a keyword with the specified language and value.
        /// If such a keyword already exists, nothing is done.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">language or value</exception>
        public void AddKeyword(string language, string value)
        {
            if (language == null) throw new ArgumentNullException(nameof(language));
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (Keywords.Find(k => k.Language == language &&
                                   k.Value == value) == null)
            {
                Keywords.Add(new Keyword
                {
                    Language = language,
                    Value = value
                });
            }
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor. Each key is
        /// <c>keyword.{lang}</c> where <c>{lang}</c> is its language value,
        /// e.g. <c>keyword.eng</c> as name and <c>sample</c> as value.
        /// The pins are returned sorted by language and then by value.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Keywords == null || Keywords.Count == 0)
                return Enumerable.Empty<DataPin>();

            List<DataPin> pins = new List<DataPin>();

            var keysByLang = from k in Keywords
                             group k by k.Language
                                 into g
                             orderby g.Key
                             select g;

            foreach (var g in keysByLang)
            {
                string[] values = (from k in g
                                  orderby k.Value
                                  select k.Value).ToArray();

                pins.AddRange(from value in values
                              select CreateDataPin($"keyword.{g.Key}", value));
            }

            return pins;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Keywords] {Keywords.Count}";
        }
    }
}
