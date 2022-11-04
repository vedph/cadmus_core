using System;
using System.Runtime.Serialization;

namespace Cadmus.Index
{
    /// <summary>
    /// Cadmus index query exception.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CadmusQueryException"/> class.
        /// </summary>
        /// <param name="info">The info that holds the serialized object data
        /// about the exception being thrown.</param>
        /// <param name="context">The cibtext that contains contextual information
        /// about the source or destination.</param>
        protected CadmusQueryException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
