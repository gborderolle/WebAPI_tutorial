using Azure.Core;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_tutorial.Validations
{
    public class FirstCharCapitalValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var firstChar = value.ToString()[0].ToString();
            if (firstChar != firstChar.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            return ValidationResult.Success;
        }

    }
}