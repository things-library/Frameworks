using System.Collections;

namespace ThingsLibrary.Attributes
{
    public class ValidateObjectAttribute : ValidationAttribute
    {        
        protected override ValidationResult IsValid(object? instance, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(validationContext.MemberName);

            if (instance == null)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                if (propertyInfo != null)
                {
                    var isNullable = propertyInfo.CustomAttributes.Any(x => x.AttributeType.Name == "NullableAttribute");
                    if (isNullable)
                    {
                        return ValidationResult.Success!;
                    }
                    else
                    {
                        return new ValidationResult($"The {validationContext.MemberName} field is required.", new List<string> { validationContext.MemberName });
                    }
                }
                else
                {
                    // if we have no instance, and can't find a property for the member then assume success?
                    return ValidationResult.Success!;
                }
            }
            else if (instance is IDictionary || instance is IList)
            {
                throw new ValidationException($"Unable to validate collection '{validationContext.ObjectType.Name}.{validationContext.DisplayName}'.");
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(instance, null, null);

            Validator.TryValidateObject(instance, context, results, true);

            if (results.Count != 0)
            {
                var compositeResult = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!", [ validationContext.MemberName ]);
                results.ForEach(compositeResult.AddResult);

                return compositeResult;
            }

            return ValidationResult.Success!;
        }
    }

    public class ValidateObjectAttribute<T> : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? instance, ValidationContext validationContext)
        {            
            var results = new List<ValidationResult>();

            if (instance == null)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                if (propertyInfo != null)
                {
                    var isNullable = propertyInfo.CustomAttributes.Any(x => x.AttributeType.Name == "NullableAttribute");
                    if (isNullable)
                    {
                        return ValidationResult.Success!;
                    }
                    else
                    {
                        return new ValidationResult($"The {validationContext.MemberName} field is required.", new List<string> { validationContext.MemberName });
                    }
                }
                else
                {
                    // assume success if there is no instance and can't find the member property?
                    return ValidationResult.Success!;
                }

            }            
            else if (instance is IDictionary<string, T> dictionary)
            {
                foreach (var keyPair in dictionary)
                {
                    var subResults = new List<ValidationResult>();
                    var context = new ValidationContext(keyPair.Value, null, null);

                    Validator.TryValidateObject(keyPair.Value, context, subResults, true);

                    if (subResults.Any())
                    {
                        var compositeResult = new CompositeValidationResult($"Validation for {validationContext.DisplayName}[{keyPair.Key}] failed!", new List<string> { $"{validationContext.MemberName}[{keyPair.Key}]" });
                        subResults.ForEach(compositeResult.AddResult);

                        results.Add(compositeResult);
                    }
                }
            }
            else if (instance is IList<T> list)
            {
                int i = 0;
                foreach (var item in list)
                {
                    var subResults = new List<ValidationResult>();
                    var context = new ValidationContext(item, null, null);

                    Validator.TryValidateObject(item, context, subResults, true);

                    if (subResults.Any())
                    {
                        var compositeResult = new CompositeValidationResult($"Validation for {validationContext.DisplayName}[{i}] failed!", new List<string> { $"{validationContext.MemberName}[{i}]" });
                        subResults.ForEach(compositeResult.AddResult);

                        results.Add(compositeResult);
                    }
                    i++;
                }
            }
            else if (instance is IDictionary || instance is IList)
            {
                throw new ValidationException("Unable to validate complex collection.");
            }
            else
            {
                var subResults = new List<ValidationResult>();
                var context = new ValidationContext(instance, null, null);

                Validator.TryValidateObject(instance, context, results, true);

                if (subResults.Any())
                {
                    var compositeResult = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!", new List<string> { validationContext.MemberName });
                    subResults.ForEach(compositeResult.AddResult);

                    results.Add(compositeResult);
                }
            }

            if (results.Any())
            {
                var compositeResults = new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!", new List<string> { validationContext.MemberName });
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }


    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        public IEnumerable<ValidationResult> Results
        {
            get
            {
                return _results;
            }
        }

        public CompositeValidationResult(string errorMessage) : base(errorMessage) 
        {
            //nothing
        }

        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) 
        {
            //nothing
        }

        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) 
        {
            //nothing
        }

        public void AddResult(ValidationResult validationResult)
        {
            _results.Add(validationResult);
        }
    }
}
