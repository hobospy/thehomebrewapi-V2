using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        // TODO: Possibly should be replaced with a filter/search
        /// <summary>
        /// Gets all ingredients for the specified recipe
        /// </summary>
        [HttpGet("/api/recipes/{recipesId}/ingredients")]
        [ProducesResponseType(typeof(IEnumerable<IngredientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Gets ingredients for the specified recipe step
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IngredientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Gets the ingredient details for the specified ingredent within a specified recipe step
        /// </summary>
        [HttpGet("{id}", Name = "GetIngredient")]
        [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Create an ingredient to associate with the specified recipe step
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IngredientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates an existing ingredient based on the recipe, recipe step and ingredient id, the
        /// ingredient model supplied is what is pushed into the persistent storage.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Updates an existing ingredient based on the recipe, recipe step and ingredient id,
        /// only the properties supplied will be updated
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes the ingredient with the supplied recipe, recipe step and ingredient id
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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