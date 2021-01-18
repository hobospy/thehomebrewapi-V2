using System;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class TastingNoteForCreationDto
    {
        [Required]
        [MaxLength(1000)]
        public string Note { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
