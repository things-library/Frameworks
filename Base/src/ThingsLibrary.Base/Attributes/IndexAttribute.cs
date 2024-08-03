namespace ThingsLibrary.Attributes
{
    /// <summary>
    /// Specifies what indexes should exist for the class
    /// </summary>
    /// <remarks>This is modeled after the IndexAttribute of Microsoft.EntityFrameworkCore</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class IndexAttribute : Attribute
    {
        /// <summary>
        /// The properties which constitute the index, in order.
        /// </summary>
        public IReadOnlyList<string> PropertyNames { get; }

        
        /// <summary>
        /// The name of the index.
        /// </summary>    
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException(nameof(Name)); }

                _name = value;
            }
        }
        private string _name = string.Empty;


        /// <summary>
        /// Whether the index is unique.
        /// </summary>
        public bool IsUnique
        {
            get => _isUnique ?? false;
            set => _isUnique = value;
        }
        private bool? _isUnique;


        /// <summary>
        /// A set of values indicating whether each corresponding index column has descending sort order.
        /// </summary>
        public bool[] IsDescending
        {
            get
            {
                // we must always return a matching set to property names
                if (_isDescending?.Length == this.PropertyNames.Count)
                {
                    return _isDescending;
                }
                else
                {
                    return new bool[this.PropertyNames.Count];
                }
            } 
            set
            {
                if (value is not null)
                {
                    if (value.Length != this.PropertyNames.Count)
                    {
                        throw new ArgumentException($"Invalid number of items for IsDescending([]), expected '{this.PropertyNames.Count}' got '{value.Length}'");
                    }

                    if (_allDescending)
                    {
                        throw new ArgumentException("Cannot specify both IsDescending And AllDescending");
                    }

                    _isDescending = value ?? new bool[this.PropertyNames.Count];
                }
                else
                {
                    _isDescending = null;
                }                
            }
        }
        private bool[]? _isDescending;


        /// <summary>
        /// Whether all index columns have descending sort order.
        /// </summary>
        public bool AllDescending
        {
            get => _allDescending;
            set
            {
                if (this.IsDescending is not null)
                {
                    throw new ArgumentException("Cannot specify both IsDescending and AllDescending.");
                }

                _allDescending = value;
            }
        }
        private bool _allDescending;


        /// <summary>
        /// Checks whether <see cref="IsUnique" /> has been explicitly set to a value.
        /// </summary>
        public bool IsUniqueHasValue => _isUnique.HasValue;


        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttribute" /> class.
        /// </summary>
        /// <param name="propertyName">The first (or only) property in the index.</param>
        /// <param name="additionalPropertyNames">The additional properties which constitute the index, if any, in order.</param>
        public IndexAttribute(string propertyName, params string[] additionalPropertyNames)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));
            if (additionalPropertyNames.Any(x => string.IsNullOrEmpty(x))) { throw new ArgumentNullException(nameof(additionalPropertyNames)); }

            this.PropertyNames = new List<string> { propertyName };
            ((List<string>)this.PropertyNames).AddRange(additionalPropertyNames);
        }

        /// <summary>
        /// Index Attribute
        /// </summary>
        public IndexAttribute(params string[] propertyNames)
        {
            if (propertyNames.Any(x => string.IsNullOrEmpty(x))) { throw new ArgumentNullException(nameof(propertyNames)); }

            this.PropertyNames = propertyNames.ToList();
        }
    }
}