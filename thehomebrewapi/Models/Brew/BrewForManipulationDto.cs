using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;

namespace thehomebrewapi.Models
{
    public abstract class BrewForManipulationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public int RecipeId { get; set; }
        [Required]
        [BrewedStateIsValidEnumAttribute(ErrorMessage = "A valid brewed state value must be supplied.")]
        public int BrewedState { get; set; }
        [BrewedDateIsValidAttribute]
        public DateTime BrewDate { get; set; }
        [MaxLength(2000)]
        public string BrewingNotes { get; set; }
        public double ABV { get; set; }
        [RatingValueIsValidAttribute(ErrorMessage = "The rating value must be between 0 and 5.")]
        public double Rating { get; set; }
        public ICollection<TastingNoteForCreationDto> TastingNotes { get; set; } = new List<TastingNoteForCreationDto>();
    }
}
