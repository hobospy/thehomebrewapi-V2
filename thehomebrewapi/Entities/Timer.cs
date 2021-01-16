using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Entities
{
    public class Timer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public long Duration { get; set; }

        [Required]
        public ETypeOfDuration Type { get; set; }

        [ForeignKey("RecipeStepId")]
        public RecipeStep RecipeStep { get; set; }
        public int RecipeStepId { get; set; }
    }
}
