using Cadmus.Core.Config;

namespace Cadmus.Core.Storage;

/// <summary>
/// Base options for <see cref="IItemBrowser"/>'s.
/// </summary>
public class ItemBrowserOptions
{
    /// <summary>
    /// Gets or sets the connection string template. This is supplied by the
    /// <see cref="ItemBrowserFactory"/>, unless overridden by this
    /// object's property. It should be a full connection string, having
    /// the <c>{0}</c> placeholder for the database name.
    /// </summary>
    public string? ConnectionString { get; set; }
}
