using System;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class TastingNoteForUpdateDto
    {
        [Required]
        [MaxLength(1000)]
        public string Note { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
