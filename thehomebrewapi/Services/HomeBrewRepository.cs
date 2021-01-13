using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using thehomebrewapi.Contexts;
using thehomebrewapi.Entities;

namespace thehomebrewapi.Services
{
    public class HomeBrewRepository : IHomeBrewRepository
    {
        private readonly HomeBrewContext _context;

        public HomeBrewRepository(HomeBrewContext context)
        {
            _context = context ?? throw new NullArgumentException(nameof(context));
        }

        public Ingredient GetIngredientForRecipe(int recipeId, int ingredientId)
        {
            return _context.Ingredients
                .Where(i => i.RecipeId == recipeId && i.Id == ingredientId).FirstOrDefault();
        }

        public IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId)
        {
            return _context.Ingredients
                .Where(i => i.RecipeId == recipeId).ToList();
        }

        public Recipe GetRecipe(int recipeId, bool includeIngredients)
        {
            if (includeIngredients)
            {
                return _context.Recipes.Include(r => r.Ingredients)
                    .Where(r => r.Id == recipeId).FirstOrDefault();
            }

            return _context.Recipes
                .Where(r => r.Id == recipeId).FirstOrDefault();
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _context.Recipes.OrderBy(r => r.Name).ToList();
        }

        public bool RecipeExists(int recipeId)
        {
            return _context.Recipes.Any(r => r.Id == recipeId);
        }

        public void AddIngredientForRecipe(int recipeId, Ingredient ingredient)
        {
            var recipe = GetRecipe(recipeId, false);

            recipe.Ingredients.Add(ingredient);
        }

        public void UpdateIngredientForRecipe(int recipeId, Ingredient ingredient)
        {

        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Remove(ingredient);
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
