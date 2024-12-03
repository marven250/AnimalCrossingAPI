using AnimalCrossingAPI.Auth;
using AnimalCrossingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AnimalCrossingAPI.Controllers
{
    [Route("api/fish")]
    [ApiController]
    public class FishController : Controller
    {
        private readonly AnimalCrossingDbContext _db;
        private readonly IAuthorizationService _authorizationService;
        public FishController(AnimalCrossingDbContext dbContext, IAuthorizationService authorizationService)
        {
            _db = dbContext;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Retrieves a list of all fish, including their availability information.
        /// </summary>
        /// <returns>A list of fish with detailed information.</returns>
        [HttpGet]
        [Authorize(Policy = "RequireWriteFishScope")]
        public async Task<ActionResult<IEnumerable<FishDto>>> ListFish()
        {
            Console.WriteLine("This is GET All fish");
            
            var fish = await _db.Fish.Include(f => f.Availability).ToListAsync();
            
            var fishDtoList = fish.Select(f => new FishDto(f)).ToList();
         
            return Ok(fishDtoList);
        }

        /// <summary>
        /// Searches for fish by name, supporting partial name matches.
        /// </summary>
        /// <param name="name">The name or partial name of the fish to search for.</param>
        /// <returns>A list of fish that match the search criteria.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FishDto>>> GetFishByName([FromQuery] string name)
        {
            var fishData = _db.Fish.Include(f => f.Availability);

            var fishQuery = fishData.Where(f => f.FileName.Contains(name)); 

            var fish = await fishQuery.ToListAsync();
            var fishDtoList = fish.Select(f => new FishDto(f)).ToList();
            return Ok(fishDtoList);
        }

        /// <summary>
        /// Creates a new fish entry in the database.
        /// </summary>
        /// <param name="createFishDto">The data containing details about the new fish.</param>
        /// <returns>The created fish with its generated identifier.</returns>
        [HttpPost]
        [Authorize(Policy = "RequireWriteFishScope")]
        public async Task<ActionResult<FishDto>> CreateFish([FromBody] CreateFishDto createFishDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            // Map the CreateFishDto to the Fish entity
            var fish = new Fish
            {
                FileName = createFishDto.FileName,
                Shadow = createFishDto.Shadow,
                Price = createFishDto.Price,
                CatchPhrase = createFishDto.CatchPhrase,
                MuseumPhrase = createFishDto.MuseumPhrase,
                CreatedBy = userId
            };

     

            fish.Availability = new Availability
            {
                IsAllDay = createFishDto.Availability.IsAllDay,
                IsAllYear = createFishDto.Availability.IsAllYear,
                Location = createFishDto.Availability.Location,
                Rarity = createFishDto.Availability.Rarity
            };


            // Add the new fish to the database
            _db.Fish.Add(fish);
            await _db.SaveChangesAsync();

            // Map the saved fish to FishDto to return in the response
            var fishDto = new FishDto(fish);

            return CreatedAtAction(nameof(CreateFish), new { id = fish.Id }, fishDto);
        }

        /// <summary>
        /// Updates the availability information of an existing fish.
        /// </summary>
        /// <param name="id">The unique identifier of the fish to update.</param>
        /// <param name="updateDto">The data containing availability information to update.</param>
        /// <returns>A confirmation of successful update or an error if the fish is not found.</returns>
        [HttpPatch("{id}/availability")]
        [Authorize(Policy = "RequireWriteFishScope")]
        public async Task<IActionResult> UpdateFishAvailability(int id, [FromBody] FishAvailabilityUpdateDto updateDto)
        {
            // Find the fish and include the Availability property
            var fish = await _db.Fish.Include(f => f.Availability).FirstOrDefaultAsync(f => f.Id == id);

            if (fish == null)
            {
                return NotFound($"Fish with Id {id} not found.");
            }

            // If no Availability exists, create a new one
            if (fish.Availability == null)
            {
                fish.Availability = new Availability();
            }

            // Apply updates if values are provided in the DTO
            if (updateDto.IsAllDay.HasValue) fish.Availability.IsAllDay = updateDto.IsAllDay.Value;
            if (updateDto.IsAllYear.HasValue) fish.Availability.IsAllYear = updateDto.IsAllYear.Value;
            if (updateDto.Location != null) fish.Availability.Location = updateDto.Location;
            if (updateDto.Rarity != null) fish.Availability.Rarity = updateDto.Rarity;

            // Save changes to the database
            await _db.SaveChangesAsync();

            return NoContent(); // Return 204 No Content on successful update
        }

        /// <summary>
        /// Updates details of an existing fish (excluding availability information).
        /// </summary>
        /// <param name="id">The unique identifier of the fish to update.</param>
        /// <param name="updateDto">The data containing updated fish details.</param>
        /// <returns>A confirmation of successful update or an error if the fish is not found.</returns>
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFish(int id, [FromBody] FishUpdateDto updateDto)
        {
            var fish = await _db.Fish.FirstOrDefaultAsync(f => f.Id == id);

            if (fish == null)
            {
                return NotFound($"Fish with Id {id} not found.");
            }
            Console.WriteLine("This is authorization service " + JsonSerializer.Serialize(_authorizationService));
            // Authorize user
            var authorizationRsult = await _authorizationService.AuthorizeAsync(User, fish, new CreatedByRequirement());

            if (!authorizationRsult.Succeeded)
            {
                return Forbid();
            }


            // Update properties only if they are provided in the DTO
            if (updateDto.FileName != null) fish.FileName = updateDto.FileName;
            if (updateDto.Shadow != null) fish.Shadow = updateDto.Shadow;
            if (updateDto.Price.HasValue) fish.Price = updateDto.Price.Value;
            if (updateDto.CatchPhrase != null) fish.CatchPhrase = updateDto.CatchPhrase;
            if (updateDto.MuseumPhrase != null) fish.MuseumPhrase = updateDto.MuseumPhrase;

            Console.WriteLine("This is fish dto :" + JsonSerializer.Serialize(updateDto));
            

            await _db.SaveChangesAsync();

            Console.WriteLine("This is fish " + JsonSerializer.Serialize(fish));

            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of fish with prices below the specified amount.
        /// </summary>
        /// <param name="price">The maximum price to filter fish by.</param>
        /// <returns>A list of fish where the price is below the specified amount.</returns>
        [HttpGet("below-price")]
        public async Task<ActionResult<IEnumerable<FishDto>>> GetFishBelowPrice([FromQuery] int price)
        {
            var fish = await _db.Fish
                .Include(f => f.Availability)
                .Where(f => f.Price < price)
                .ToListAsync();

            var fishDtoList = fish.Select(f => new FishDto(f)).ToList();
            return Ok(fishDtoList);
        }

        /// <summary>
        /// Retrieves a list of fish based on their availability location.
        /// </summary>
        /// <param name="location">The location to filter fish by.</param>
        /// <returns>A list of fish available in the specified location.</returns>
        [HttpGet("location")]
        public async Task<ActionResult<IEnumerable<FishDto>>> GetFishByLocation([FromQuery] string location)
        {
            var fish = await _db.Fish
                .Include(f => f.Availability)
                .Where(f => f.Availability != null && f.Availability.Location == location)
                .ToListAsync();

            var fishDtoList = fish.Select(f => new FishDto(f)).ToList();
            return Ok(fishDtoList);
        }


    }
}
