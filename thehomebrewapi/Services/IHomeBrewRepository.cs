using System.Collections.Generic;
using thehomebrewapi.Entities;

namespace thehomebrewapi.Services
{
    public interface IHomeBrewRepository
    {
        IEnumerable<Recipe> GetRecipes();

        Recipe GetRecipe(int recipeId, bool includeSteps);

        IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId);

        IEnumerable<Ingredient> GetIngredientsForRecipeStep(int stepId);

        Ingredient GetIngredientForRecipeStep(int stepId, int ingredientId);

        bool RecipeExists(int recipeId);

        bool RecipeStepExists(int stepId);

        bool WaterProfileExists(int waterProfileId);

        public void AddIngredientForRecipeStep(int recipeStepId, Ingredient ingredient);

        void UpdateIngredient(Ingredient ingredient);

        void DeleteIngredient(Ingredient ingredient);

        void AddRecipe(Recipe recipe);

        void AddWaterProfile(WaterProfile waterProfile);

        IEnumerable<RecipeStep> GetStepsForRecipe(int recipeId, bool includeIngredients);

        RecipeStep GetRecipeStep(int stepId);

        void AddStepForRecipe(int recipeId, RecipeStep recipeStep);

        void AddTimerForRecipeStep(int recipeId, int stepId, Timer timer);

        IEnumerable<WaterProfile> GetWaterProfiles();

        WaterProfile GetWaterProfile(int waterProfileId, bool includeAdditions);

        void DeleteRecipe(Recipe recipe);

        void UpdateRecipe(Recipe recipe);

        void UpdateWaterProfile(WaterProfile waterProfile);

        void DeleteWaterProfile(WaterProfile waterProfile);

        IEnumerable<Brew> GetBrews();

        Brew GetBrew(int brewId, bool includeAdditionalInfo = false);

        void AddBrew(Brew brew);

        void DeleteBrew(Brew brew);

        void UpdateBrew(Brew brew);

        bool Save();
    }
}
