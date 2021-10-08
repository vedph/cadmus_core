using Fusi.Tools.Config;
using McMaster.NETCore.Plugins;
using System;
using System.IO;
using System.Linq;

// https://github.com/natemcmaster/DotNetCorePlugins

namespace Cadmus.Cli.Core
{
    /// <summary>
    /// Plugins-based provider for Cadmus factory providers. This is used to
    /// load a factory provider from an external plugin.
    /// </summary>
    public static class PluginFactoryProvider
    {
        /// <summary>
        /// Gets the plugins directory.
        /// </summary>
        /// <returns>Directory.</returns>
        public static string GetPluginsDir() =>
            Path.Combine(AppContext.BaseDirectory, "plugins");

        /// <summary>
        /// Scans all the plugins in the plugins folder and returns the first
        /// plugin matching the requested tag.
        /// </summary>
        /// <param name="tag">The requested plugin tag.</param>
        /// <returns>The provider, or null if not found.</returns>
        public static T GetFromTag<T>(string tag) where T : class
        {
            // create plugin loaders
            string pluginsDir = GetPluginsDir();
            foreach (string dir in Directory.GetDirectories(pluginsDir))
            {
                string dirName = Path.GetFileName(dir);
                string pluginDll = Path.Combine(dir, dirName + ".dll");

                T provider = Get<T>(pluginDll, tag);
                if (provider != null) return provider;
            }

            return null;
        }

        /// <summary>
        /// Gets the provider plugin from the specified directory.
        /// </summary>
        /// <param name="path">The path to the plugin file.</param>
        /// <param name="tag">The optional provider tag. If null, the first
        /// matching plugin in the target assembly will be returned. This can
        /// be used when an assembly just contains a single plugin implementation.
        /// </param>
        /// <returns>Provider, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">path</exception>
        public static T Get<T>(string path, string tag = null) where T : class
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path)) return null;

            PluginLoader loader = PluginLoader.CreateFromAssemblyFile(
                    path,
                    sharedTypes: new[] { typeof(T) });

            foreach (Type type in loader.LoadDefaultAssembly()
                .GetExportedTypes()
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
            {
                if (tag == null)
                    return (T)Activator.CreateInstance(type);

                TagAttribute tagAttr = (TagAttribute)Attribute.GetCustomAttribute(
                    type, typeof(TagAttribute));
                if (tagAttr?.Tag == tag)
                    return (T)Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
