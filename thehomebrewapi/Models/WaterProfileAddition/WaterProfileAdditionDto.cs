using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class WaterProfileAdditionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}
