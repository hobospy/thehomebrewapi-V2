using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class WaterProfileForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public ICollection<WaterProfileAdditionForUpdateDto> Additions { get; set; } = new List<WaterProfileAdditionForUpdateDto>();

    }
}
