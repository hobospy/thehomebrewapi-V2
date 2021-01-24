using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class TimerDto
    {
        public int Id { get; set; }

        public long Duration { get; set; }

        public ETypeOfDuration Type { get; set; }
    }
}
