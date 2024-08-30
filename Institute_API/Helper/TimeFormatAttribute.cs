using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Institute_API.Helper
{
    public class TimeFormatAttribute : ValidationAttribute
    {
        private static readonly Regex TimeRegex = new Regex(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$", RegexOptions.Compiled);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var stringValue = value as string;
            if (stringValue == null)
            {
                return new ValidationResult("Invalid time format");
            }

            if (TimeRegex.IsMatch(stringValue))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid time format");
        }
    }
}
