using Cadmus.Core;
using Fusi.Tools.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cadmus.Parts.General
{
    [Tag("net.fusisoft.index-keywords")]
    public sealed class IndexKeywordsPart : PartBase
    {
        /// <summary>
        /// Keywords.
        /// </summary>
        public List<IndexKeyword> Keywords { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexKeywordsPart"/>
        /// class.
        /// </summary>
        public IndexKeywordsPart()
        {
            Keywords = new List<IndexKeyword>();
        }

        /// <summary>
        /// Adds the specified keyword. If such keywords already exist,
        /// the old ones are replaced by the new keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <exception cref="ArgumentNullException">keyword</exception>
        public void AddKeyword(IndexKeyword keyword)
        {
            if (keyword == null) throw new ArgumentNullException(nameof(keyword));

            Keywords.RemoveAll(k => k.IndexId == keyword.IndexId
                               && k.Language == keyword.Language
                               && k.Value == keyword.Value);
            Keywords.Add(keyword);
        }

        /// <summary>
        /// Get all the key=value pairs exposed by the implementor. Each key is
        /// <c>keyword.{lang}</c> where <c>{lang}</c> is its language value,
        /// e.g. <c>keyword.eng</c> as name and <c>sample</c> as value.
        /// The pins are returned sorted by language and then by value.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            if (Keywords == null || Keywords.Count == 0)
                return Enumerable.Empty<DataPin>();

            List<DataPin> pins = new List<DataPin>();

            foreach (string indexId in Keywords.Select(k => k.IndexId).Distinct())
            {
                var keysByLang = from k in Keywords
                                 where k.IndexId == indexId
                                 group k by k.Language
                                     into g
                                 orderby g.Key
                                 select g;

                foreach (var g in keysByLang)
                {
                    string[] values = (from k in g
                                       orderby k.Value
                                       select k.Value).ToArray();

                    string iid = string.IsNullOrEmpty(indexId) ? "" : "."+ indexId;
                    pins.AddRange(from value in values
                                  select CreateDataPin($"xkeyword.{iid}{g.Key}", value));
                }
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
