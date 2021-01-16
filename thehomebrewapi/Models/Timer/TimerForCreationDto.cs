using System.ComponentModel.DataAnnotations;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class TimerForCreationDto
    {
        [Required(ErrorMessage = "You need to supply a duration for the timer.")]
        public long Duration { get; set; }

        [Required(ErrorMessage = "You need to supply a type for the timer.")]
        public ETypeOfDuration Type { get; set; }
    }
}
