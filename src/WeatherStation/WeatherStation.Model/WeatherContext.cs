using Microsoft.EntityFrameworkCore;

namespace WeatherStation.Model
{
    public class WeatherContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Connection String
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=WeatherStation;User Id=postgres;Password=123456;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Weather>().ToTable("weather");
        }
    }
}
