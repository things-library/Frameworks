using System.Reflection;

namespace ThingsLibrary.IO
{
    public static class Embedded
    {
        /// <summary>
        /// Check to see if the embedded resource exists
        /// </summary>
        /// <param name="resourceName">Resource Name</param>
        /// <param name="assembly">Assembly the embedded resource is located in (if null, calling assembly will be used)</param>
        /// <returns>If embedded resource exists</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Exists(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName)) { throw new ArgumentNullException(nameof(resourceName)); }

            // assign if missing
            assembly ??= Assembly.GetCallingAssembly();

            var info = assembly.GetManifestResourceInfo(resourceName);

            info ??= assembly.GetManifestResourceInfo($"{assembly.GetName().Name}.{resourceName}");

            return (info != null);
        }

        /// <summary>
        /// Get the embedded resource as a stream
        /// </summary>
        /// <param name="resourceName">Resource Name</param>
        /// <param name="assembly">Assembly the embedded resource is located in (if null, calling assembly will be used)</param>
        /// <returns><see cref="Stream"/></returns>
        /// <exception cref="ArgumentNullException">If resource name is null</exception>
        public static Stream GetStream(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName)) { throw new ArgumentNullException(nameof(resourceName)); }

            // assign if missing
            assembly ??= Assembly.GetCallingAssembly();

            var stream = assembly.GetManifestResourceStream(resourceName);

            stream ??= assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}");

            return stream ?? throw new ArgumentException($"Unable to locate stream for resource {resourceName}");
        }

        /// <summary>
        /// Load the embedded resource data as a string
        /// </summary>
        /// <param name="resourceName">Resource Name</param>
        /// <param name="assembly">Assembly the embedded resource is located in (if null, calling assembly will be used)</param>
        /// <returns>Resource Data as string</returns>
        /// <exception cref="ArgumentNullException">If resource name is null</exception>
        public static string LoadAsString(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName)) { throw new ArgumentNullException(nameof(resourceName)); }

            // assign if missing
            assembly ??= Assembly.GetCallingAssembly();

            using var stream = GetStream(resourceName, assembly);
            if (stream == null) { throw new ArgumentException($"Unable to load stream for resource name: '{resourceName}'"); }

            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Get the byte data for the embedded resource
        /// </summary>
        /// <param name="resourceName">Resource Name</param>
        /// <param name="assembly">Assembly the embedded resource is located in (if null, calling assembly will be used)</param>
        /// <returns><see cref="byte[]"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] LoadByteFile(string resourceName, Assembly? assembly = null)
        {
            if (string.IsNullOrEmpty(resourceName)) { throw new ArgumentNullException(nameof(resourceName)); }

            // assign if missing
            assembly ??= Assembly.GetCallingAssembly();

            using var stream = GetStream(resourceName, assembly);
            if (stream == null) { throw new ArgumentException($"Unable to load stream for resource name: '{resourceName}'"); }

            using var reader = new BinaryReader(stream);

            var rawData = new byte[reader.BaseStream.Length];

            for (var pos = 0; pos < rawData.Length; pos++)
            {
                rawData[pos] = reader.ReadByte();     //NEED TO BE TESTED AND VERIFIED THAT THIS IS CORRECT
            }
            reader.Close();

            return rawData;
        }
    }
}
