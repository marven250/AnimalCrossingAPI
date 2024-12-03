using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AnimalCrossingAPI.Models
{
    public class AnimalCrossingDbContext : DbContext
    {
        public DbSet<Fish> Fish { get; set; }
        public DbSet<Availability> Availability { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { 
            optionsBuilder.UseNpgsql(connectionString: "Server=localhost;Port=5432;User Id=postgres;Password=password;Database=animalcrossing;Include Error Detail=true;"); 
            base.OnConfiguring(optionsBuilder); 
        }
    }
}
