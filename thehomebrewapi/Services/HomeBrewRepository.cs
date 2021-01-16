using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
            //return _context.Ingredients
            //    .Where(i => i.RecipeId == recipeId && i.Id == ingredientId).FirstOrDefault();
        }

        public IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId)
        {
            throw new NotImplementedException();
            //return _context.Ingredients
            //    .Where(i => i.RecipeId == recipeId).ToList();
        }

        public Recipe GetRecipe(int recipeId, bool includeSteps)
        {
            if (includeSteps)
            {
                return _context.Recipes
                    .Include(r => r.Steps).ThenInclude(s => s.Ingredients)
                    .Include(r => r.WaterProfile).ThenInclude(wp => wp.Additions)
                    .Where(r => r.Id == recipeId).FirstOrDefault();
            }

            return _context.Recipes
                .Include(r => r.WaterProfile).ThenInclude(wp => wp.Additions)
                .Where(r => r.Id == recipeId).FirstOrDefault();
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _context.Recipes
                .Include(r => r.WaterProfile).ThenInclude(wp => wp.Additions)
                .OrderBy(r => r.Name).ToList();
        }

        public bool RecipeExists(int recipeId)
        {
            return _context.Recipes.Any(r => r.Id == recipeId);
        }

        public bool WaterProfileExists(int waterProfileId)
        {
            return _context.WaterProfiles.Any(wp => wp.Id == waterProfileId);
        }

        public bool RecipeStepExists(int recipeId, int recipeStepId)
        {
            return _context.RecipeSteps.Any(rs => rs.RecipeId == recipeId && rs.Id == recipeStepId);
        }

        public void AddIngredientForRecipeStep(int recipeId, int recipeStepId, Ingredient ingredient)
        {
            var recipeStep = GetStepForRecipe(recipeId, recipeStepId);

            recipeStep.Ingredients.Add(ingredient);
        }

        public void UpdateIngredientForRecipe(int recipeId, Ingredient ingredient)
        {
            throw new NotImplementedException();
        }

        public void DeleteIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Remove(ingredient);
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
        }

        public void AddWaterProfile(WaterProfile waterProfile)
        {
            _context.WaterProfiles.Add(waterProfile);
        }

        public IEnumerable<RecipeStep> GetStepsForRecipe(int recipeId, bool includeIngredients)
        {
            if (includeIngredients)
            {
                var result = _context.RecipeSteps.Include(rs => rs.Ingredients)
                    .Where(rs => rs.RecipeId == recipeId).ToList();

                return result;
            }

            return _context.RecipeSteps
                .Where(rs => rs.RecipeId == recipeId).ToList();
        }

        public RecipeStep GetStepForRecipe(int recipeId, int stepId)
        {
            return _context.RecipeSteps
                .Where(rs => rs.RecipeId == recipeId && rs.Id == stepId).FirstOrDefault();
        }

        public void AddStepForRecipe(int recipeId, RecipeStep recipeStep)
        {
            var recipe = GetRecipe(recipeId, false);
            recipe.Steps.Add(recipeStep);
        }

        public void AddTimerForRecipeStep(int recipeId, int stepId, Timer timer)
        {
            var recipeStep = GetStepForRecipe(recipeId, stepId);
            recipeStep.Timer = timer;
        }

        public IEnumerable<WaterProfile> GetWaterProfiles()
        {
            return _context.WaterProfiles.OrderBy(wp => wp.Name).ToList();
        }

        public WaterProfile GetWaterProfile(int waterProfileId, bool includeAdditions)
        {
            if (includeAdditions)
            {
                return _context.WaterProfiles
                    .Include(wp => wp.Additions)
                    .FirstOrDefault(wp => wp.Id == waterProfileId);
            }

            return _context.WaterProfiles
                    .FirstOrDefault(wp => wp.Id == waterProfileId);
        }

        public void DeleteRecipe(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
        }

        public void UpdateRecipe(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
        }

        public void UpdateWaterProfile(WaterProfile waterProfile)
        {
            _context.WaterProfiles.Update(waterProfile);
        }

        public void DeleteWaterProfile(WaterProfile waterProfile)
        {
            _context.WaterProfiles.Remove(waterProfile);
        }

        public IEnumerable<Brew> GetBrews()
        {
            return _context.Brews.OrderBy(b => b.BrewDate).ToList();
        }

        public Brew GetBrew(int brewId, bool includeTastingNotes = false)
        {
            if (includeTastingNotes)
            {
                return _context.Brews
                    .Include(b => b.TastingNotes)
                    .FirstOrDefault(b => b.ID == brewId);
            }
            return _context.Brews
                .FirstOrDefault(b => b.ID == brewId);
        }

        public void AddBrew(Brew brew)
        {
            _context.Brews.Add(brew);
        }

        public void DeleteBrew(Brew brew)
        {
            _context.Brews.Remove(brew);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
