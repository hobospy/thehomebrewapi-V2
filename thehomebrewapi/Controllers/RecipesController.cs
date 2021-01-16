using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;

        public RecipesController(IHomeBrewRepository homeBrewRepository, IMapper mapper)
        {
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetRecipes()
        {
            var recipes = _homeBrewRepository.GetRecipes();

            return Ok(_mapper.Map<IEnumerable<RecipeWithoutStepsDto>>(recipes));
        }

        [HttpGet("{id}", Name = "GetRecipe")]
        public IActionResult GetRecipe(int id, bool includeSteps = false)
        {
            var recipe = _homeBrewRepository.GetRecipe(id, includeSteps);

            if (recipe == null)
            {
                return NotFound();
            }

            if (includeSteps)
            {
                return Ok(_mapper.Map<RecipeDto>(recipe));
            }

            return Ok(_mapper.Map<RecipeWithoutStepsDto>(recipe));
        }

        [HttpPost]
        public IActionResult CreateRecipe([FromBody] RecipeForCreationDto recipe)
        {
            if (recipe.WaterProfileId == 0 || !_homeBrewRepository.WaterProfileExists(recipe.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
            }

            foreach (var step in recipe.Steps)
            {
                foreach (var ingredient in step.Ingredients)
                {
                    if (ingredient.Amount <= 0)
                    {
                        ModelState.AddModelError(
                            "Description",
                            "The ingredient amount for all ingredients must be a value greater than 0.");

                        break;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalRecipe = _mapper.Map<Entities.Recipe>(recipe);

            _homeBrewRepository.AddRecipe(finalRecipe);
            finalRecipe.WaterProfile = _homeBrewRepository.GetWaterProfile(recipe.WaterProfileId, true);

            _homeBrewRepository.Save();

            var createdRecipeToReturn = _mapper.Map<Models.RecipeDto>(finalRecipe);

            return CreatedAtRoute(
                "GetRecipe",
                new { id = finalRecipe.Id, includeIngredients = true },
                createdRecipeToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRecipe(int id, [FromBody] RecipeForUpdateDto recipe)
        {
            if (recipe.WaterProfileId == 0 || !_homeBrewRepository.WaterProfileExists(recipe.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
            }

            foreach (var step in recipe.Steps)
            {
                foreach (var ingredient in step.Ingredients)
                {
                    if (ingredient.Amount <= 0)
                    {
                        ModelState.AddModelError(
                            "Description",
                            "The ingredient amount for all ingredients must be a value greater than 0.");

                        break;
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recipeEntity = _homeBrewRepository.GetRecipe(id, true);
            if (recipeEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(recipe, recipeEntity);

            _homeBrewRepository.UpdateRecipe(recipeEntity);
            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateRecipe(int id,
            [FromBody] JsonPatchDocument<RecipeForUpdateDto> patchDoc)
        {
            var recipeEntity = _homeBrewRepository.GetRecipe(id, true);
            if (recipeEntity == null)
            {
                return NotFound();
            }

            var recipeToPatch = _mapper.Map<RecipeForUpdateDto>(recipeEntity);

            patchDoc.ApplyTo(recipeToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (recipeToPatch.WaterProfileId == 0 || !_homeBrewRepository.WaterProfileExists(recipeToPatch.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
            }

            foreach (var step in recipeToPatch.Steps)
            {
                foreach (var ingredient in step.Ingredients)
                {
                    if (ingredient.Amount <= 0)
                    {
                        ModelState.AddModelError(
                            "Description",
                            "The ingredient amount for all ingredients must be a value greater than 0.");

                        break;
                    }
                }
            }

            if (!TryValidateModel(recipeToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(recipeToPatch, recipeEntity);

            _homeBrewRepository.UpdateRecipe(recipeEntity);
            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            var recipeEntity = _homeBrewRepository.GetRecipe(id, false);
            if (recipeEntity == null)
            {
                return NotFound();
            }

            _homeBrewRepository.DeleteRecipe(recipeEntity);
            _homeBrewRepository.Save();

            return NoContent();
        }
    }
}
