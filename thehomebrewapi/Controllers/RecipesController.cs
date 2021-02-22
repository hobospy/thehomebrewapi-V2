using AutoMapper;
//using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using thehomebrewapi.Helpers;
using thehomebrewapi.Models;
using thehomebrewapi.ResourceParameters;
using thehomebrewapi.Services;
using static thehomebrewapi.Entities.Enumerations;

namespace thehomebrewapi.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    public class RecipesController : ExtendedControllerBase
    {
        private readonly IHomeBrewRepository _homeBrewRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public RecipesController(IHomeBrewRepository homeBrewRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
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
        //[HttpCacheExpiration(NoStore = true)]
        public ActionResult<IEnumerable<RecipeWithoutStepsDto>> GetRecipes(
            [FromQuery] RecipesResourceParameters recipesResourceParameters,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<RecipeDto, Entities.Recipe>
                (recipesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var recipes = _homeBrewRepository.GetRecipes(recipesResourceParameters);

            var paginationMetaData = new
            {
                totalCount = recipes.TotalCount,
                pageSize = recipes.PageSize,
                currentPage = recipes.CurrentPage,
                totalPages = recipes.TotalPages
            };

            Response.Headers.Add(this.PAGINATION_HEADER,
                JsonSerializer.Serialize(paginationMetaData));

            var shapedRecipes = _mapper.Map<IEnumerable<RecipeWithoutStepsDto>>(recipes)
                                       .ShapeData(null);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForRecipes(recipesResourceParameters,
                                recipes.HasNext,
                                recipes.HasPrevious);

                var shapedRecipesWithLinks = shapedRecipes.Select(recipe =>
                {
                    var recipeAsDictionary = recipe as IDictionary<string, object>;
                    var recipeLinks = CreateLinksForRecipe((int)recipeAsDictionary["Id"], false);
                    recipeAsDictionary.Add(this.LINKS, recipeLinks);
                    return recipeAsDictionary;
                });

                var linkedCollectionResouce = new
                {
                    value = shapedRecipesWithLinks,
                    links
                };

                return Ok(linkedCollectionResouce);
            }

            return Ok(shapedRecipes);
        }

        [HttpGet("{id}", Name = "GetRecipe")]
        public IActionResult GetRecipe(int id,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes,
            bool includeSteps = false)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var recipe = _homeBrewRepository.GetRecipe(id, includeSteps);

            if (recipe == null)
            {
                return NotFound();
            }

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForRecipe(id, includeSteps);

                // We aren't supporting shaping of data currently so pass in an empty string
                var linkedResourceToReturn = includeSteps ?
                    _mapper.Map<RecipeDto>(recipe).ShapeData(null) as IDictionary<string, object> :
                    _mapper.Map<RecipeWithoutStepsDto>(recipe).ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return includeSteps ?
                Ok(_mapper.Map<RecipeDto>(recipe)) :
                Ok(_mapper.Map<RecipeWithoutStepsDto>(recipe).ShapeData(null));
        }

        [HttpPost(Name = "CreateRecipe")]
        public ActionResult<RecipeDto> CreateRecipe([FromBody] RecipeForCreationDto recipe,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

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

            var includeSteps = true;
            var finalRecipe = _mapper.Map<Entities.Recipe>(recipe);

            _homeBrewRepository.AddRecipe(finalRecipe);
            finalRecipe.WaterProfile = _homeBrewRepository.GetWaterProfile(recipe.WaterProfileId, true);
            _homeBrewRepository.Save();

            var recipeToReturn = _mapper.Map<Models.RecipeDto>(finalRecipe);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForRecipe(recipeToReturn.Id, includeSteps);

                var linkedResourceToReturn = recipeToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return CreatedAtRoute(
                    "GetRecipe",
                    new { id = linkedResourceToReturn["Id"], includeSteps },
                    linkedResourceToReturn);
            }

            return CreatedAtRoute(
                    "GetRecipe",
                    new { id = recipeToReturn.Id, includeSteps },
                    recipeToReturn);
        }

        [HttpPut("{id}", Name = "UpdateRecipe")]
        public ActionResult UpdateRecipe(int id,
            [FromBody] RecipeForUpdateDto recipe,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

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

            var includeSteps = true;
            var recipeEntity = _homeBrewRepository.GetRecipe(id, includeSteps);
            if (recipeEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(recipe, recipeEntity);
            _homeBrewRepository.UpdateRecipe(recipeEntity);
            _homeBrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForRecipe(recipeEntity.Id, includeSteps);

                var recipeToReturn = _mapper.Map<RecipeDto>(recipeEntity);
                var linkedResourceToReturn = recipeToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateRecipe")]
        public ActionResult PartiallyUpdateRecipe(int id,
            [FromBody] JsonPatchDocument<RecipeForUpdateDto> patchDoc,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var includeSteps = true;
            var recipeEntity = _homeBrewRepository.GetRecipe(id, includeSteps);
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

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForRecipe(recipeEntity.Id, includeSteps);

                var recipeToReturn = _mapper.Map<RecipeDto>(recipeEntity);
                var linkedResourceToReturn = recipeToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteRecipe" )]
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
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.HEAD}, {this.OPTIONS}");

            return Ok();
        }

        [HttpOptions("{id}")]
        public ActionResult GetRecipeOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.DELETE}, {this.GET}, {this.OPTIONS}, {this.PATCH}, {this.POST}, {this.PUT}");

            return Ok();
        }

        #region Private functions
        private IEnumerable<LinkDto> CreateLinksForRecipe(int id, bool includeSteps)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRecipe", new { id, includeSteps }),
                this.SELF,
                this.GET));

            links.Add(
                new LinkDto(Url.Link("UpdateRecipe", new { id }),
                "update_recipe",
                this.PUT));

            links.Add(
                new LinkDto(Url.Link("PartiallyUpdateRecipe", new { id }),
                "partially_update_recipe",
                this.PATCH));

            links.Add(
                new LinkDto(Url.Link("DeleteRecipe", new { id }),
                "delete_recipe",
                this.DELETE));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForRecipes(
            RecipesResourceParameters recipesResourceParameters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(CreateRecipesResourceUri(
                    recipesResourceParameters, ETypeOfResourceUri.Current),
                this.SELF,
                this.GET));

            if (hasNext)
            {
                links.Add(
                new LinkDto(CreateRecipesResourceUri(
                    recipesResourceParameters, ETypeOfResourceUri.NextPage),
                this.NEXT_PAGE,
                this.GET));
            }

            if (hasPrevious)
            {
                links.Add(
                new LinkDto(CreateRecipesResourceUri(
                    recipesResourceParameters, ETypeOfResourceUri.PreviousPage),
                this.PREVIOUS_PAGE,
                this.GET));
            }

            return links;
        }

        private string CreateRecipesResourceUri(
            RecipesResourceParameters recipesResourceParameters,
            ETypeOfResourceUri type)
        {
            var linkProps = new ExpandoObject() as IDictionary<string, Object>;

            if (Enum.IsDefined(typeof(ETypeOfBeer), recipesResourceParameters.BeerType))
            {
                linkProps.Add("beerType", recipesResourceParameters.BeerType);
            }

            switch (type)
            {
                case ETypeOfResourceUri.PreviousPage:
                    linkProps.Add("orderBy", recipesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", recipesResourceParameters.PageNumber - 1);
                    linkProps.Add("pageSize", recipesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", recipesResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);
                case ETypeOfResourceUri.NextPage:
                    linkProps.Add("orderBy", recipesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", recipesResourceParameters.PageNumber + 1);
                    linkProps.Add("pageSize", recipesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", recipesResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);
                case ETypeOfResourceUri.Current:
                default:
                    linkProps.Add("orderBy", recipesResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", recipesResourceParameters.PageNumber);
                    linkProps.Add("pageSize", recipesResourceParameters.PageSize);
                    linkProps.Add("searchQuery", recipesResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);
            }
        }
        #endregion
    }
}
