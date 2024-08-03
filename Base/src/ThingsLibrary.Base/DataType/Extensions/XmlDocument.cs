using System.Xml;

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class XmlDocumentExtensions
    {
        /// <summary>
        /// Get property from string xpath
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="xmlDocument"><see cref="XmlDocument"/></param>
        /// <param name="propertyPath">Property Path</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Property cast as return type if found, default value if not found </returns>
        public static T GetProperty<T>(this XmlDocument xmlDocument, string propertyPath, T defaultValue)
        {
            if(xmlDocument.DocumentElement == null) { return defaultValue; }

            return xmlDocument.DocumentElement.GetProperty<T>(propertyPath, defaultValue);
        }
    }
}
