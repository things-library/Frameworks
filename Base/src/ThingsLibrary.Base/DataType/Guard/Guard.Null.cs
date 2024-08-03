using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ThingsLibrary.DataType
{
    /// <summary>
    /// A guard collection of nullability preconditions.
    /// </summary>
    /// <remarks><see cref="https://github.com/testcontainers/testcontainers-dotnet/blob/b121ddebe1597f94f73e307a079bedd3d6762053/src/Testcontainers/Guard.Null.cs"/></remarks>
    public static partial class Guard
    {
        /// <summary>
        /// Ensures the argument value is null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <typeparam name="TType">The type.</typeparam>
        /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
        /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>        
        public static ref readonly ArgumentInfo<TType> Null<TType>(in this ArgumentInfo<TType> argument, string? exceptionMessage = null) where TType : class
        {
            if (!argument.HasValue())
            {
                return ref argument;
            }

            const string message = "'{0}' must be null.";
            throw new ArgumentException(exceptionMessage ?? string.Format(CultureInfo.InvariantCulture, message, argument.Name), argument.Name);
        }

        /// <summary>
        /// Ensures the argument value is not null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <typeparam name="TType">The type.</typeparam>
        /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
        /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>
        [MemberNotNull(nameof(argument.Value))]
        public static ref readonly ArgumentInfo<TType> NotNull<TType>(in this ArgumentInfo<TType> argument, string? exceptionMessage = null) where TType : class
        {
            if (argument.HasValue())
            {
                return ref argument;
            }

            const string message = "'{0}' cannot be null.";
            throw new ArgumentException(exceptionMessage ?? string.Format(CultureInfo.InvariantCulture, message, argument.Name), argument.Name);
        }

        /// <summary>
        /// Ensures the argument value not pass the predicate.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="condition">The condition that raises the exception.</param>
        /// <param name="ifClause">The function to invoke to create the exception object.</param>
        /// <typeparam name="TType">The type.</typeparam>
        /// <returns>An instance of the <see cref="ArgumentInfo{TType}" /> struct.</returns>
        /// <exception cref="ArgumentException">Thrown when the condition is not met.</exception>
        public static ArgumentInfo<TType> ThrowIf<TType>(in this ArgumentInfo<TType> argument, Func<ArgumentInfo<TType>, bool> condition, Func<ArgumentInfo<TType>, Exception> ifClause)
        {
            return condition(argument) ? throw ifClause(argument) : argument;
        }
    }
}

