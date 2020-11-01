using System;

namespace Cadmus.Index
{
    /// <summary>
    /// Cadmus index query exception.
    /// </summary>
    /// <seealso cref="Exception" />
    public class CadmusQueryException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CadmusQueryException()
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public CadmusQueryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">error message</param>
        /// <param name="innerException">inner exception</param>
        public CadmusQueryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
