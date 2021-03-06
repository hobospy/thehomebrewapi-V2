﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/recipeSteps/{recipeStepId}/ingredients")]
    public class IngredientsController : ControllerBase
    {
        private readonly ILogger<IngredientsController> _logger;
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;

        public IngredientsController(ILogger<IngredientsController> logger, IHomeBrewRepository homeBrewRepository, IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        // Possibly should be replaced with a filter/search
        [HttpGet("/api/recipes/{recipesId}/ingredients")]
        public ActionResult<IEnumerable<IngredientDto>> GetFullRecipeIngredients(int recipesId)
        {
            try
            {
                if (!_homeBrewRepository.RecipeExists(recipesId))
                {
                    _logger.LogInformation($"Recipe with id {recipesId} wasn't found when accessing ingredients");

                    return NotFound();
                }
                var ingredientsForRecipe = _homeBrewRepository.GetIngredientsForRecipe(recipesId);

                return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredientsForRecipe));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting ingredients for recipe with id {recipesId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<IngredientDto>> GetIngredients(int recipeId, int recipeStepId)
        {
            try
            {
                if (!_homeBrewRepository.RecipeExists(recipeId))
                {
                    _logger.LogInformation($"Recipe with id {recipeId} wasn't found when accessing ingredients");

                    return NotFound();
                }

                if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
                {
                    _logger.LogInformation($"Recipe step with id {recipeStepId} wasn't found when accessing ingredients");

                    return NotFound();
                }

                var ingredientsForRecipe = _homeBrewRepository.GetIngredientsForRecipeStep(recipeStepId);
                
                return Ok(_mapper.Map<IEnumerable<IngredientDto>>(ingredientsForRecipe));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting ingredients for recipe step with id {recipeStepId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{id}", Name = "GetIngredient")]
        public ActionResult<IngredientDto> GetIngredient(int recipeId, int recipeStepId, int id)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                _logger.LogInformation($"Recipe with id {recipeId} wasn't found when accessing ingredients");

                return NotFound();
            }

            if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
            {
                _logger.LogInformation($"Recipe step with id {recipeStepId} wasn't found when accessing ingredients");

                return NotFound();
            }

            var ingredient = _homeBrewRepository.GetIngredientForRecipeStep(recipeStepId, id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IngredientDto>(ingredient));
        }

        [HttpPost]
        public ActionResult<IngredientDto> CreateIngredient(int recipeStepId,
            [FromBody] IngredientForCreationDto ingredient)
        {
            if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
            {
                return NotFound();
            }

            var finalIngredient = _mapper.Map<Entities.Ingredient>(ingredient);

            _homeBrewRepository.AddIngredientForRecipeStep(recipeStepId, finalIngredient);

            _homeBrewRepository.Save();

            var createdIngredientToReturn = _mapper.Map<Models.IngredientDto>(finalIngredient);

            return CreatedAtRoute(
                "GetIngredient",
                new { recipeStepId, id = finalIngredient.Id },
                createdIngredientToReturn);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateIngredient(int recipeId, int recipeStepId, int id,
            [FromBody] IngredientForUpdateDto ingredient)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
            {
                return NotFound();
            }

            var ingredientEntity = _homeBrewRepository
                .GetIngredientForRecipeStep(recipeStepId, id);
            if (ingredientEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(ingredient, ingredientEntity);

            _homeBrewRepository.UpdateIngredient(ingredientEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartiallyUpdateIngredient(int recipeId, int recipeStepId, int id,
            [FromBody] JsonPatchDocument<IngredientForUpdateDto> patchDoc)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
            {
                return NotFound();
            }

            var ingredientEntity = _homeBrewRepository.GetIngredientForRecipeStep(recipeStepId, id);
            if (ingredientEntity == null)
            {
                return NotFound();
            }

            var ingredientToPatch = _mapper.Map<IngredientForUpdateDto>(ingredientEntity);

            patchDoc.ApplyTo(ingredientToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(ingredientToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(ingredientToPatch, ingredientEntity);

            _homeBrewRepository.UpdateIngredient(ingredientEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteIngredient(int recipeId, int recipeStepId, int id)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            if (!_homeBrewRepository.RecipeStepExists(recipeStepId))
            {
                return NotFound();
            }

            var ingredientEntity = _homeBrewRepository.GetIngredientForRecipeStep(recipeStepId, id);
            if (ingredientEntity == null)
            {
                return NotFound();
            }

            _homeBrewRepository.DeleteIngredient(ingredientEntity);

            _homeBrewRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public ActionResult GetIngredientsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, PATCH, DELETE");
            return Ok();
        }
    }
}
