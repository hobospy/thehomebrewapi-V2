using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class WaterProfileForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<WaterProfileAdditionForCreationDto> Additions { get; set; } = new List<WaterProfileAdditionForCreationDto>();
    }
}
