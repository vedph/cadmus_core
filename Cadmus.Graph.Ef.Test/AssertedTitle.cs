using Cadmus.Refs.Bricks;

namespace Cadmus.Graph.Ef.Test;

/// <summary>
/// A title with an assertion.
/// </summary>
/// <seealso cref="IHasAssertion" />
public class AssertedTitle : IHasAssertion
{
    /// <summary>
    /// Gets or sets the title's language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the title's value.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets the assertion.
    /// </summary>
    public Assertion? Assertion { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"[{Language}] {Value}";
    }
}
