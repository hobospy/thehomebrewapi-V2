using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Entities
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public ETypeOfBeer Type { get; set; }

        public double ExpectedABV { get; set; }

        public bool Favourite { get; set; } = false;

        public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();

        [ForeignKey("WaterProfileId")]
        public WaterProfile WaterProfile { get; set; }
        public int WaterProfileId { get; set; }
    }
}
