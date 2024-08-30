using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Student_API.Helper
{
    public class ValidDateStringAttribute : ValidationAttribute
    {
        private readonly string _dateFormat;

        public ValidDateStringAttribute(string dateFormat)
        {
            _dateFormat = dateFormat;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Date string cannot be null or empty.");
            }

            DateTime parsedDate;
            bool isValid = DateTime.TryParseExact(value.ToString(), _dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            if (!isValid)
            {
                return new ValidationResult($"Invalid date format. Expected format is {_dateFormat}.");
            }

            return ValidationResult.Success;
        }
    }
}
