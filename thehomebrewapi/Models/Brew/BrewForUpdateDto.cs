using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class BrewForUpdateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime BrewDate { get; set; }
        [Required]
        public int BrewedState { get; set; }
        public string BrewingNotes { get; set; }
        public double ABV { get; set; }
        public double Rating { get; set; } = 0.0;
        public ICollection<TastingNoteDto> TastingNotes { get; set; } = new List<TastingNoteDto>();
    }
}
