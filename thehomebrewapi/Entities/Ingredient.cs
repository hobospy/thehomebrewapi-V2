using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Entities
{
    public class Ingredient
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

        [ForeignKey("RecipeStepId")]
        public RecipeStep RecipeStep { get; set; }
        public int RecipeStepId { get; set; }
    }
}
