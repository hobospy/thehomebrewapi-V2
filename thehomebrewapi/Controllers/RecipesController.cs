using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using thehomebrewapi.Models;
using thehomebrewapi.ResourceParameters;
using thehomebrewapi.Services;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public RecipesController(IHomeBrewRepository homeBrewRepository, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _homeBrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        [HttpGet(Name = "GetRecipes")]
        [HttpHead]
        public ActionResult<IEnumerable<RecipeWithoutStepsDto>> GetRecipes(
            [FromQuery] RecipesResourceParameters recipesResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<RecipeDto, Entities.Recipe>
                (recipesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var recipes = _homeBrewRepository.GetRecipes(recipesResourceParameters);

            var previousPageLink = recipes.HasPrevious ?
                CreateRecipesResourceUri(recipesResourceParameters, ETypeOfResourceUri.PreviousPage) :
                null;
            var nextPageLink = recipes.HasNext ?
                CreateRecipesResourceUri(recipesResourceParameters, ETypeOfResourceUri.NextPage) :
                null;

            var paginationMetaData = new
            {
                totalCount = recipes.TotalCount,
                pageSize = recipes.PageSize,
                currentPage = recipes.CurrentPage,
                totalPages = recipes.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

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
        public ActionResult<RecipeDto> CreateRecipe([FromBody] RecipeForCreationDto recipe)
        {
            if (!_homeBrewRepository.WaterProfileExists(recipe.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
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
        public ActionResult UpdateRecipe(int id, [FromBody] RecipeForUpdateDto recipe)
        {
            if (!_homeBrewRepository.WaterProfileExists(recipe.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
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
        public ActionResult PartiallyUpdateRecipe(int id,
            [FromBody] JsonPatchDocument<RecipeForUpdateDto> patchDoc)
        {
            var recipeEntity = _homeBrewRepository.GetRecipe(id, true);
            if (recipeEntity == null)
            {
                return NotFound();
            }

            var recipeToPatch = _mapper.Map<RecipeForUpdateDto>(recipeEntity);

            patchDoc.ApplyTo(recipeToPatch, ModelState);

            if (!_homeBrewRepository.WaterProfileExists(recipeToPatch.WaterProfileId))
            {
                ModelState.AddModelError(
                            "Description",
                            "The water profile ID for the recipe must exist.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        public ActionResult DeleteRecipe(int id)
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

        [HttpOptions]
        public ActionResult GetRecipesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, PATCH, DELETE");
            return Ok();
        }

        private string CreateRecipesResourceUri(
            RecipesResourceParameters recipesResourceParameters,
            ETypeOfResourceUri type)
        {
            switch(type)
            {
                case ETypeOfResourceUri.PreviousPage:
                    return Url.Link("GetRecipes",
                        new
                        {
                            orderBy = recipesResourceParameters.OrderBy,
                            pageNumber = recipesResourceParameters.PageNumber - 1,
                            pageSize = recipesResourceParameters.PageSize,
                            beerType = recipesResourceParameters.BeerType,
                            searchQuery = recipesResourceParameters.SearchQuery
                        });
                case ETypeOfResourceUri.NextPage:
                    return Url.Link("GetRecipes",
                        new
                        {
                            orderBy = recipesResourceParameters.OrderBy,
                            pageNumber = recipesResourceParameters.PageNumber + 1,
                            pageSize = recipesResourceParameters.PageSize,
                            beerType = recipesResourceParameters.BeerType,
                            searchQuery = recipesResourceParameters.SearchQuery
                        });
                default:
                    return Url.Link("GetRecipes",
                        new
                        {
                            orderBy = recipesResourceParameters.OrderBy,
                            pageNumber = recipesResourceParameters.PageNumber,
                            pageSize = recipesResourceParameters.PageSize,
                            beerType = recipesResourceParameters.BeerType,
                            searchQuery = recipesResourceParameters.SearchQuery
                        });
            }
        }
    }
}
