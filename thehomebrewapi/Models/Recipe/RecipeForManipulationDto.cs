using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public abstract class RecipeForManipulationDto
    {
        [Required(ErrorMessage = "You need to supply a name for the recipe.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [BeerTypeIsValidEnumAttribute(ErrorMessage = "A valid beer type must be supplied")]
        public ETypeOfBeer Type { get; set; }
        public double ExpectedABV { get; set; }
        public bool Favourite { get; set; }
        public int WaterProfileId { get; set; }

        public ICollection<RecipeStepForCreationDto> Steps { get; set; } = new List<RecipeStepForCreationDto>();

    }
}
