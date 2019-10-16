using Fusi.Tools.Config;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cadmus.Core.Layers;

namespace Cadmus.Core.Config
{
    /// <summary>
    /// A set of mappings between attributes of type <see cref="TagAttribute"/>
    /// and the types they decorate, built from a set of preloaded assemblies,
    /// and/or by loading assemblies from a specified directory.
    /// </summary>
    /// <remarks>For those classes implementing <see cref="ITextLayerFragment"/>,
    /// and of course decorated with the <see cref="TagAttribute"/>, a mapping
    /// is added by combining the generic layer part with
    /// each of these fragment classes. This way, we have a mapping for all
    /// the closed generic types representing the various text layers. In this
    /// case, the tag is prefixed with the tag from the layer part followed by
    /// <c>:fr.</c>, e.g. <c>token-text-layer:fr.comment</c> for a fragment
    /// with tag <c>comment</c> combined with a layer part with tag
    /// <c>token-text-layer</c>.</remarks>
    public sealed class TagAttributeToTypeMap
    {
        private readonly Dictionary<string, Type> _map;
        private readonly Dictionary<string, Type> _frMap;
        private readonly TypeInfo _frTypeInfo;
        private bool _frDirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagAttributeToTypeMap"/> class.
        /// </summary>
        public TagAttributeToTypeMap()
        {
            _map = new Dictionary<string, Type>();
            _frMap = new Dictionary<string, Type>();
            _frTypeInfo = typeof(ITextLayerFragment).GetTypeInfo();
        }

        /// <summary>
        /// Clears this map.
        /// </summary>
        public void Clear()
        {
            _map.Clear();
            _frMap.Clear();
        }

        private void SupplyFragments()
        {
            // first collect all the exported text layer part fragments types
            Dictionary<string, Type> dctFragmentTypes =
                new Dictionary<string, Type>();
            foreach (var p in _map)
            {
                if (_frTypeInfo.IsAssignableFrom(p.Value))
                    dctFragmentTypes[p.Key] = p.Value;
            }

            // then, for each mapping, if it's a layer part (assuming that
            // each layer part name ends with the suffix "LayerPart<>" where
            // the type argument is the fragment type) we must add a mapping
            // for each fragment type against this generic part
            foreach (string tag in _map.Keys.ToList())
            {
                Type type = _map[tag];
                if (type.Name.EndsWith("LayerPart`1",
                    StringComparison.OrdinalIgnoreCase))
                {
                    TagAttribute attr = type.GetTypeInfo()
                        .GetCustomAttributes<TagAttribute>()
                        .FirstOrDefault();

                    foreach (Type fragmentType in dctFragmentTypes.Values)
                    {
                        TagAttribute attrFr = fragmentType.GetTypeInfo()
                            .GetCustomAttributes<TagAttribute>()
                            .FirstOrDefault();
                        if (attrFr != null)
                        {
                            _frMap[$"{attr.Tag}:{attrFr.Tag}"] =
                                type.MakeGenericType(fragmentType);
                        }
                    }
                }
            }
            _frDirty = false;
        }

        private void ScanAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.ExportedTypes)
            {
                string tag = t.GetTypeInfo()
                    .GetCustomAttribute<TagAttribute>()?.Tag;
                if (tag != null) _map[tag] = t;
            }
        }

        /// <summary>
        /// Adds to the map all the types decorated with <see cref="TagAttribute"/>
        /// found in the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <exception cref="ArgumentNullException">assemblies</exception>
        public void Add(Assembly[] assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            foreach (Assembly assembly in assemblies) ScanAssembly(assembly);
            _frDirty = true;
        }

        /// <summary>
        /// Adds to the map all the types decorated with <see cref="TagAttribute"/>
        /// found in the assemblies inside the files in the specified directory,
        /// matching <paramref name="fileMask"/>, and additionally in all the
        /// explicitly received <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="fileMask">The file mask.</param>
        /// <param name="assemblies">The optional additional assemblies
        /// to be scanned.</param>
        /// <returns>map</returns>
        /// <exception cref="ArgumentNullException">directory</exception>
        public void Add(string directory,
            string fileMask = "*.dll",
            params Assembly[] assemblies)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(fileMask))
            {
                Assembly assembly = Assembly.Load(
                    AssemblyName.GetAssemblyName(file.FullName));
                ScanAssembly(assembly);
            }
            if (assemblies.Length > 0) Add(assemblies);
            else _frDirty = true;
        }

        /// <summary>
        /// Gets the type from the specified <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">The tag value (e.g. <c>note</c> or
        /// <c>token-text-layer:fr.comment</c>.</param>
        /// <returns>type or null</returns>
        /// <exception cref="ArgumentNullException">tag</exception>
        public Type Get(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            if (_frDirty) SupplyFragments();

            if (_map.ContainsKey(tag)) return _map[tag];
            return _frMap.ContainsKey(tag) ? _frMap[tag] : null;
        }
    }
}
