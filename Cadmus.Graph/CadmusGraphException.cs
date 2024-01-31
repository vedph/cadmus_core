using System;
using System.Runtime.Serialization;

namespace Cadmus.Graph;

/// <summary>
/// An exception specific to Cadmus graph handling.
/// </summary>
[Serializable]
public class CadmusGraphException : Exception
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CadmusGraphException()
    {
    }

    /// <summary>
    /// Create a new exception with the specified error message.
    /// </summary>
    /// <param name="message">error message</param>
    public CadmusGraphException(string message) : base(message)
    {
    }

    /// <summary>
    /// Create a new exception with the specified error message and inner
    /// exception.
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="innerException">inner exception</param>
    public CadmusGraphException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
