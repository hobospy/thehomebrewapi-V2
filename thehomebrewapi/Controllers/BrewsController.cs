using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    [Produces("application/json")]
    [Route("api/brews")]
    public class BrewsController : ExtendedControllerBase
    {
        private readonly IHomeBrewRepository _homebrewRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public BrewsController(IHomeBrewRepository homeBrewRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService)
        {
            _homebrewRepository = homeBrewRepository ??
                throw new ArgumentNullException(nameof(homeBrewRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));
        }

        /// <summary>
        /// Gets a list of brews.  Optionally supply properties specifying whether you want to have
        /// additional properties returned (the recipe the brew is based on and any associated
        /// tasting notes) along with any filters.
        /// </summary>
        [HttpGet(Name = "GetBrews")]
        [HttpHead]
        [ProducesResponseType(typeof(IEnumerable<BrewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<BrewWithoutAdditionalInfoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<BrewWithoutAdditionalInfoDto>> GetBrews(
            [FromQuery] BrewsResourceParameters brewsResourceParameters,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<BrewDto, Entities.Brew>
                (brewsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var brews = _homebrewRepository.GetBrews(brewsResourceParameters);

            var paginationMetaData = new
            {
                totalCount = brews.TotalCount,
                pageSize = brews.PageSize,
                currentPage = brews.CurrentPage,
                totalPages = brews.TotalPages
            };

            Response.Headers.Add(this.PAGINATION_HEADER,
                JsonSerializer.Serialize(paginationMetaData));

            var shapedBrews = brewsResourceParameters.IncludeAdditionalInfo ?
                                                        _mapper.Map<IEnumerable<BrewDto>>(brews).ShapeData(null) :
                                                        _mapper.Map<IEnumerable<BrewWithoutAdditionalInfoDto>>(brews).ShapeData(null);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForBrews(brewsResourceParameters,
                                brews.HasNext,
                                brews.HasPrevious);

                var shapedBrewsWithLinks = shapedBrews.Select(brew =>
                {
                    var brewAsDictionary = brew as IDictionary<string, object>;
                    var brewLinks = CreateLinksForBrew((int)brewAsDictionary["Id"], false);
                    brewAsDictionary.Add(this.LINKS, brewLinks);
                    return brewAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedBrewsWithLinks,
                    links
                };

                return Ok(linkedCollectionResource);
            }

            return Ok(shapedBrews);
        }

        /// <summary>
        /// Get the brew associated with the supplied id.  Optionally supply a boolean value indicating
        /// whether you want to include additional properties (the recipe the brew is based on and any
        /// associated tasting notes).
        /// </summary>
        [HttpGet("{id}", Name = "GetBrew")]
        [ProducesResponseType(typeof(BrewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BrewWithoutAdditionalInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetBrew(int id,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes,
            bool includeAdditionalInfo = false)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var brew = _homebrewRepository.GetBrew(id, includeAdditionalInfo);

            if (brew == null)
            {
                return NotFound();
            }

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForBrew(id, includeAdditionalInfo);

                var linkedResourceToReturn = includeAdditionalInfo ?
                    _mapper.Map<BrewDto>(brew).ShapeData(null) as IDictionary<string, object> :
                    _mapper.Map<BrewWithoutAdditionalInfoDto>(brew).ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return includeAdditionalInfo ?
                Ok(_mapper.Map<BrewDto>(brew).ShapeData(null)) :
                Ok(_mapper.Map<BrewWithoutAdditionalInfoDto>(brew).ShapeData(null));
        }

        /// <summary>
        /// Create a new brew based on an existing recipe
        /// </summary>
        [HttpPost(Name = "CreateBrew")]
        [ProducesResponseType(typeof(BrewDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<BrewDto> CreateBrew([FromBody] BrewForCreationDto brew,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var includeSteps = true;
            var includeAdditionalInfo = true;
            var finalBrew = _mapper.Map<Entities.Brew>(brew);

            _homebrewRepository.AddBrew(finalBrew);
            finalBrew.Recipe = _homebrewRepository.GetRecipe(brew.RecipeId, includeSteps);
            _homebrewRepository.Save();

            var brewToReturn = _mapper.Map<Models.BrewDto>(finalBrew);

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForBrew(brewToReturn.Id, includeAdditionalInfo);

                var linkedResourceToReturn = brewToReturn.ShapeData(null)
                    as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return CreatedAtRoute(
                    "GetBrew",
                    new { id = linkedResourceToReturn["Id"], includeAdditionalInfo },
                    linkedResourceToReturn);
            }

            return CreatedAtRoute(
                "GetBrew",
                new { id = brewToReturn.Id, includeAdditionalInfo },
                brewToReturn);
        }

        /// <summary>
        /// Updates an existing brew based on the brew id, the brew model supplied is what is pushed into the
        /// persistent storage.
        /// </summary>
        [HttpPut("{id}", Name = "UpdateBrew")]
        [ProducesResponseType(typeof(BrewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateBrew(int id,
            [FromBody] BrewForUpdateDto brew,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            if (!_homebrewRepository.RecipeExists(brew.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brew.RecipeId}] for the {brew.Name} brew must be valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var includeAdditionalInfo = true;
            var brewEntity = _homebrewRepository.GetBrew(id, includeAdditionalInfo);
            if (brewEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(brew, brewEntity);
            _homebrewRepository.UpdateBrew(brewEntity);
            _homebrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForBrew(brewEntity.Id, includeAdditionalInfo);

                var linkedResourceToReturn = _mapper.Map<BrewDto>(brewEntity)
                                                .ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        /// <summary>
        /// Updates an existing brew based on the brew id, only the properties supplied will be updated
        /// </summary>
        [HttpPatch("{id}", Name = "PartiallyUpdateBrew")]
        public ActionResult PartiallyUpdateBrew(int id,
            [FromBody] JsonPatchDocument<BrewForUpdateDto> patchDoc,
            [FromHeader(Name = ExtendedControllerBase.ACCEPT)] string mediaTypes)
        {
            var splitMediaTypes = mediaTypes.Split(',');
            if (!MediaTypeHeaderValue.TryParseList(splitMediaTypes,
                out IList<MediaTypeHeaderValue> parsedMediaTypes))
            {
                return BadRequest();
            }

            var includeAdditionalInfo = true;
            var brewEntity = _homebrewRepository.GetBrew(id, includeAdditionalInfo);
            if (brewEntity == null)
            {
                return NotFound();
            }

            var brewToPatch = _mapper.Map<BrewForUpdateDto>(brewEntity);

            patchDoc.ApplyTo(brewToPatch, ModelState);

            if (!_homebrewRepository.RecipeExists(brewToPatch.RecipeId))
            {
                ModelState.AddModelError(
                            "Recipe ID",
                            $"The associated recipe id [{brewToPatch.RecipeId}] for the {brewToPatch.Name} brew must be valid.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(brewToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(brewToPatch, brewEntity);
            _homebrewRepository.UpdateBrew(brewEntity);
            _homebrewRepository.Save();

            if (parsedMediaTypes.Any(pmt => pmt.SubTypeWithoutSuffix.EndsWith(this.HATEOAS, StringComparison.InvariantCultureIgnoreCase)))
            {
                var links = CreateLinksForBrew(brewEntity.Id, includeAdditionalInfo);

                var linkedResourceToReturn = _mapper.Map<BrewDto>(brewEntity)
                                                .ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add(this.LINKS, links);

                return Ok(linkedResourceToReturn);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes the brew with the supplied id
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteBrew")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteBrew(int id)
        {
            var brewEntity = _homebrewRepository.GetBrew(id, false);
            if (brewEntity == null)
            {
                return NotFound();
            }

            _homebrewRepository.DeleteBrew(brewEntity);
            _homebrewRepository.Save();

            return NoContent();
        }

        [HttpOptions]
        public ActionResult GetBrewsOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.GET}, {this.HEAD}, {this.OPTIONS}");
            return Ok();
        }

        [HttpOptions("{id}")]
        public ActionResult GetBrewOptions()
        {
            Response.Headers.Add(this.ALLOW, $"{this.DELETE}, {this.GET}, {this.OPTIONS}, {this.PATCH}, {this.POST}, {this.PUT}");
            return Ok();
        }

        #region Private functions

        private string CreateBrewResourceUri(
            BrewsResourceParameters brewsResourceParameters,
            ETypeOfResourceUri type)
        {
            var linkProps = new ExpandoObject() as IDictionary<string, Object>;

            if (brewsResourceParameters.MinRating > 0)
            {
                linkProps.Add("minRating", brewsResourceParameters.MinRating);
            }

            switch (type)
            {
                case ETypeOfResourceUri.PreviousPage:
                    linkProps.Add("orderBy", brewsResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", brewsResourceParameters.PageNumber - 1);
                    linkProps.Add("pageSize", brewsResourceParameters.PageSize);
                    linkProps.Add("searchQuery", brewsResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);

                case ETypeOfResourceUri.NextPage:
                    linkProps.Add("orderBy", brewsResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", brewsResourceParameters.PageNumber + 1);
                    linkProps.Add("pageSize", brewsResourceParameters.PageSize);
                    linkProps.Add("searchQuery", brewsResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);

                case ETypeOfResourceUri.Current:
                default:
                    linkProps.Add("orderBy", brewsResourceParameters.OrderBy);
                    linkProps.Add("pageNumber", brewsResourceParameters.PageNumber);
                    linkProps.Add("pageSize", brewsResourceParameters.PageSize);
                    linkProps.Add("searchQuery", brewsResourceParameters.SearchQuery);

                    return Url.Link("GetBrews", linkProps);
            }
        }

        private IEnumerable<LinkDto> CreateLinksForBrew(int id, bool includeAdditionalInfo)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetBrew", new { id, includeAdditionalInfo }),
                this.SELF,
                this.GET));

            links.Add(
                new LinkDto(Url.Link("UpdateBrew", new { id }),
                "update_brew",
                this.PUT));

            links.Add(
                new LinkDto(Url.Link("PartiallyUpdateBrew", new { id }),
                "partially_update_brew",
                this.PATCH));

            links.Add(
                new LinkDto(Url.Link("DeleteBrew", new { id }),
                "delete_brew",
                this.DELETE));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForBrews(
            BrewsResourceParameters brewsResourceParameters,
            bool hasNext,
            bool hasPrevious)
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(CreateBrewResourceUri(
                    brewsResourceParameters, ETypeOfResourceUri.Current),
                this.SELF,
                this.GET));

            if (hasNext)
            {
                links.Add(
                    new LinkDto(CreateBrewResourceUri(
                        brewsResourceParameters, ETypeOfResourceUri.NextPage),
                    this.NEXT_PAGE,
                    this.GET));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateBrewResourceUri(
                        brewsResourceParameters, ETypeOfResourceUri.PreviousPage),
                    this.PREVIOUS_PAGE,
                    this.GET));
            }

            return links;
        }

        #endregion Private functions
    }
}