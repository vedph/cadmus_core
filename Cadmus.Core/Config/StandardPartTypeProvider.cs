using System;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// Standard part type provider, simply wrapping a <see cref="TagAttributeToTypeMap"/>.
    /// </summary>
    /// <seealso cref="IPartTypeProvider" />
    public sealed class StandardPartTypeProvider : IPartTypeProvider
    {
        private readonly TagAttributeToTypeMap _map;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardPartTypeProvider"/>
        /// class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <exception cref="ArgumentNullException">map</exception>
        public StandardPartTypeProvider(TagAttributeToTypeMap map)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        /// <summary>
        /// Gets the type from its identifier.
        /// </summary>
        /// <param name="id">The part or fragment identifier.</param>
        /// <returns>
        /// part/fragment type, or null if not found
        /// </returns>
        public Type? Get(string id)
        {
            return _map.Get(id);
        }
    }
}
