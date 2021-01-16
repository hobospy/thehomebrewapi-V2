using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Entities
{
    public class WaterProfileAddition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public EUnitOfMeasure Unit { get; set; }

        [ForeignKey("WaterProfileId")]
        public WaterProfile WaterProfile { get; set; }
        public int WaterProfileId { get; set; }
    }
}
