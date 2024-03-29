﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Entities
{
    public class Brew
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        // Comma separated list of images
        public string Images { get; set; }

        [DataType(DataType.Date)]
        public DateTime BrewDate { get; set; }

        public EBrewedState BrewedState { get; set; } = EBrewedState.notBrewed;

        [MaxLength(2000)]
        public string BrewingNotes { get; set; }

        public IList<TastingNote> TastingNotes { get; set; }

        [Required]
        public double ABV { get; set; }

        public double Rating { get; set; } = 0.0;

        [ForeignKey("RecipeId")]
        public Recipe Recipe { get; set; }
        public int RecipeId { get; set; }
    }
}
