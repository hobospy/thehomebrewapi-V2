using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class RecipeWithoutStepsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ETypeOfBeer Type { get; set; }
        public double ExpectedABV { get; set; }
        public bool Favourite { get; set; }
        public WaterProfileDto WaterProfile { get; set; }
    }
}
