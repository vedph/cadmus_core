using System;
using System.Linq;
using System.Collections.Generic;

namespace Cadmus.Lexicon.Parts
{
    /// <summary>
    /// A single lineage in a word etymology.
    /// </summary>
    public sealed class Lineage
    {
        /// <summary>
        /// Gets or sets the rank for this lineage.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the source for this lineage (e.g. Chantraine 1972).
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the steps in this lineage.
        /// </summary>
        public List<LineageStep> Steps { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lineage"/> class.
        /// </summary>
        public Lineage()
        {
            Steps = new List<LineageStep>();
        }

        /// <summary>
        /// Returns a <see cref="String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{Rank}]: " + string.Join(" < ", from step in Steps
                                                      select step.ToString());
        }
    }
}
