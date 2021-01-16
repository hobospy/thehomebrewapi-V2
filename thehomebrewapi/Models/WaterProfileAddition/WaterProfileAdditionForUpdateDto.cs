using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class WaterProfileAdditionForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public EUnitOfMeasure Unit { get; set; }
    }
}
