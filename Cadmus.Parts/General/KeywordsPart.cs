using System.Collections.Generic;
using System.Linq;
using Cadmus.Core;
using Fusi.Tools.Config;

namespace Cadmus.Parts.General
{
    /// <summary>
    /// Keyword part.
    /// </summary>
    /// <remarks>This part contains any number of <see cref="Keyword"/>'s.
    /// <para>Search pins:</para>
    /// <list type="bullet">
    /// <item>
    /// <term>keyword.XXX</term>
    /// <description>: a string containing the keyword for the language
    /// specified by XXX, (ISO-639 3), e.g. <c>keyword.eng</c>.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Tag("keywords")]
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
        /// Get all the key=value pairs exposed by the implementor. Each key is
        /// <c>keyword.{lang}</c> where <c>{lang}</c> is its language value.
        /// </summary>
        /// <returns>pins</returns>
        public override IEnumerable<DataPin> GetDataPins()
        {
            List<DataPin> pins = new List<DataPin>();
            if (Keywords?.Count == 0) return pins;

            var keysByLang = from k in Keywords
                             group k by k.Language
                                 into g
                             orderby g.Key
                             select g;

            foreach (var g in keysByLang)
            {
                string[] keys = (from k in g
                                  orderby k.Value
                                  select k.Value).ToArray();

                pins.AddRange(from s in keys
                               select CreateDataPin($"keyword.{g.Key}", s));
            }

            return pins;
        }

        /// <summary>
        /// Textual representation of this part.
        /// </summary>
        /// <returns>count of keywords</returns>
        public override string ToString()
        {
            return $"{nameof(KeywordsPart)}: {Keywords.Count}";
        }
    }
}
