using System.Globalization;

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// Reduce code redundancy and clarity for parameter validation checks
    /// </summary>
    /// <remarks><see cref="https://github.com/testcontainers/testcontainers-dotnet/blob/b121ddebe1597f94f73e307a079bedd3d6762053/src/Testcontainers/Guard.Argument.cs"/></remarks>
    [DebuggerStepThrough]    
    public static partial class Guard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentInfo{TType}" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        /// <typeparam name="TType">The type.</typeparam>
        /// <returns>A new instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
        public static ArgumentInfo<TType> Argument<TType>(TType value, string name)
        {
            return new ArgumentInfo<TType>(value, name);
        }
    }
}

