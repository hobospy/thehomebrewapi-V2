using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace thehomebrewapi.Entities
{
    public class RecipeStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public Timer Timer { get; set; }

        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
    }
}
