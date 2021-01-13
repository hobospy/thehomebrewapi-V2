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

        [HttpGet("{id}")]
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
    }
}
