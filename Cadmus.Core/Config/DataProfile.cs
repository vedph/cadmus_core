using System;
using System.Linq;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// Data profile. This includes metadata about facets and flags definitions
    /// and thesauri.
    /// </summary>
    public class DataProfile
    {
        /// <summary>
        /// Gets the facets.
        /// </summary>
        public FacetDefinition[] Facets { get; set; }

        /// <summary>
        /// Gets the flags definitions.
        /// </summary>
        public FlagDefinition[] Flags { get; set; }

        /// <summary>
        /// Gets the thesauri.
        /// </summary>
        public Thesaurus[] Thesauri { get; set; }

        private bool HasAnyPart(Func<PartDefinition, bool> condition) =>
            Facets?.Length > 0 && Facets.Any(f => f.PartDefinitions.Any(d => condition(d)));

        /// <summary>
        /// Returns true if this profile is valid.
        /// </summary>
        /// <remarks>
        /// To be valid, a data profile must:
        /// <list type="bullet">
        /// <item>
        /// <description>have at least 1 facet definition;</description>
        /// </item>
        /// <item>
        /// <description>if any definition refers to a layer, have a part with
        /// role ID = <see cref="PartBase.BASE_TEXT_ROLE_ID"/>;</description>
        /// </item>
        /// <item>
        /// <description>have no more than 1 part definition with role ID =
        /// <see cref="PartBase.BASE_TEXT_ROLE_ID"/>.</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// An error message if the profile is not valid; else, null.
        /// </returns>
        public string IsValid()
        {
            if (Facets == null || Facets.Length == 0) return null;

            // any layers?
            if (HasAnyPart(def => def.RoleId?.StartsWith(
                    PartBase.FR_PREFIX, StringComparison.Ordinal) == true))
            {
                // there must be a base text
                if (!HasAnyPart(def => def.RoleId == PartBase.BASE_TEXT_ROLE_ID))
                    return Properties.Resources.LayerPartsWithoutBaseText;
            }

            // single base text
            int n = 0;
            foreach (FacetDefinition facet in Facets)
            {
                n += facet.PartDefinitions.Count(
                    d => d.RoleId == PartBase.BASE_TEXT_ROLE_ID);
                if (n > 1)
                {
                    return Properties.Resources.MultipleBaseTexts;
                }
            }
            // TODO:
            throw new NotImplementedException();
        }
    }
}
