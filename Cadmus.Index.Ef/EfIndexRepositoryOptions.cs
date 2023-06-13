namespace Cadmus.Index.Ef;

/// <summary>
/// Options for Entity Framework-based index readers/writers.
/// </summary>
public class EfIndexRepositoryOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfIndexRepositoryOptions"/>
    /// class.
    /// </summary>
    public EfIndexRepositoryOptions()
    {
        ConnectionString = "";
    }
}
