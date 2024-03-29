﻿using System;
using System.Collections.Generic;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Models
{
    public class BrewFullAdditionalInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Images { get; set; }
        public DateTime BrewDate { get; set; }
        public EBrewedState BrewedState { get; set; }
        public string BrewingNotes { get; set; }
        public double ABV { get; set; }
        public double Rating { get; set; } = 0.0;
        public ICollection<TastingNoteDto> TastingNotes { get; set; } = new List<TastingNoteDto>();
        public RecipeDto Recipe { get; set; }
    }
}