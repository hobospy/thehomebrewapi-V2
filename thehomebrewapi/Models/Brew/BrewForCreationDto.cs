using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace thehomebrewapi.Models
{
    public class BrewForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int RecipeId { get; set; }
        public DateTime BrewDate { get; set; }
        [Required]
        public int BrewedState { get; set; }
        [MaxLength(2000)]
        public string BrewingNotes { get; set; }
        [Required]
        public double ABV { get; set; }
        public double Rating { get; set; }
        public ICollection<TastingNoteForCreationDto> TastingNotes { get; set; } = new List<TastingNoteForCreationDto>();
    }
}
