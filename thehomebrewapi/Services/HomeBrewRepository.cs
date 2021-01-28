using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using thehomebrewapi.Contexts;
using thehomebrewapi.Entities;
using thehomebrewapi.Helpers;
using thehomebrewapi.ResourceParameters;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Services
{
    public class HomeBrewRepository : IHomeBrewRepository, IDisposable
    {
        private readonly HomeBrewContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public HomeBrewRepository(HomeBrewContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId)
        {
            var recipe = _context.Recipes.Include(r => r.Steps).FirstOrDefault(r => r.Id == recipeId);

            var ingredients = new List<Ingredient>();

            foreach(var step in recipe.Steps)
            {
                ingredients.AddRange(_context.Ingredients.Where(i => i.RecipeStepId == step.Id).ToList());
            }

            return ingredients;
        }

        public Ingredient GetIngredientForRecipeStep(int stepId, int ingredientId)
        {
            return _context.Ingredients
                .Where(i => i.RecipeStepId == stepId && i.Id == ingredientId).FirstOrDefault();
        }

        public IEnumerable<Ingredient> GetIngredientsForRecipeStep(int stepId)
        {
            return _context.Ingredients
                .Where(i => i.RecipeStepId == stepId).ToList();
        }

        public Recipe GetRecipe(int recipeId, bool includeSteps)
        {
            if (includeSteps)
            {
                return _context.Recipes
                    .Include(r => r.Steps).ThenInclude(s => s.Ingredients)
                    .Include(r => r.Steps).ThenInclude(s => s.Timer)
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

        public PagedList<Recipe> GetRecipes(RecipesResourceParameters recipesResourceParameters)
        {
            if (recipesResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(recipesResourceParameters));
            }

            var collection = _context.Recipes as IQueryable<Recipe>;

            collection = collection.Include(r => r.WaterProfile).ThenInclude(wp => wp.Additions);

            if (Enum.IsDefined(typeof(ETypeOfBeer), recipesResourceParameters.BeerType))
            {
                var beerType = (ETypeOfBeer)recipesResourceParameters.BeerType;

                collection = collection.Where(r => r.Type == beerType);
            }

            if (!string.IsNullOrWhiteSpace(recipesResourceParameters.SearchQuery))
            {
                var searchQuery = recipesResourceParameters.SearchQuery.Trim();

                collection = collection.Where(r => r.Name.Contains(searchQuery) ||
                    r.Description.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(recipesResourceParameters.OrderBy))
            {
                var recipePropertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<Models.RecipeDto, Recipe>();

                collection = collection.ApplySort(recipesResourceParameters.OrderBy,
                    recipePropertyMappingDictionary);
            }

            return PagedList<Recipe>.Create(collection,
                recipesResourceParameters.PageNumber,
                recipesResourceParameters.PageSize);
        }

        public bool RecipeExists(int recipeId)
        {
            return _context.Recipes.Any(r => r.Id == recipeId);
        }

        public bool RecipeStepExists(int stepId)
        {
            return _context.RecipeSteps.Any(rs => rs.Id == stepId);
        }

        public bool WaterProfileExists(int waterProfileId)
        {
            return _context.WaterProfiles.Any(wp => wp.Id == waterProfileId);
        }

        public bool RecipeStepExists(int recipeId, int recipeStepId)
        {
            return _context.RecipeSteps.Any(rs => rs.RecipeId == recipeId && rs.Id == recipeStepId);
        }

        public void AddIngredientForRecipeStep(int stepId, Ingredient ingredient)
        {
            var recipeStep = GetRecipeStep(stepId);

            recipeStep.Ingredients.Add(ingredient);
        }

        public void UpdateIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Update(ingredient);
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
                var result = _context.RecipeSteps
                    .Include(rs => rs.Ingredients)
                    .Include(rs => rs.Timer)
                    .Where(rs => rs.RecipeId == recipeId).ToList();

                return result;
            }

            return _context.RecipeSteps
                .Include(rs => rs.Timer)
                .Where(rs => rs.RecipeId == recipeId).ToList();
        }

        public RecipeStep GetRecipeStep(int stepId)
        {
            return _context.RecipeSteps
                .Where(rs => rs.Id == stepId).FirstOrDefault();
        }

        public void AddStepForRecipe(int recipeId, RecipeStep recipeStep)
        {
            var recipe = GetRecipe(recipeId, false);
            recipe.Steps.Add(recipeStep);
        }

        public void AddTimerForRecipeStep(int recipeId, int stepId, Timer timer)
        {
            var recipeStep = GetRecipeStep(stepId);
            recipeStep.Timer = timer;
        }

        public PagedList<WaterProfile> GetWaterProfiles(WaterProfileResourceParameters waterProfileResourceParameters)
        {
            if (waterProfileResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(waterProfileResourceParameters));
            }

            var collection = _context.WaterProfiles as IQueryable<WaterProfile>;

            collection = collection.Include(wp => wp.Additions);

            if (!string.IsNullOrWhiteSpace(waterProfileResourceParameters.SearchQuery))
            {
                var searchQuery = waterProfileResourceParameters.SearchQuery.Trim();

                collection = collection.Where(r => r.Name.Contains(searchQuery) ||
                    r.Description.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(waterProfileResourceParameters.OrderBy))
            {
                var waterProfilePropertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<Models.WaterProfileDto, WaterProfile>();

                collection = collection.ApplySort(waterProfileResourceParameters.OrderBy,
                    waterProfilePropertyMappingDictionary);
            }

            return PagedList<WaterProfile>.Create(collection,
                waterProfileResourceParameters.PageNumber,
                waterProfileResourceParameters.PageSize);
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

        public PagedList<Brew> GetBrews(BrewsResourceParameters brewsResourceParameters)
        {
            if (brewsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(brewsResourceParameters));
            }

            var collection = _context.Brews as IQueryable<Brew>;

            if (brewsResourceParameters.MinRating > 0)
            {
                collection = collection.Where(b => b.Rating >= brewsResourceParameters.MinRating);
            }

            if (!string.IsNullOrWhiteSpace(brewsResourceParameters.SearchQuery))
            {
                var searchQuery = brewsResourceParameters.SearchQuery.Trim();

                collection = collection.Where(b => b.Name.Contains(searchQuery) ||
                    b.BrewingNotes.Contains(searchQuery) ||
                    b.TastingNotes.Where(tn => tn.Note.Contains(searchQuery)).Any());
            }

            if (!string.IsNullOrWhiteSpace(brewsResourceParameters.OrderBy))
            {
                var brewPropertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<Models.BrewDto, Brew>();

                collection = collection.ApplySort(brewsResourceParameters.OrderBy,
                    brewPropertyMappingDictionary);
            }

            return PagedList<Brew>.Create(collection,
                brewsResourceParameters.PageNumber,
                brewsResourceParameters.PageSize);
        }

        public IEnumerable<Brew> GetBrews()
        {
            return _context.Brews.OrderBy(b => b.BrewDate).ToList();
        }

        public Brew GetBrew(int brewId, bool includeAdditionalInfo = false)
        {
            if (includeAdditionalInfo)
            {
                return _context.Brews.Include(b => b.TastingNotes)
                                     .Include(b => b.Recipe.Steps).ThenInclude(s => s.Ingredients)
                                     .Include(b => b.Recipe.Steps).ThenInclude(s => s.Timer)
                                     .Include(b => b.Recipe.WaterProfile).ThenInclude(wp => wp.Additions)
                                     .FirstOrDefault(b => b.Id == brewId);
            }
            return _context.Brews
                .Include(b => b.Recipe.WaterProfile).ThenInclude(wp => wp.Additions)
                .FirstOrDefault(b => b.Id == brewId);
        }

        public void AddBrew(Brew brew)
        {
            _context.Brews.Add(brew);
        }

        public void DeleteBrew(Brew brew)
        {
            _context.Brews.Remove(brew);
        }

        public void UpdateBrew(Brew brew)
        {
            _context.Brews.Update(brew);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resource here
            }
        }
    }
}
