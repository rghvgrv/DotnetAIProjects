using Microsoft.EntityFrameworkCore;
using PromptExample.Models;

namespace PromptExample.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City { Id = 1, Name = "Mumbai" },
                new City { Id = 2, Name = "Delhi" },
                new City { Id = 3, Name = "Pune" }
                );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, Name = "Rahul", Age = 21, CityId = 1 },
                new Student { Id = 2, Name = "Priya", Age = 22, CityId = 2 },
                new Student { Id = 3, Name = "Amit", Age = 20, CityId = 1 }
            );
        }
    }
}
