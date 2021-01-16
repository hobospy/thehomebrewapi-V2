using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Models
{
    public class IngredientForUpdateDto
    {
        [Required(ErrorMessage = "You need to supply a name for the ingredient.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
