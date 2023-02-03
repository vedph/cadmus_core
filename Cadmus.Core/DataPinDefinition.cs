using System;

namespace Cadmus.Core;

/// <summary>
/// The definition of a <see cref="DataPin"/>, used to provide more
/// information to clients when building index queries.
/// </summary>
public class DataPinDefinition
{
    /// <summary>
    /// Gets or sets the pin's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets a tip about this pin and its usage.
    /// </summary>
    public string? Tip { get; set; }

    /// <summary>
    /// Gets or sets a set of attributes, each represented by a single
    /// letter. Usual attributes are <c>M</c>=multivalued pin (=the component
    /// can emit several pins with the same name, for different values),
    /// <c>F</c>=filtered text value (without digits), <c>f</c>=filtered
    /// text value (with digits).
    /// </summary>
    public string? Attributes { get; set; }

    /// <summary>
    /// Gets or sets the type of this pin's value. All pin values are
    /// stored as strings, but their actual value is defined here.
    /// </summary>
    public DataPinValueType Type { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPinDefinition"/> class.
    /// </summary>
    public DataPinDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPinDefinition"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The name.</param>
    /// <param name="tip">The tip.</param>
    /// <param name="attributes">The optional attributes.</param>
    /// <exception cref="ArgumentNullException">name</exception>
    public DataPinDefinition(DataPinValueType type, string name, string tip,
        string? attributes = null)
    {
        Type = type;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Tip = tip;
        Attributes = attributes;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"[{Enum.GetName(typeof(DataPinValueType), Type)}] {Name}: {Tip}"
            + $" [{Attributes}]";
    }
}

/// <summary>
/// The type of value for a <see cref="DataPin"/> as found in the
/// <see cref="DataPinDefinition"/>.
/// </summary>
public enum DataPinValueType
{
    /// <summary>String.</summary>
    String = 0,
    /// <summary>Boolean: either <c>1</c> or <c>0</c>.</summary>
    Boolean,
    /// <summary>Integer number.</summary>
    Integer,
    /// <summary>Decimal number.</summary>
    Decimal
}
