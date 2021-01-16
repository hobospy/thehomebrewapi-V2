using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class RecipeStepForUpdateDto
    {
        [Required(ErrorMessage = "You need to supply a description for the recipe step.")]
        [MaxLength(500)]
        public string Description { get; set; }
        public TimerForCreationDto Timer { get; set; }
        public ICollection<IngredientForUpdateDto> Ingredients { get; set; } = new List<IngredientForUpdateDto>();
    }
}
