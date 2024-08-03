namespace ThingsLibrary.Testing.Extensions
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Asserts that all properties of the class are non-default
        /// </summary>
        /// <param name="classObj"></param>
        public static void AssertNonDefaultProperties(this object classObj)
        {
            var defaultProperties = Reflection.GetDefaultProperties(classObj);

            Assert.AreEqual(0, defaultProperties.Count, $"Error Non-Default Properties: {string.Join("; ", defaultProperties.Select(x => x.Name))}");
        }
    }
}
