using Microsoft.EntityFrameworkCore;
using thehomebrewapi.Entities;

namespace thehomebrewapi.Contexts
{
    public class HomeBrewContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        public HomeBrewContext(DbContextOptions<HomeBrewContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>()
                .HasData(
                new Recipe()
                {
                    Id = 1,
                    Name = "Amarillo SMaSH",
                    Description = "Single malt and single hop recipe using Amarillo hops",
                },
                new Recipe()
                {
                    Id = 2,
                    Name = "Raspberry Porter",
                    Description = "Porter based recipe with raspberries added during the second half of fermentation",
                },
                new Recipe()
                {
                    Id = 3,
                    Name = "Bock",
                    Description = "Basic Bock recipe for those cold nights",
                });

            modelBuilder.Entity<Ingredient>()
                .HasData(
                    new Ingredient()
                    {
                        Id = 1,
                        RecipeId = 1,
                        Name = "Amarillo",
                        Amount = 68
                    },
                    new Ingredient()
                    {
                        Id = 2,
                        RecipeId = 1,
                        Name = "Pale malt",
                        Amount = 5.5
                    },
                    new Ingredient()
                    {
                        Id = 3,
                        RecipeId = 1,
                        Name = "Light crystal malt",
                        Amount = 150
                    },
                    new Ingredient()
                    {
                        Id = 4,
                        RecipeId = 2,
                        Name = "Dark roast malt",
                        Amount = 300
                    },
                    new Ingredient()
                    {
                        Id = 5,
                        RecipeId = 2,
                        Name = "Bittering",
                        Amount = 60
                    },
                    new Ingredient()
                    {
                        Id = 6,
                        RecipeId = 3,
                        Name = "Dark crystal",
                        Amount = 120
                    },
                    new Ingredient()
                    {
                        Id = 7,
                        RecipeId = 3,
                        Name = "Rice husks",
                        Amount = 300
                    },
                    new Ingredient()
                    {
                        Id = 8,
                        RecipeId = 3,
                        Name = "Whirlfloc",
                        Amount = 0.5
                    });

            base.OnModelCreating(modelBuilder);
        }
    }
}
