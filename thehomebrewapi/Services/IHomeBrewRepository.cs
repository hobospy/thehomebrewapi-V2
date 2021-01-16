using System.Collections.Generic;
using thehomebrewapi.Entities;

namespace thehomebrewapi.Services
{
    public interface IHomeBrewRepository
    {
        IEnumerable<Recipe> GetRecipes();

        Recipe GetRecipe(int recipeId, bool includeSteps);

        IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId);

        Ingredient GetIngredientForRecipe(int recipeId, int ingredientId);

        bool RecipeExists(int recipeId);

        bool WaterProfileExists(int waterProfileId);

        public void AddIngredientForRecipeStep(int recipeId, int recipeStepId, Ingredient ingredient);

        void UpdateIngredientForRecipe(int recipeId, Ingredient ingredient);

        void DeleteIngredient(Ingredient ingredient);

        void AddRecipe(Recipe recipe);

        void AddWaterProfile(WaterProfile waterProfile);

        IEnumerable<RecipeStep> GetStepsForRecipe(int recipeId, bool includeIngredients);

        RecipeStep GetStepForRecipe(int recipeId, int stepId);

        void AddStepForRecipe(int recipeId, RecipeStep recipeStep);

        void AddTimerForRecipeStep(int recipeId, int stepId, Timer timer);

        IEnumerable<WaterProfile> GetWaterProfiles();

        WaterProfile GetWaterProfile(int waterProfileId, bool includeAdditions);

        void DeleteRecipe(Recipe recipe);

        void UpdateRecipe(Recipe recipe);

        void UpdateWaterProfile(WaterProfile waterProfile);

        void DeleteWaterProfile(WaterProfile waterProfile);

        IEnumerable<Brew> GetBrews();

        Brew GetBrew(int brewId, bool includeTastingNotes = false);

        void AddBrew(Brew brew);

        void DeleteBrew(Brew brew);

        void UpdateBrew(Brew brew);

        bool Save();
    }
}
