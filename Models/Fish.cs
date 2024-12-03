using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AnimalCrossingAPI.Models
{

    public class Fish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("file-name")]
        public string FileName { get; set; }
        [Required]
        public Availability Availability { get; set; }

        [Required]
        public string Shadow { get; set; }
        [Required]
        public int Price { get; set; }


        public string? CreatedBy { get; set; }

        [Required]
        [JsonPropertyName("catch-phrase")]
        public string CatchPhrase { get; set; }

        [Required]
        [JsonPropertyName("museum-phrase")]
        public string MuseumPhrase { get; set; }
    }

    public class Availability
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Fish")]
        public int FishId { get; set; }

        public bool IsAllDay { get; set; }
        public bool IsAllYear { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Rarity { get; set; }
        public Fish Fish { get; set; }
    }

}
