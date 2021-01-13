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

            return Ok(_mapper.Map<IEnumerable<RecipeWithoutIngredientsDto>>(recipes));
        }

        [HttpGet("{id}", Name = "GetRecipe")]
        public IActionResult GetRecipe(int id, bool includeIngredients = false)
        {
            var recipe = _homeBrewRepository.GetRecipe(id, includeIngredients);

            if (recipe == null)
            {
                return NotFound();
            }

            if (includeIngredients)
            {
                return Ok(_mapper.Map<RecipeDto>(recipe));
            }

            return Ok(_mapper.Map<RecipeWithoutIngredientsDto>(recipe));
        }

        [HttpPost]
        public IActionResult CreateRecipe([FromBody] RecipeForCreationDto recipe)
        {
            foreach(var ingredient in recipe.Ingredients)
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

            var recipeForCreation = _mapper.Map<Models.RecipeWithoutIngredientsDto>(recipe);
            var finalRecipe = _mapper.Map<Entities.Recipe>(recipeForCreation);

            _homeBrewRepository.AddRecipe(finalRecipe);

            _homeBrewRepository.Save();

            foreach(var ingredient in recipe.Ingredients)
            {
                var finalIngredient = _mapper.Map<Entities.Ingredient>(ingredient);

                _homeBrewRepository.AddIngredientForRecipe(finalRecipe.Id, finalIngredient);

                _homeBrewRepository.Save();
            }

            var createdRecipeToReturn = _mapper.Map<Models.RecipeDto>(finalRecipe);

            return CreatedAtRoute(
                "GetRecipe",
                new { id = finalRecipe.Id, includeIngredients = true},
                createdRecipeToReturn);
        }
    }
}
