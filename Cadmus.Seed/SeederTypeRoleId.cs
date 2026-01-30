using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cadmus.Seed;

/// <summary>
/// A composite identifier representing a part or fragment type ID with an
/// optional role ID. This is used in seeder configurations to allow
/// different seeding options for the same part/fragment type with different
/// roles.
/// </summary>
/// <remarks>
/// <para>The format is <c>typeId</c> or <c>typeId:roleId</c>, where:</para>
/// <list type="bullet">
/// <item><description><c>typeId</c> is the part or fragment type ID
/// (e.g., <c>it.vedph.categories</c> or <c>fr.it.vedph.comment</c>).
/// </description></item>
/// <item><description><c>roleId</c> is the optional role ID
/// (e.g., <c>function</c> or <c>scholarly</c>).</description></item>
/// </list>
/// <para>For seeder configuration entries, the format includes the
/// <c>seed.</c> prefix (e.g., <c>seed.it.vedph.categories:function</c>),
/// which is stripped when parsing.</para>
/// </remarks>
public readonly struct SeederTypeRoleId : IEquatable<SeederTypeRoleId>
{
    private const string SEED_PREFIX = "seed.";

    /// <summary>
    /// Gets the type ID portion.
    /// </summary>
    public string TypeId { get; }

    /// <summary>
    /// Gets the optional role ID portion.
    /// </summary>
    public string? RoleId { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has a role ID.
    /// </summary>
    public bool HasRole => !string.IsNullOrEmpty(RoleId);

    /// <summary>
    /// Initializes a new instance of the <see cref="SeederTypeRoleId"/>
    /// struct.
    /// </summary>
    /// <param name="typeId">The type ID.</param>
    /// <param name="roleId">The optional role ID.</param>
    /// <exception cref="ArgumentNullException">typeId is null.</exception>
    public SeederTypeRoleId(string typeId, string? roleId = null)
    {
        ArgumentNullException.ThrowIfNull(typeId);
        TypeId = typeId;
        RoleId = string.IsNullOrEmpty(roleId) ? null : roleId;
    }

    /// <summary>
    /// Parses the specified composite ID string into a
    /// <see cref="SeederTypeRoleId"/>.
    /// </summary>
    /// <param name="id">The composite ID string, which may include the
    /// <c>seed.</c> prefix and/or a <c>:roleId</c> suffix.</param>
    /// <returns>The parsed instance.</returns>
    /// <exception cref="ArgumentNullException">id is null.</exception>
    public static SeederTypeRoleId Parse(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        ReadOnlySpan<char> span = id.AsSpan();

        // strip seed. prefix if present
        if (span.StartsWith(SEED_PREFIX.AsSpan(), StringComparison.Ordinal))
            span = span[SEED_PREFIX.Length..];

        // find the colon separator for role
        int colonIndex = span.IndexOf(':');

        if (colonIndex == -1)
        {
            return new SeederTypeRoleId(span.ToString());
        }

        string typeId = span[..colonIndex].ToString();
        string roleId = span[(colonIndex + 1)..].ToString();

        return new SeederTypeRoleId(typeId, roleId);
    }

    /// <summary>
    /// Tries to parse the specified composite ID string.
    /// </summary>
    /// <param name="id">The composite ID string.</param>
    /// <param name="result">The parsed result if successful.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(
        string? id,
        [NotNullWhen(true)] out SeederTypeRoleId? result)
    {
        result = null;
        if (string.IsNullOrEmpty(id)) return false;

        try
        {
            result = Parse(id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Builds a composite key from a type ID and optional role ID.
    /// </summary>
    /// <param name="typeId">The type ID.</param>
    /// <param name="roleId">The optional role ID.</param>
    /// <returns>The composite key (e.g., <c>it.vedph.categories:function</c>).
    /// </returns>
    public static string BuildKey(string typeId, string? roleId)
    {
        return string.IsNullOrEmpty(roleId)
            ? typeId
            : $"{typeId}:{roleId}";
    }

    /// <summary>
    /// Gets the composite key representation of this instance.
    /// </summary>
    /// <returns>The key (e.g., <c>it.vedph.categories:function</c>).</returns>
    public string ToKey() => BuildKey(TypeId, RoleId);

    /// <summary>
    /// Filters a list of composite IDs to find roles matching a specific
    /// type ID.
    /// </summary>
    /// <param name="compositeIds">The list of composite IDs to filter.</param>
    /// <param name="typeId">The type ID to match.</param>
    /// <returns>A list of role IDs for the matching type.</returns>
    public static IList<string> GetRolesForType(
        IList<string>? compositeIds,
        string typeId)
    {
        List<string> roles = [];

        if (compositeIds == null || compositeIds.Count == 0)
            return roles;

        foreach (string id in compositeIds)
        {
            if (TryParse(id, out SeederTypeRoleId? parsed) &&
                parsed.Value.HasRole &&
                string.Equals(parsed.Value.TypeId, typeId,
                    StringComparison.Ordinal))
            {
                roles.Add(parsed.Value.RoleId!);
            }
        }

        return roles;
    }

    /// <summary>
    /// Extracts the fragment role from a layer part's role ID.
    /// </summary>
    /// <remarks>
    /// <para>Layer parts have role IDs in the format
    /// <c>fr.{fragmentTypeId}</c> or <c>fr.{fragmentTypeId}:{fragmentRole}</c>.
    /// This method extracts the fragment role portion after the colon.</para>
    /// </remarks>
    /// <param name="layerPartRoleId">The layer part's role ID.</param>
    /// <returns>The fragment role, or null if none is present.</returns>
    public static string? ExtractFragmentRole(string? layerPartRoleId)
    {
        if (string.IsNullOrEmpty(layerPartRoleId)) return null;

        // layer part roleId format: fr.{fragmentTypeId} or
        // fr.{fragmentTypeId}:{fragmentRole}
        int colonIndex = layerPartRoleId.IndexOf(':');
        if (colonIndex == -1 || colonIndex == layerPartRoleId.Length - 1)
            return null;

        return layerPartRoleId[(colonIndex + 1)..];
    }

    /// <summary>
    /// Extracts the fragment type ID from a layer part's role ID.
    /// </summary>
    /// <remarks>
    /// <para>Layer parts have role IDs in the format
    /// <c>fr.{fragmentTypeId}</c> or <c>fr.{fragmentTypeId}:{fragmentRole}</c>.
    /// This method extracts the fragment type ID portion (including the
    /// <c>fr.</c> prefix).</para>
    /// </remarks>
    /// <param name="layerPartRoleId">The layer part's role ID.</param>
    /// <returns>The fragment type ID (e.g., <c>fr.it.vedph.comment</c>),
    /// or null if the input is null/empty.</returns>
    public static string? ExtractFragmentTypeId(string? layerPartRoleId)
    {
        if (string.IsNullOrEmpty(layerPartRoleId)) return null;

        int colonIndex = layerPartRoleId.IndexOf(':');
        return colonIndex == -1
            ? layerPartRoleId
            : layerPartRoleId[..colonIndex];
    }

    /// <inheritdoc/>
    public bool Equals(SeederTypeRoleId other)
    {
        return string.Equals(TypeId, other.TypeId, StringComparison.Ordinal)
            && string.Equals(RoleId, other.RoleId, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is SeederTypeRoleId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            TypeId?.GetHashCode(StringComparison.Ordinal) ?? 0,
            RoleId?.GetHashCode(StringComparison.Ordinal) ?? 0);
    }

    /// <inheritdoc/>
    public override string ToString() => ToKey();

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(SeederTypeRoleId left, SeederTypeRoleId right)
        => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(SeederTypeRoleId left, SeederTypeRoleId right)
        => !left.Equals(right);
}
