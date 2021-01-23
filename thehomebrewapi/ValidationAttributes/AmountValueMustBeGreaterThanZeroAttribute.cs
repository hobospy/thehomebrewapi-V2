using System.ComponentModel.DataAnnotations;
using thehomebrewapi.Models;

namespace thehomebrewapi.ValidationAttributes
{
    public class AmountValueMustBeGreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((double)value <= 0)
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(IngredientForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
