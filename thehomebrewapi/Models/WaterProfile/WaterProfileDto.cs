using System.Collections.Generic;

namespace thehomebrewapi.Models
{
    public class WaterProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<WaterProfileAdditionDto> Additions { get; set; } = new List<WaterProfileAdditionDto>();
    }
}
