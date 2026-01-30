using Cadmus.Core.Config;
using System.Collections.Generic;

namespace Cadmus.Seed;

/// <summary>
/// General seeding options. This is the root in the seed configuration used
/// by <see cref="PartSeederFactory"/>.
/// </summary>
public class SeedOptions
{
    /// <summary>
    /// Gets or sets the optional seed to be used for randomizers.
    /// Set this to get reproducible results.
    /// </summary>
    public int? Seed { get; set; }

    /// <summary>
    /// Gets or sets the users names to be randomly picked for assigning
    /// users to seeded data.
    /// </summary>
    public IList<string>? Users { get; set; }

    /// <summary>
    /// Gets or sets the optional part roles to pick from.
    /// </summary>
    /// <remarks>
    /// <para>When set, parts of the specified types can be assigned one of
    /// these roles. Each entry is a composite value in the format
    /// <c>seed.{partTypeId}:{roleId}</c>, where:</para>
    /// <list type="bullet">
    /// <item><description><c>partTypeId</c> is the part type ID (e.g.,
    /// <c>it.vedph.categories</c>).</description></item>
    /// <item><description><c>roleId</c> is the role ID to assign (e.g.,
    /// <c>function</c>).</description></item>
    /// </list>
    /// <para>For example: <c>seed.it.vedph.categories:function</c> means
    /// that parts of type <c>it.vedph.categories</c> can be assigned the
    /// role <c>function</c>.</para>
    /// <para>This format also allows having distinct seeding options for
    /// the same part type with different roles. In the part seeders
    /// configuration, you can specify seeders with IDs like
    /// <c>seed.it.vedph.categories:function</c> and
    /// <c>seed.it.vedph.categories:material</c>, each with their own
    /// options.</para>
    /// </remarks>
    public IList<string>? PartRoles { get; set; }

    /// <summary>
    /// Gets or sets the optional fragment roles to pick from.
    /// </summary>
    /// <remarks>
    /// <para>When set, fragments of the specified types can be assigned one
    /// of these roles. Each entry is a composite value in the format
    /// <c>seed.{fragmentTypeId}:{roleId}</c>, where:</para>
    /// <list type="bullet">
    /// <item><description><c>fragmentTypeId</c> is the fragment type ID
    /// (e.g., <c>fr.it.vedph.comment</c>).</description></item>
    /// <item><description><c>roleId</c> is the role ID to assign (e.g.,
    /// <c>scholarly</c>).</description></item>
    /// </list>
    /// <para>For example: <c>seed.fr.it.vedph.comment:scholarly</c> means
    /// that fragments of type <c>fr.it.vedph.comment</c> can be assigned
    /// the role <c>scholarly</c>.</para>
    /// <para>This format also allows having distinct seeding options for
    /// the same fragment type with different roles. In the fragment seeders
    /// configuration, you can specify seeders with IDs like
    /// <c>seed.fr.it.vedph.comment:scholarly</c> and
    /// <c>seed.fr.it.vedph.comment:general</c>, each with their own
    /// options.</para>
    /// </remarks>
    public IList<string>? FragmentRoles { get; set; }

    /// <summary>
    /// Gets or sets the facets definitions to be used. Each facet defines
    /// the set of parts which can be present in an item.
    /// </summary>
    public IList<FacetDefinition>? FacetDefinitions { get; set; }
}
