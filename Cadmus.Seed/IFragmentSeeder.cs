﻿using Cadmus.Core;
using Cadmus.Core.Layers;

namespace Cadmus.Seed
{
    /// <summary>
    /// Cadmus fragment seeder. This is used to seed a fragment with
    /// random data. Each implementor should get injected in its constructor
    /// an instance of <see cref="IPartTypeProvider"/>, which allows it
    /// to instantiate the part, and get a <see cref="TagAttribute"/> with
    /// value equal to <c>seed.</c> plus the part type ID.
    /// </summary>
    public interface IFragmentSeeder
    {
        /// <summary>
        /// Configures this seeder with the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        void Configure(SeedOptions options);

        /// <summary>
        /// Creates and seeds a new part.
        /// </summary>
        /// <param name="item">The item this part should belong to.</param>
        /// <param name="location">The location.</param>
        /// <param name="baseText">The base text.</param>
        /// <returns>A new part.</returns>
        ITextLayerFragment GetFragment(IItem item,
            string location, string baseText);
    }
}
