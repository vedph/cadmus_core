namespace Cadmus.Index.Sql;

/// <summary>
/// SQL options. These are used by implementations of
/// <see cref="IItemIndexWriter"/> to set their configuration.
/// </summary>
public class SqlOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }
}
