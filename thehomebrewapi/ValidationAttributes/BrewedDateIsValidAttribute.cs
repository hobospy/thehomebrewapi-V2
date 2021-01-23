using System;
using System.ComponentModel.DataAnnotations;
using thehomebrewapi.Models;

namespace thehomebrewapi.ValidationAttributes
{
    public class BrewedDateIsValidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var brew = (BrewForManipulationDto)validationContext.ObjectInstance;

            if (brew.BrewedState != 0)
            {
                var nullDateTime = new DateTime(1, 1, 1, 0, 0, 0);

                if (brew.BrewDate == nullDateTime)
                {
                    return new ValidationResult(ErrorMessage,
                        new[] { nameof(BrewForManipulationDto) });
                }
            }

            return ValidationResult.Success;
        }
    }
}
