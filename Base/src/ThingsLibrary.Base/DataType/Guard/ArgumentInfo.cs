namespace ThingsLibrary.DataType
{
    /// <summary>
    /// An argument.
    /// </summary>
    /// <typeparam name="TType">The type.</typeparam>
    [DebuggerStepThrough]
    public readonly struct ArgumentInfo<TType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentInfo{TType}" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        public ArgumentInfo(TType value, string name)
        {
            Value = value;
            Name = name;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TType Value { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Checks whether the argument has a value or not.
        /// </summary>
        /// <returns>True if the argument has a value; otherwise, false.</returns>
        public bool HasValue()
        {
            return Value != null;
        }
    }
}
