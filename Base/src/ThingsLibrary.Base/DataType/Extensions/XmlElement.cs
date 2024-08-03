using System.Xml;

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class XmlElementExtensions
    {
        /// <summary>
        /// Get property from string xpath
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="xmlElement"><see cref="XmlElement"/></param>
        /// <param name="propertyPath">Property Path</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Property cast as return type if found, default value if not found </returns>
        public static T GetProperty<T>(this XmlElement xmlElement, string propertyPath, T defaultValue)
        {
            if (xmlElement == null) { throw new ArgumentNullException(nameof(xmlElement)); }
            if (string.IsNullOrEmpty(propertyPath)) { throw new ArgumentNullException(nameof(propertyPath)); }

            var xElement = xmlElement.SelectSingleNode(propertyPath);
            if (xElement != null)
            {
                return (T)Convert.ChangeType(xElement.InnerText, typeof(T));
            }

            // try finding the element as a attribute
            var elementNames = propertyPath.Split('/');
            elementNames[elementNames.Length - 1] = $"@{elementNames[elementNames.Length - 1]}";

            propertyPath = string.Join('/', elementNames);
            var xNode = xmlElement.SelectSingleNode(propertyPath);
            if (xNode == null) { return defaultValue; }

            return (T)Convert.ChangeType(xNode.Value, typeof(T));
        }
    }
}
