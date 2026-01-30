using Bogus;
using Cadmus.Core;
using System;
using System.Collections.Generic;

namespace Cadmus.Seed;

/// <summary>
/// Base class for part seeders. A part seeder creates and seeds a Cadmus
/// part with mock data.
/// </summary>
/// <seealso cref="IPartSeeder" />
public abstract class PartSeederBase : IPartSeeder
{
    // assign a role in 1 case out of 10
    private const int ASSIGN_ROLE_MAX = 10;

    /// <summary>
    /// Gets the options.
    /// </summary>
    protected SeedOptions? Options { get; private set; }

    /// <summary>
    /// Set the general options for seeding.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <exception cref="ArgumentNullException">options</exception>
    public void SetSeedOptions(SeedOptions options)
    {
        Options = options ??
            throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Tries to randomly assign a fragment role to a layer part.
    /// </summary>
    /// <param name="part">The layer part.</param>
    /// <param name="roleId">The current role ID (e.g., fr.it.vedph.comment).
    /// </param>
    /// <returns>True if a role was assigned, false otherwise.</returns>
    private bool TryAssignFragmentRole(IPart part, string roleId)
    {
        if (Options?.FragmentRoles == null || Options.FragmentRoles.Count == 0)
            return false;

        // roleId format: fr.{fragmentTypeId} or fr.{fragmentTypeId}:{role}
        // we only assign if no role is present yet
        if (roleId.Contains(':')) return false;

        // random chance to assign a role
        if (Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) != 1) return false;

        // fragment type ID is the whole roleId (e.g., fr.it.vedph.comment)
        string fragmentTypeId = roleId;

        // find applicable roles for this fragment type
        IList<string> roles = SeederTypeRoleId.GetRolesForType(
            Options.FragmentRoles, fragmentTypeId);

        if (roles.Count == 0) return false;

        // pick a random role and append it
        string? pickedRole = SeedHelper.RandomPickOneOf(roles);
        if (!string.IsNullOrEmpty(pickedRole))
        {
            part.RoleId = roleId + ":" + pickedRole;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to randomly assign a part role to a regular part.
    /// </summary>
    /// <param name="part">The part.</param>
    /// <returns>True if a role was assigned, false otherwise.</returns>
    private bool TryAssignPartRole(IPart part)
    {
        if (Options?.PartRoles == null || Options.PartRoles.Count == 0)
            return false;

        // random chance to assign a role
        if (Randomizer.Seed.Next(0, ASSIGN_ROLE_MAX) != 1) return false;

        // find applicable roles for this part type
        IList<string> roles = SeederTypeRoleId.GetRolesForType(
            Options.PartRoles, part.TypeId);

        if (roles.Count == 0) return false;

        // pick a random role
        string? pickedRole = SeedHelper.RandomPickOneOf(roles);
        if (!string.IsNullOrEmpty(pickedRole))
        {
            part.RoleId = pickedRole;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the part metadata: item ID, creator and user ID, role ID.
    /// </summary>
    /// <remarks>
    /// <para>This method may optionally assign a role to the part if:</para>
    /// <list type="bullet">
    /// <item><description>For layer parts: the role ID does not already
    /// contain a fragment role, and <see cref="SeedOptions.FragmentRoles"/>
    /// contains entries matching the fragment type ID.</description></item>
    /// <item><description>For regular parts: the role ID is empty, and
    /// <see cref="SeedOptions.PartRoles"/> contains entries matching the
    /// part's type ID.</description></item>
    /// </list>
    /// <para>The role assignment is random, occurring in approximately
    /// 1 out of 10 cases.</para>
    /// </remarks>
    /// <param name="part">The part.</param>
    /// <param name="roleId">The part's role ID or null.</param>
    /// <param name="item">The item.</param>
    /// <exception cref="ArgumentNullException">part or item</exception>
    protected void SetPartMetadata(IPart part, string? roleId, IItem item)
    {
        ArgumentNullException.ThrowIfNull(part);
        ArgumentNullException.ThrowIfNull(item);

        part.ItemId = item.Id;
        part.CreatorId = item.CreatorId;
        part.UserId = item.UserId;
        part.RoleId = string.IsNullOrEmpty(roleId) ? null : roleId;
        // (no thesaurus scope)

        // optionally assign a role based on type
        if (roleId?.StartsWith(PartBase.FR_PREFIX, StringComparison.Ordinal)
            == true)
        {
            // layer part: try to assign a fragment role
            TryAssignFragmentRole(part, roleId);
        }
        else if (string.IsNullOrEmpty(roleId))
        {
            // regular part without role: try to assign a part role
            TryAssignPartRole(part);
        }
    }

    /// <summary>
    /// Creates and seeds a new part.
    /// </summary>
    /// <param name="item">The item this part should belong to.</param>
    /// <param name="roleId">The optional part role ID.</param>
    /// <param name="factory">The optional part seeder factory. This is used
    /// for layer parts, which need to seed a set of fragments.</param>
    /// <returns>A new part, or null.</returns>
    public abstract IPart? GetPart(IItem item, string? roleId,
        PartSeederFactory? factory);
}
