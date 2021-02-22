using System.Collections.Generic;
using thehomebrewapi.Entities;
using thehomebrewapi.Helpers;
using thehomebrewapi.ResourceParameters;

namespace thehomebrewapi.Services
{
    public interface IHomeBrewRepository
    {
        void AddBrew(Brew brew);

        public void AddIngredientForRecipeStep(int recipeStepId, Ingredient ingredient);

        void AddRecipe(Recipe recipe);

        void AddStepForRecipe(int recipeId, RecipeStep recipeStep);

        void AddTastingNote(TastingNote tastingNote);

        void AddTimerForRecipeStep(int recipeId, int stepId, Timer timer);

        void AddWaterProfile(WaterProfile waterProfile);

        bool BrewExists(int brewId);

        void DeleteBrew(Brew brew);

        void DeleteIngredient(Ingredient ingredient);

        void DeleteRecipe(Recipe recipe);

        void DeleteTastingNote(TastingNote tastingNote);

        void DeleteWaterProfile(WaterProfile waterProfile);

        Brew GetBrew(int brewId, bool includeAdditionalInfo = false);

        PagedList<Brew> GetBrews(BrewsResourceParameters brewsResourceParameters);

        IEnumerable<Brew> GetBrews();

        Ingredient GetIngredientForRecipeStep(int stepId, int ingredientId);

        IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId);

        IEnumerable<Ingredient> GetIngredientsForRecipeStep(int stepId);

        Recipe GetRecipe(int recipeId, bool includeSteps);

        IEnumerable<Recipe> GetRecipes();

        PagedList<Recipe> GetRecipes(RecipesResourceParameters recipesResourceParameters);

        RecipeStep GetRecipeStep(int stepId);

        IEnumerable<RecipeStep> GetStepsForRecipe(int recipeId, bool includeIngredients);

        PagedList<TastingNote> GetTastingNotes(TastingNotesResourceParameters tastingNotesResourceParameters);

        IEnumerable<TastingNote> GetTastingNotes();

        TastingNote GetTastingNote(int brewId, int noteId);

        WaterProfile GetWaterProfile(int waterProfileId, bool includeAdditions);

        PagedList<WaterProfile> GetWaterProfiles(WaterProfileResourceParameters waterProfileResourceParameters);

        bool RecipeExists(int recipeId);

        bool RecipeStepExists(int stepId);

        bool Save();

        void UpdateBrew(Brew brew);

        void UpdateIngredient(Ingredient ingredient);

        void UpdateRecipe(Recipe recipe);

        void UpdateTastingNote(TastingNote tastingNote);

        void UpdateWaterProfile(WaterProfile waterProfile);

        bool WaterProfileExists(int waterProfileId);
    }
}
