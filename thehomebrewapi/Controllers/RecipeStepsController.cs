using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
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

        [HttpGet("{id}", Name = "GetRecipeStep")]
        public IActionResult GetRecipeStep(int recipeId, int id)
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

        [HttpPost]
        public IActionResult CreateRecipeStep(int recipeId,
            [FromBody] RecipeStepForCreationDto recipeStep)
        {
            foreach(var ingredient in recipeStep.Ingredients)
            {
                if (ingredient.Amount <= 0)
                {
                    ModelState.AddModelError(
                        "Description",
                        "The ingredient amount for all ingredients must be a value greater than 0.");

                    break;
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
    }
}
