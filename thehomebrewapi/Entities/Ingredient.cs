using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
    }
}
