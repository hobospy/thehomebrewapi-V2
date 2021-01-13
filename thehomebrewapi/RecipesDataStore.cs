using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using thehomebrewapi.Models;

namespace thehomebrewapi
{
    public class RecipesDataStore
    {
        public static RecipesDataStore Current { get; } = new RecipesDataStore();

        public List<RecipeDto> Recipes { get; set; }

        public RecipesDataStore()
        {
            Recipes = new List<RecipeDto>()
            {
                new RecipeDto()
                {
                    Id = 1,
                    Name = "Amarillo SMaSH",
                    Description = "Single malt and single hop recipe using Amarillo hops",
                    Ingredients = new List<IngredientDto>()
                    {
                        new IngredientDto()
                        {
                            Id = 1,
                            Name = "Amarillo",
                            Amount = 68
                        },
                        new IngredientDto()
                        {
                            Id = 2,
                            Name = "Pale malt",
                            Amount = 5.5
                        },
                        new IngredientDto()
                        {
                            Id = 3,
                            Name = "Light crystal malt",
                            Amount = 150
                        }
                    }
                },
                new RecipeDto()
                {
                    Id = 2,
                    Name = "Raspberry Porter",
                    Description = "Porter based recipe with raspberries added during the second half of fermentation",
                    Ingredients = new List<IngredientDto>()
                    {
                        new IngredientDto()
                        {
                            Id = 4,
                            Name = "Dark roast malt",
                            Amount = 300
                        },
                        new IngredientDto()
                        {
                            Id = 5,
                            Name = "Bittering",
                            Amount = 60
                        }
                    }
                },
                new RecipeDto()
                {
                    Id = 3,
                    Name = "Bock",
                    Description = "Basic Bock recipe for those cold nights",
                    Ingredients = new List<IngredientDto>()
                    {
                        new IngredientDto()
                        {
                            Id = 6,
                            Name = "Dark crystal",
                            Amount = 120
                        },
                        new IngredientDto()
                        {
                            Id = 7,
                            Name = "Rice husks",
                            Amount = 300
                        },
                        new IngredientDto()
                        {
                            Id = 8,
                            Name = "Whirlfloc",
                            Amount = 0.5
                        }
                    }
                }
            };
        }
    }
}
