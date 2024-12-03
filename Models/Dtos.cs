namespace AnimalCrossingAPI.Models
{

    public class FishDto
    {
        public FishDto(Fish fish)
        {
            Id = fish.Id;
            FileName = fish.FileName;
            Shadow = fish.Shadow;
            Price = fish.Price;
            CatchPhrase = fish.CatchPhrase;
            CreatedBy = fish.CreatedBy;
            MuseumPhrase = fish.MuseumPhrase;
            Availability = fish.Availability == null ? null : new AvailabilityDto(fish.Availability);
        }

        public int Id { get; set; }
        public string FileName { get; set; }
        public string Shadow { get; set; }
        public int Price { get; set; }
        public string CatchPhrase { get; set; }
        public string MuseumPhrase { get; set; }

        public string CreatedBy { get; set; } = null;

        public AvailabilityDto? Availability { get; set; }
    }

    public class AvailabilityDto
    {
        public AvailabilityDto(Availability availability)
        {
            IsAllDay = availability.IsAllDay;
            IsAllYear = availability.IsAllYear;
            Location = availability.Location;
            Rarity = availability.Rarity;
        }

        public bool IsAllDay { get; set; }
        public bool IsAllYear { get; set; }
        public string Location { get; set; }
        public string Rarity { get; set; }
    }

    public class CreateFishDto
    {
        public required string FileName { get; set; }
        public required string Shadow { get; set; }
        public int Price { get; set; }
        public required string CatchPhrase { get; set; }
        public required string MuseumPhrase { get; set; }
        public required CreateAvailabilityDto Availability { get; set; }
    }

    public class CreateAvailabilityDto
    {
        public bool IsAllDay { get; set; }
        public bool IsAllYear { get; set; }
        public required string Location { get; set; }
        public required string Rarity { get; set; }
    }

    public class FishAvailabilityUpdateDto
    {
        public bool? IsAllDay { get; set; }
        public bool? IsAllYear { get; set; }
        public string? Location { get; set; }
        public string? Rarity { get; set; }
    }

    public class FishUpdateDto
    {
        public string? FileName { get; set; }
        public string? Shadow { get; set; }
        public int? Price { get; set; }
        public string? CatchPhrase { get; set; }
        public string? MuseumPhrase { get; set; }
    }

}
