using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public abstract class WaterProfileAdditionForManipulationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [AmountValueMustBeGreaterThanZeroAttribute(ErrorMessage = "The amount value must be greater than zero.")]
        public double Amount { get; set; }
        [Required]
        [UnitValueIsValidEnumAttribute(ErrorMessage = "A valid unit of measure value must be supplied")]
        public EUnitOfMeasure Unit { get; set; }

    }
}
