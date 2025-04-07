using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Application.Helpers
{
    public static class ValidationHelper
    {
        public class ValidationOutcome(bool isValid, Dictionary<string, List<string>> errors)
        {
            public bool IsValid { get; } = isValid;
            public Dictionary<string, List<string>> Errors { get; } = errors;
        }

        public static ValidationOutcome Validate<T>(T model) where T : new()
        {
            List<ValidationResult> validationResults = [];
            Dictionary<string, List<string>> errors = [];

            // Model will be null for invalid JSON format.
            if (model is null)
            {
                model = new();
            }

            ValidationContext validationContext = new(model, null, null);

            // Validate the model.
            bool isValid = Validator.TryValidateObject(
                model,
                validationContext,
                validationResults,
                validateAllProperties: true
            );

            // Generate the errors dictionary.
            foreach (ValidationResult validationResult in validationResults)
            {
                foreach (string memberName in validationResult.MemberNames)
                {
                    if(validationResult.ErrorMessage != null)
                    {
                        if(!errors.ContainsKey(memberName))
                        {
                            errors[memberName] = [];
                        }
                        errors[memberName].Add(validationResult.ErrorMessage);
                    }
                }
            }

            return new ValidationOutcome(isValid, errors);
        }

        public static ModelStateDictionary ToModelStateDictionary(this Dictionary<string, List<string>> errors)
        {
            ModelStateDictionary modelStateDictionary = new();

            foreach(KeyValuePair<string, List<string>> keyValuePair in errors)
            {
                string errorEntity = keyValuePair.Key;
                foreach(string errorMessage in keyValuePair.Value)
                {
                    modelStateDictionary.AddModelError(errorEntity, errorMessage);
                }
            }

            return modelStateDictionary;
        }
    }
}
