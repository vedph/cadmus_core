using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Core.Config;

/// <summary>
/// Data profile. This includes metadata about facets and flags definitions
/// and thesauri.
/// </summary>
public class DataProfile
{
    /// <summary>
    /// Gets the facets.
    /// </summary>
    public IList<FacetDefinition>? Facets { get; set; }

    /// <summary>
    /// Gets the flags definitions.
    /// </summary>
    public IList<FlagDefinition>? Flags { get; set; }

    /// <summary>
    /// Gets the thesauri.
    /// </summary>
    public IList<Thesaurus>? Thesauri { get; set; }

    /// <summary>
    /// Validate this profile.
    /// </summary>
    /// <remarks>
    /// These conditions are checked:
    /// <list type="bullet">
    /// <item>
    /// <description>there must be at least 1 facet definition;</description>
    /// </item>
    /// <item>
    /// <description>there must be at least 1 part definition inside
    /// each facet definition;</description>
    /// </item>
    /// <item>
    /// <description>if a facet definition contains a part definition
    /// referring to a layer (=with a role ID starting with
    /// <see cref="PartBase.FR_PREFIX"/>), the same facet must have a part
    /// with role ID = <see cref="PartBase.BASE_TEXT_ROLE_ID"/>, representing
    /// its base text;</description>
    /// </item>
    /// <item>
    /// <description>in each facet, there cannot be more than 1 part
    /// definition with role ID = <see cref="PartBase.BASE_TEXT_ROLE_ID"/>.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// An error message if the profile is not valid; else, null.
    /// </returns>
    public string? Validate()
    {
        // at least 1 facet
        if (Facets == null || Facets.Count == 0)
            return Properties.Resources.NoFacetInProfile;

        // at least 1 part per facet
        FacetDefinition? emptyFacet = Facets.FirstOrDefault(
            f => f.PartDefinitions.Count == 0);
        if (emptyFacet != null)
        {
            return LocalizedStrings.Format(
                Properties.Resources.NoPartInFacet,
                emptyFacet.Id);
        }

        // layers imply a single base text part
        foreach (FacetDefinition facet in Facets)
        {
            // any layers?
            if (facet.PartDefinitions.Any(def => def.RoleId?.StartsWith(
                PartBase.FR_PREFIX, StringComparison.Ordinal) == true))
            {
                // there must be a base text
                int count = facet.PartDefinitions.Count(
                    def => def.RoleId == PartBase.BASE_TEXT_ROLE_ID);
                if (count == 0)
                {
                    return LocalizedStrings.Format(
                        Properties.Resources.LayerPartsWithoutBaseText,
                        facet.Id);
                }
                if (count > 1)
                {
                    return LocalizedStrings.Format(
                        Properties.Resources.MultipleBaseTexts,
                        facet.Id);
                }
            }
        }

        return null;
    }
}
