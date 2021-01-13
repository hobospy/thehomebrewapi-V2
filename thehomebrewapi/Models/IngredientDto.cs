using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Models
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
