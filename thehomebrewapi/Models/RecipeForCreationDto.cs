using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class RecipeForCreationDto
    {
        [Required(ErrorMessage = "You need to supply a name for the recipe.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<IngredientForCreationDto> Ingredients { get; set; } = new List<IngredientForCreationDto>();
    }
}
