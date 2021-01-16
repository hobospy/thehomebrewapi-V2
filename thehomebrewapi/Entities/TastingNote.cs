using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace thehomebrewapi.Entities
{
    public class TastingNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Note { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [ForeignKey("BrewID")]
        public Brew Brew { get; set; }
        public int BrewID { get; set; }

    }
}
