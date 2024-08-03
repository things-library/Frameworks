using System.Reflection;

namespace ThingsLibrary
{
    public static class Reflection
    {
        /// <summary>        
        /// Retrieves the default value for a given Type
        /// </summary>
        /// <typeparam name="T">The Type for which to get the default value</typeparam>
        /// <returns>The default value for Type T</returns>
        /// <remarks>
        /// If a reference Type or a System.Void Type is supplied, this method always returns null.  If a value type is supplied which is not publicly visible or which contains generic parameters, this method will fail with an exception.
        /// </remarks>
        /// <seealso cref="GetDefault(Type)"/>
        public static T GetDefault<T>()
        {
            var defaultValue = GetDefault(typeof(T));
            if(defaultValue == null) { return default(T); }

            return (T)defaultValue;
        }

        /// <summary>        
        /// Retrieves the default value for a given Type
        /// </summary>
        /// <param name="type">The Type for which to get the default value</param>
        /// <returns>The default value for <paramref name="type"/></returns>
        /// <remarks>
        /// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type is supplied which is not publicly visible or which contains generic parameters, this method will fail with an exception.
        /// </remarks>
        /// <seealso cref="GetDefault&lt;T&gt;"/>
        public static object? GetDefault(Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
            if (type == null || !type.IsValueType || type == typeof(void))
            {
                return null;
            }

            // If the supplied Type has generic parameters, its default value cannot be determined
            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException($"{{{MethodInfo.GetCurrentMethod()}}} Error:\n\nThe supplied value type <{type}> contains generic parameters, so the default value cannot be retrieved");
            }

            // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
            //  default instance of the value type
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"{{{MethodInfo.GetCurrentMethod()}}} Error:\n\nThe Activator.CreateInstance method could not create a default instance of the supplied value type <{type}> (Inner Exception message: \"{e.Message}\")", e);
                }
            }

            // Fail with exception
            throw new ArgumentException($"{{{MethodInfo.GetCurrentMethod()}}} Error:\n\nThe supplied value type <{type}> is not a publicly-visible type, so the default value cannot be retrieved");
        }

        /// <summary>
        /// Gets the properties that are set to the default value
        /// </summary>        
        /// <param name="obj">Object to get default properties from</param>
        /// <param name="bindingFlags">Binding flags</param>
        /// <returns>List of <see cref="PropertyInfo"/> that are set to default(T)</returns>
        public static List<PropertyInfo> GetDefaultProperties(object obj, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var list = new List<PropertyInfo>();

            var type = obj.GetType();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                // get the default value for the reflected property
                var defaultValue = Reflection.GetDefault(property.PropertyType);

                var propValue = property.GetValue(obj);
                if (Nullable.Equals(propValue, defaultValue))
                {                    
                    list.Add(property);
                }
            }

            return list;
        }
    }
}
