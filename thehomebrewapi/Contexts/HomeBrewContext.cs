using Microsoft.EntityFrameworkCore;
using System;
using thehomebrewapi.Entities;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Contexts
{
    public class HomeBrewContext : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Timer> Timers { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<WaterProfile> WaterProfiles { get; set; }
        public DbSet<WaterProfileAddition> WaterProfileAdditions { get; set; }
        public DbSet<Brew> Brews { get; set; }
        public DbSet<TastingNote> TastingNotes { get; set; }

        public HomeBrewContext(DbContextOptions<HomeBrewContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WaterProfile>()
                .HasData(
                    new WaterProfile()
                    {
                        Id = 1,
                        Name = "Pilsner",
                        Description = "Light beer water profile"
                    },
                    new WaterProfile()
                    {
                        Id = 2,
                        Name = "Stout",
                        Description = "Dark beer water profile"
                    }
                );

            modelBuilder.Entity<WaterProfileAddition>()
                .HasData(
                    new WaterProfileAddition()
                    {
                        Id = 1,
                        WaterProfileId = 1,
                        Name = "Light crystal malt",
                        Amount = 300,
                        Unit = EUnitOfMeasure.gram
                    }
                );

            modelBuilder.Entity<Recipe>()
                .HasData(
                new Recipe()
                {
                    Id = 1,
                    Name = "Amarillo SMaSH",
                    Description = "Single malt and single hop recipe using Amarillo hops",
                    Type = ETypeOfBeer.AmberHybridBeer,
                    ExpectedABV = 3.5,
                    Favourite = true,
                    WaterProfileId = 1
                },
                new Recipe()
                {
                    Id = 2,
                    Name = "Raspberry Porter",
                    Description = "Porter based recipe with raspberries added during the second half of fermentation",
                    Type = ETypeOfBeer.AmericanAle,
                    ExpectedABV = 5.2,
                    Favourite = false,
                    WaterProfileId = 2
                },
                new Recipe()
                {
                    Id = 3,
                    Name = "Bock",
                    Description = "Basic Bock recipe for those cold nights",
                    Type = ETypeOfBeer.BelFrAle,
                    ExpectedABV = 6.8,
                    Favourite = false,
                    WaterProfileId = 2
                });

            modelBuilder.Entity<RecipeStep>()
                .HasData(
                    new RecipeStep()
                    {
                        Id = 1,
                        Description = "Get the water up to mash in temp",
                        RecipeId = 1
                    },
                    new RecipeStep()
                    {
                        Id = 2,
                        Description = "Mash in the grains",
                        RecipeId = 1
                    },
                    new RecipeStep()
                    {
                        Id = 3,
                        Description = "Mash out",
                        RecipeId = 1
                    },
                    new RecipeStep()
                    {
                        Id = 4,
                        Description = "Raise temp to a rolling boil",
                        RecipeId = 1
                    },
                    new RecipeStep()
                    {
                        Id = 5,
                        Description = "Add bittering hops",
                        RecipeId = 1
                    }
                );

            modelBuilder.Entity<Timer>()
                .HasData(
                    new Timer()
                    {
                        Id = 1,
                        RecipeStepId = 2,
                        Duration = 3600,
                        Type = ETypeOfDuration.independent
                    },
                    new Timer()
                    {
                        Id = 2,
                        RecipeStepId = 5,
                        Duration = 600,
                        Type = ETypeOfDuration.beforeFlameout
                    }
                );

            modelBuilder.Entity<Ingredient>()
                .HasData(
                    new Ingredient()
                    {
                        Id = 1,
                        RecipeStepId = 2,
                        Name = "Amarillo",
                        Amount = 68,
                        Unit = EUnitOfMeasure.gram
                    },
                    new Ingredient()
                    {
                        Id = 2,
                        RecipeStepId = 2,
                        Name = "Pale malt",
                        Amount = 5.5,
                        Unit = EUnitOfMeasure.kilo
                    },
                    new Ingredient()
                    {
                        Id = 3,
                        RecipeStepId = 2,
                        Name = "Light crystal malt",
                        Amount = 150,
                        Unit = EUnitOfMeasure.gram
                    },
                    new Ingredient()
                    {
                        Id = 4,
                        RecipeStepId = 5,
                        Name = "Amarillo",
                        Amount = 27.6,
                        Unit = EUnitOfMeasure.gram
                    });

            modelBuilder.Entity<Brew>()
                .HasData(
                    new Brew()
                    {
                        Id = 1,
                        RecipeId = 1,
                        ABV = 4.3,
                        BrewDate = new DateTime(2020, 11, 14),
                        BrewingNotes = "The yeast in this one didn't settle",
                        BrewedState = EBrewedState.brewed,
                        Name = "First Brew",
                        Rating = 3
                    },
                    new Brew()
                    {
                        Id = 2,
                        RecipeId = 1,
                        ABV = 4.1,
                        BrewDate = new DateTime(2021, 1, 19),
                        BrewedState = EBrewedState.notBrewed,
                        Name = "Gonna be better brew"
                    }
                );

            modelBuilder.Entity<TastingNote>()
                .HasData(
                    new TastingNote()
                    {
                        Id = 1,
                        Date = new DateTime(2021, 1, 1),
                        Note = "Oooooh so tasty",
                        BrewID = 1
                    },
                    new TastingNote()
                    {
                        Id = 2,
                        Date = new DateTime(2021, 1, 10),
                        Note = "Oh my god, what happened, has someone poisoned this!!",
                        BrewID = 1
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
