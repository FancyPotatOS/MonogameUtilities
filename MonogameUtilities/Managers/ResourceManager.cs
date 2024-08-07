
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using MonogameUtilities.Util;

namespace MonogameUtilities.Resources
{
    public static class ResourceManager
    {

        private readonly static Dictionary<string, JsonWrapper> _cachedResources;

        static ResourceManager()
        {
            _cachedResources = new();
        }

        /// <summary>
        /// Resets the cached resource values
        /// </summary>
        public static void Unload()
        {
            _cachedResources.Clear();
        }

        /// <summary>
        /// Provides a <see cref="JsonWrapper"/> of the JSON resource at the path specified.<br/>
        /// For example, if you wanted something in an embedded resource at the path <code>"<see cref="MonogameUtilities"/>/entities/monsters.json > hostile.skeleton.health"</code><br/>
        /// then the path would be <code>"<see cref="MonogameUtilities"/>.entities.monsters:hostile.skeleton.health"</code>
        /// </summary>
        /// <param name="jsonResourcePath">The path to get the resource from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws an exception if the provided path is null or empty.</exception>
        /// <exception cref="ArgumentException">Throws an exeception if the path contains more than one ':'</exception>
        public static JsonWrapper GetResource(string jsonResourcePath, Assembly targetAssembly = null)
        {
            if (string.IsNullOrEmpty(jsonResourcePath)) throw new ArgumentNullException(nameof(jsonResourcePath));

            string[] source_path = jsonResourcePath.Split(':');

            if (source_path.Length < 1 || 2 < source_path.Length) throw new ArgumentException("Path contains an unexpected number of ':'.");

            string baseFile = source_path[0];

            JsonWrapper baseResource = GetJsonResource(baseFile, targetAssembly);
            if (1 < source_path.Length)
            {
                string path = source_path[1];

                baseResource = baseResource[path];
            }

            return baseResource;
        }

        private static JsonWrapper GetJsonResource(string resourceName, Assembly targetAssembly = null)
        {
            if (_cachedResources.ContainsKey(resourceName))
            {
                return _cachedResources[resourceName];
            }

            string actualPath = resourceName;
            if (!actualPath.EndsWith(".json"))
            {
                actualPath += ".json";
            }

            Assembly assembly = targetAssembly ?? Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(actualPath);
            using StreamReader reader = new(stream);
            string jsonFile = reader.ReadToEnd(); //Make string equal to full file

            JsonWrapper resource = JsonWrapper.GetJsonWrapper(jsonFile);

            _cachedResources[resourceName] = resource;

            return resource;
        }
    }
}
