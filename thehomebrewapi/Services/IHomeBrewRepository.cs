using System.Collections.Generic;
using thehomebrewapi.Entities;

namespace thehomebrewapi.Services
{
    public interface IHomeBrewRepository
    {
        IEnumerable<Recipe> GetRecipes();

        Recipe GetRecipe(int recipeId, bool includeIngredients);

        IEnumerable<Ingredient> GetIngredientsForRecipe(int recipeId);

        Ingredient GetIngredientForRecipe(int recipeId, int ingredientId);

        bool RecipeExists(int recipeId);

        void AddIngredientForRecipe(int recipeId, Ingredient ingredient);

        void UpdateIngredientForRecipe(int recipeId, Ingredient ingredient);

        void DeleteIngredient(Ingredient ingredient);

        void AddRecipe(Recipe recipe);

        bool Save();
    }
}
