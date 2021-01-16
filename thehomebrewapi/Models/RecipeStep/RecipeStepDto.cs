using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Models
{
    public class RecipeStepDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public TimerDto Timer { get; set; }
        // TODO: TEMP - For example purposes only
        public int NumberOfIngredients
        {
            get
            {
                return Ingredients.Count;
            }
        }

        public ICollection<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
    }
}
