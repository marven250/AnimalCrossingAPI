using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using AnimalCrossingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalCrossingAPI.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<AnimalCrossingDbContext>();

            // Check if any fish data exists
            if (context.Fish.Any())
            {
                return; // DB has been seeded
            }

            // Read JSON file
            var jsonData = File.ReadAllText("Data/fish.json");

            // Deserialize JSON to C# objects
            var fishData = JsonSerializer.Deserialize<Dictionary<string, Fish>>(jsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Add data to database
            if (fishData != null)
            {
                foreach (var fish in fishData.Values)
                {
                    if (fish.Availability != null)
                    {
                        fish.Availability.Fish = fish;
                        context.Add(fish.Availability);  // Ensure Availability is added
                    }
                    context.Add(fish); // Add Fish to the context
                }

                context.SaveChanges(); // Save all changes to the database
            }

            // Get the current max Id
            var maxId = context.Fish.Max(f => f.Id);

            // Set the sequence to start from maxId + 1
            context.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('\"Fish\"', 'Id'), {maxId + 1})");

        }
    }
}
