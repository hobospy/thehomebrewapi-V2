using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public abstract class IngredientForManipulationDto
    {
        [Required(ErrorMessage = "You need to supply a name for the ingredient.")]
        [MaxLength(50, ErrorMessage = "The name shouldn't have more than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You need to supply an ingredient amount.")]
        [AmountValueMustBeGreaterThanZeroAttribute(ErrorMessage = "The amount value must be greater than zero.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "You need to supply a unit of measure for the ingredient.")]
        public EUnitOfMeasure Unit { get; set; }
    }
}
