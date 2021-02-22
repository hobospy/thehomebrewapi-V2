using System.Collections.Generic;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public ETypeOfBeer Type { get; set; }
        public string Type { get; set; }
        public double ExpectedABV { get; set; }
        public bool Favourite { get; set; }
        public WaterProfileDto WaterProfile { get; set; }
        // TODO: TEMP - For example purposes only
        public int NumberOfSteps
        {
            get
            {
                return Steps.Count;
            }
        }

        public ICollection<RecipeStepDto> Steps { get; set; } = new List<RecipeStepDto>();
    }
}
