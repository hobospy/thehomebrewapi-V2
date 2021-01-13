using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using thehomebrewapi.Models;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/ingredients")]
    public class IngredientsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetIngredients(int recipeId)
        {
            var recipe = RecipesDataStore.Current.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                return NotFound();
            }

            return Ok(recipe.Ingredients);
        }

        [HttpGet("{id}", Name = "GetIngredient")]
        public IActionResult GetIngredient(int recipeId, int id)
        {
            var recipe = RecipesDataStore.Current.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                return NotFound();
            }

            var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(ingredient);
        }

        [HttpPost]
        public IActionResult CreateIngredient(int recipeId,
            [FromBody] IngredientForCreationDto ingredient)
        {
            if (ingredient.Amount <= 0)
            {
                ModelState.AddModelError(
                    "Description",
                    "The ingredient amount must be a value greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipe = RecipesDataStore.Current.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                return NotFound();

            }

            var maxId = RecipesDataStore.Current.Recipes.SelectMany(
                r => r.Ingredients).Max(i => i.Id);

            var finalIngredient = new IngredientDto()
            {
                Id = maxId,
                Name = ingredient.Name,
                Amount = ingredient.Amount
            };

            recipe.Ingredients.Add(finalIngredient);

            return CreatedAtRoute(
                "GetIngredient",
                new { recipeId, id = finalIngredient.Id },
                finalIngredient);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateIngredient(int recipeId, int id,
            [FromBody] IngredientForUpdateDto ingredient)
        {
            if (ingredient.Amount <= 0)
            {
                ModelState.AddModelError(
                    "Description",
                    "The ingredient amount must be a value greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipe = RecipesDataStore.Current.Recipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe == null)
            {
                return NotFound();

            }

            var ingredientFromStore = recipe.Ingredients
                .FirstOrDefault(i => i.Id == id);
            if (ingredientFromStore == null)
            {
                return NotFound();
            }

            ingredientFromStore.Name = ingredient.Name;
            ingredientFromStore.Amount = ingredient.Amount;

            return NoContent();
        }
    }
}
