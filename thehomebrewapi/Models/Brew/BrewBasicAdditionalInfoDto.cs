using System;
using System.Collections.Generic;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class BrewBasicAdditionalInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BrewDate { get; set; }
        public int BrewedState { get; set; }
        public string BrewingNotes { get; set; }
        public double ABV { get; set; }
        public double Rating { get; set; } = 0.0;
        public string RecipeDescription { get; set; }
        public string RecipeType { get; set; }
    }
}