using System.ComponentModel.DataAnnotations;

namespace keepscape_api.Validators
{
    public class NotEqualToAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public NotEqualToAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherProperty = validationContext.ObjectType.GetProperty(_otherProperty);
            var otherPropertyValue = otherProperty?.GetValue(validationContext.ObjectInstance, null);

            if (Equals(value, otherPropertyValue))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
