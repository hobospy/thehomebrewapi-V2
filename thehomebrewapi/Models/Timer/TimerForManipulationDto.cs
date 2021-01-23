using System.ComponentModel.DataAnnotations;
using thehomebrewapi.ValidationAttributes;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public abstract class TimerForManipulationDto
    {
        [Required(ErrorMessage = "You need to supply a duration for the timer.")]
        public long Duration { get; set; }

        [Required(ErrorMessage = "You need to supply a type for the timer.")]
        [DurationTypeIsValidEnumAttribute(ErrorMessage = "A valid duration type must be supplied.")]
        public ETypeOfDuration Type { get; set; }
    }
}
