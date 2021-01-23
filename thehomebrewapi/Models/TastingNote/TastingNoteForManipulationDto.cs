using System;
using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;

namespace thehomebrewapi.Models
{
    public abstract class TastingNoteForManipulationDto
    {
        [Required]
        [MaxLength(1000)]
        public string Note { get; set; }
        [Required]
        [TastingNoteDateIsValidAttribute(ErrorMessage = "A valid date has to be supplied with the tasting note.") ]
        public DateTime Date { get; set; }
    }
}
