namespace Cadmus.Core;

/// <summary>
/// Interface implemented by objects having a 32-bits <see cref="Flags"/>
/// property.
/// </summary>
public interface IHasFlags
{
    /// <summary>
    /// Gets or sets the flags.
    /// </summary>
    int Flags { get; set; }
}
