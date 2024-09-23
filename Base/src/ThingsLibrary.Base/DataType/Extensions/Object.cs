// ================================================================================
// <copyright file="Object.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static class ObjectExtensions
    {
        public static T ConvertTo<T>(this object value)
        {
            if (value is T variable) { return variable; }

            //Handling Nullable types i.e, int?, double?, bool? .. etc
            if (Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                
                return (T)converter.ConvertFrom(value);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
