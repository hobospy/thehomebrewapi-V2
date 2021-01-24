using System;

namespace thehomebrewapi.Models
{
    public class BrewWithoutAdditionalInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BrewDate { get; set; }
        public int BrewedState { get; set; }
        public string BrewingNotes { get; set; }
        public double ABV { get; set; }
        public double Rating { get; set; } = 0.0;
    }
}
