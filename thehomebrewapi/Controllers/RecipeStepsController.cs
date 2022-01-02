using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using thehomebrewapi.Models;
using thehomebrewapi.Services;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/recipeSteps")]
    public class RecipeStepsController : ControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;

        public RecipeStepsController(IHomeBrewRepository homeBrewRepository, IMapper mapper)
        {
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Gets a list of steps associated with the supplied recipe ID.  Ingredients can optionally
        /// be included
        /// </summary>
        [HttpGet(Name = "GetRecipeSteps")]
        [ProducesResponseType(typeof(IEnumerable<RecipeStepDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<RecipeStepWithoutIngredientsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRecipeSteps(int recipeId, bool includeIngredients = false)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            var stepsForRecipe = _homeBrewRepository.GetStepsForRecipe(recipeId, includeIngredients);

            if (includeIngredients)
            {
                return Ok(_mapper.Map<IEnumerable<RecipeStepDto>>(stepsForRecipe));
            }

            return Ok(_mapper.Map<IEnumerable<RecipeStepWithoutIngredientsDto>>(stepsForRecipe));
        }

        /// <summary>
        /// Gets a recipe step (including ingredients) from the supplied recipe and step ID.
        /// </summary>
        [HttpGet("{id}", Name = "GetRecipeStep")]
        [ProducesResponseType(typeof(IEnumerable<RecipeStepDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecipeStepDto> GetRecipeStep(int recipeId, int id)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            var recipeStep = _homeBrewRepository.GetRecipeStep(id);
            if (recipeStep == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<RecipeStepDto>(recipeStep));
        }

        /// <summary>
        /// Create a new recipe step to be associated with the supplied recipe
        /// </summary>
        [HttpPost(Name = "CreateRecipeStep")]
        [ProducesResponseType(typeof(RecipeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RecipeStepDto> CreateRecipeStep(int recipeId,
            [FromBody] RecipeStepForCreationDto recipeStep)
        {
            if (!_homeBrewRepository.RecipeExists(recipeId))
            {
                return NotFound();
            }

            var finalRecipeStep = _mapper.Map<Entities.RecipeStep>(recipeStep);

            _homeBrewRepository.AddStepForRecipe(recipeId, finalRecipeStep);

            _homeBrewRepository.Save();

            var createdRecipeStepToReturn = _mapper.Map<Models.RecipeStepDto>(finalRecipeStep);

            return CreatedAtRoute(
                "GetRecipeStep",
                new { recipeId, id = finalRecipeStep.Id },
                createdRecipeStepToReturn);
        }

        [HttpOptions]
        public ActionResult GetRecipeStepsOptions()
        {
            Response.Headers.Add("Allow", "GET, POST");
            return Ok();
        }
    }
}