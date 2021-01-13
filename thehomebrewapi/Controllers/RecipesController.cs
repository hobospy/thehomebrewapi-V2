using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRecipes()
        {
            return Ok(RecipesDataStore.Current.Recipes);
        }

        [HttpGet("{id}")]
        public IActionResult GetRecipe(int id)
        {
            var recipeToReturn = RecipesDataStore.Current.Recipes.FirstOrDefault(r => r.Id == id);
            if (recipeToReturn == null)
            {
                return NotFound();
            }

            return Ok(recipeToReturn);
        }
    }
}
