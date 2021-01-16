using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class RecipeForUpdateDto
    {
        [Required(ErrorMessage = "You need to supply a name for the recipe.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public ETypeOfBeer Type { get; set; }
        public double ExpectedABV { get; set; }
        public bool Favourite { get; set; }
        public int WaterProfileId { get; set; }

        public ICollection<RecipeStepForUpdateDto> Steps { get; set; } = new List<RecipeStepForUpdateDto>();
    }
}
