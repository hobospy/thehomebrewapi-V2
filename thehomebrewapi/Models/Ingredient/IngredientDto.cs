using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public EUnitOfMeasure Unit { get; set; }
    }
}
