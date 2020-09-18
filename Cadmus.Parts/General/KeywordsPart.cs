using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Keywords part.
    /// Tag: <c>it.vedph.keywords</c>.
    /// </summary>
    /// <remarks>This part contains any number of <see cref="Keyword"/>'s,
    /// each with its own language and value.
    /// </remarks>
    [Tag("it.vedph.keywords")]
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
        /// Get all the key=value pairs exposed by the implementor.
        /// </summary>
        /// <param name="item">The optional item. The item with its parts
        /// can optionally be passed to this method for those parts requiring
        /// to access further data.</param>
        /// <returns>The pins: <c>tot-count</c> and a list of keywords, with
        /// form <c>keyword.LANG</c> where <c>LANG</c> is its language value,
        /// e.g. <c>keyword.eng</c> as name and <c>sample</c> as value.
        /// The pins are returned sorted by language and then by value.</returns>
        public override IEnumerable<DataPin> GetDataPins(IItem item = null)
        {
            if (Keywords == null || Keywords.Count == 0)
                return Enumerable.Empty<DataPin>();

            List<DataPin> pins = new List<DataPin>
            {
                CreateDataPin("tot-count",
                    (Keywords?.Count ?? 0).ToString(CultureInfo.InvariantCulture))
            };

            IDataPinTextFilter filter = new StandardDataPinTextFilter();

            var keysByLang = from k in Keywords
                             group k by k.Language
                                 into g
                             orderby g.Key
                             select g;

            foreach (var g in keysByLang)
            {
                var values = from k in g
                             orderby k.Value
                             select filter.Apply(k.Value, true);

                pins.AddRange(from value in values
                              select CreateDataPin($"keyword.{g.Key}", value));
            }

            return pins;
        }

        /// <summary>
        /// Gets the definitions of data pins used by the implementor.
        /// </summary>
        /// <returns>Data pins definitions.</returns>
        public override IList<DataPinDefinition> GetDataPinDefinitions()
        {
            return new List<DataPinDefinition>(new[]
            {
                new DataPinDefinition(DataPinValueType.Integer,
                    "tot-count",
                    "The total count of keywords."),
                new DataPinDefinition(DataPinValueType.Integer,
                    "keyword.{LANG}",
                    "The list of keywords grouped by language.",
                    "Mf"),
            });
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[Keywords] {Keywords.Count}";
        }
    }
}
